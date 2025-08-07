// ==============================
// RoadNetworkBuilder.cs
// (Decals clipped by sampling spans outside junction polygons)
// ==============================
#if UNITY_EDITOR
using Assets.Scripts.SUMOImporter.NetFileComponents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using NetFile;

public class RoadNetworkBuilder : MonoBehaviour
{
    public static RoadNetworkBuilder Singleton { get; private set; }
    private void Awake() => Singleton = this;
    public void InitializeInEditMode()
    {
        if (Singleton != this)
        {
            Singleton = this;
            Debug.Log("RoadNetworkBuilder: Editor-based initialization complete.");
        }
    }
    private void OnDestroy()
    {
        if (Singleton == this) Singleton = null;
    }

    [Header("Materials (Road, Junction, Decals)")]
    public Material roadSurfaceMaterial;
    public Material junctionSurfaceMaterial;
    public Material roadMarkingMaterial;

    [Header("Polygon Types (Wood/Terrain/Roadside/Residential)")]
    public Material polygonWoodMaterial;
    public Material polygonTerrainMaterial;
    public Material polygonRoadsideMaterial;
    public Material polygonResidentialMaterial;
    private Material polygonFallbackMaterial;

    private GameObject roadNetworkRoot;

    // ★ NEW: Ground-layer support --------------------------------------------
    private const string groundLayerName = "Ground";
    private int groundLayer = -1;
    // ------------------------------------------------------------------------

    public Dictionary<string, RoadJunctionData> junctionRecords;
    public Dictionary<string, RoadLaneData> laneRecords;
    public Dictionary<string, RoadEdgeData> edgeRecords;
    public Dictionary<string, PolygonShapeData> polygonShapes;

    private string sumoXmlFolderPath;

    private float minX = 0f, minY = 0f, maxX = 0f, maxY = 0f;
    private float originX = 0f, originY = 0f;

    private const float laneMeshScaleWidth = 3.2f;
    private const float laneUvVerticalScale = 5f;
    private const float laneUvHorizontalScale = 1f;

    private readonly Dictionary<string, float> laneWidthMap = new();

    // NEW: cache junction polygons (Unity XZ plane)
    private readonly List<Vector2[]> _junctionPolys2D = new();

    public void LoadSumoXmlFiles(string sumoFilesFolder)
    {
        if (roadNetworkRoot != null)
        {
            DestroyImmediate(roadNetworkRoot);
            roadNetworkRoot = null;
        }
        laneWidthMap.Clear();
        junctionRecords?.Clear();
        laneRecords?.Clear();
        edgeRecords?.Clear();
        polygonShapes?.Clear();
        _junctionPolys2D.Clear();

        sumoXmlFolderPath = sumoFilesFolder;

        // ★ NEW: cache “Ground” layer index once
        if (groundLayer < 0) groundLayer = LayerMask.NameToLayer(groundLayerName);
        if (groundLayer < 0)
            Debug.LogWarning($"Layer \"{groundLayerName}\" does not exist – objects will keep their current layer.");

        roadNetworkRoot = new GameObject("RoadNetworkRoot");
        if (groundLayer >= 0) roadNetworkRoot.layer = groundLayer;        // ★ NEW

        var netFilePath = Path.Combine(sumoXmlFolderPath, "Sumo2Unity.net.xml");
        var polyFilePath = Path.Combine(sumoXmlFolderPath, "Sumo2Unity.poly.xml");

        laneRecords = new();
        edgeRecords = new();
        junctionRecords = new();
        polygonShapes = new();

        NetType netFile;
        {
            var serializer = new XmlSerializer(typeof(NetType));
            using var fs = new FileStream(netFilePath, FileMode.Open, FileAccess.Read);
            using var rd = new StreamReader(fs);
            netFile = (NetType)serializer.Deserialize(rd);
        }

        if (!string.IsNullOrEmpty(netFile.Location?.ConvBoundary))
        {
            var bounds = netFile.Location.ConvBoundary.Split(',');
            minX = float.Parse(bounds[0]);
            minY = float.Parse(bounds[1]);
            maxX = float.Parse(bounds[2]);
            maxY = float.Parse(bounds[3]);
        }

        if (!string.IsNullOrEmpty(netFile.Location?.NetOffset))
        {
            var p = netFile.Location.NetOffset.Split(',');
            originX = float.Parse(p[0]);
            originY = float.Parse(p[1]);
        }
        else
        {
            originX = minX; originY = minY;
        }

        foreach (JunctionType jt in netFile.Junction)
        {
            if (jt.Type.ToString().Equals("Internal", StringComparison.OrdinalIgnoreCase))
                continue;

            if (string.IsNullOrEmpty(jt.X) || string.IsNullOrEmpty(jt.Y))
            {
                Debug.LogError($"Junction {jt.Id} missing X/Y. Skipping.");
                continue;
            }

            try
            {
                var newJunction = new RoadJunctionData(
                    jt.Id,
                    jt.Type,
                    float.Parse(jt.X),
                    float.Parse(jt.Y),
                    string.IsNullOrEmpty(jt.Z) ? 0f : float.Parse(jt.Z),
                    jt.IncLanes,
                    jt.Shape);

                if (!junctionRecords.ContainsKey(newJunction.junctionId))
                    junctionRecords.Add(newJunction.junctionId, newJunction);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Junction {jt.Id} parse error: {ex.Message}");
            }
        }

        foreach (EdgeType et in netFile.Edge)
        {
            if (string.IsNullOrEmpty(et.From))
            {
                Debug.LogWarning($"Edge {et.Id} has no 'from'. Skipping.");
                continue;
            }

            var newEdge = new RoadEdgeData(et.Id, et.From, et.To, et.Priority, et.Shape);
            edgeRecords[et.Id] = newEdge;

            if (et.Lane == null) continue;

            foreach (LaneType laneType in et.Lane)
            {
                float width = laneType.WidthSpecified ? laneType.Width : laneMeshScaleWidth;
                laneWidthMap[laneType.Id] = width;

                newEdge.AddLaneData(
                    laneType.Id,
                    laneType.Index.ToString(),
                    laneType.Speed,
                    laneType.Length,
                    width,
                    laneType.Shape);
            }
        }

        if (!File.Exists(polyFilePath)) return;

        try
        {
            var serializer = new XmlSerializer(typeof(AdditionalType));
            using FileStream fs = new FileStream(polyFilePath, FileMode.Open);
            using TextReader rd = new StreamReader(fs);
            AdditionalType additionalPolygons = (AdditionalType)serializer.Deserialize(rd);

            foreach (PolygonType poly in additionalPolygons.Poly)
            {
                if (!IsKnownPolygonType(poly.Type)) continue;

                var shapeData = new PolygonShapeData();
                foreach (string pair in poly.Shape.Split(' '))
                {
                    var parts = pair.Split(',');
                    shapeData.AddPoint(Convert.ToDouble(parts[0]), Convert.ToDouble(parts[1]));
                }
                shapeData.RemoveDuplicateEndPoint();
                if (!polygonShapes.ContainsKey(poly.Id))
                    polygonShapes.Add(poly.Id, shapeData);

                if (shapeData.polygonPoints.Count >= 3)
                    BuildPolygonGameObject(shapeData, poly.Id, poly.Type);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to deserialize polygon file: {e}");
        }

        // ★ NEW: ensure polygons just created inherit the Ground layer
        SetLayerRecursively(roadNetworkRoot, groundLayer);
    }

    public void GenerateRoadsAndJunctions()
    {
        // lanes
        int laneCounter = 0;
        foreach (var edgeData in edgeRecords.Values)
        {
            foreach (var laneData in edgeData.GetLaneDataList())
            {
                var lanePoints = new Vector3[laneData.shapePoints.Count];
                for (int i = 0; i < laneData.shapePoints.Count; i++)
                    lanePoints[i] = ToUnity(laneData.shapePoints[i][0], laneData.shapePoints[i][1]);

                float laneWidth = laneWidthMap.TryGetValue(laneData.laneId, out float w) ? w : laneMeshScaleWidth;

                Mesh laneMesh = CreateLaneMesh(lanePoints, laneWidth, laneUvHorizontalScale, laneUvVerticalScale);
                if (laneMesh == null) continue;

                var laneObj = new GameObject($"LaneSegment_{laneCounter++}");
                laneObj.transform.SetParent(roadNetworkRoot.transform);
                if (groundLayer >= 0) laneObj.layer = groundLayer;  // ★ NEW
                var mf = laneObj.AddComponent<MeshFilter>();
                var mr = laneObj.AddComponent<MeshRenderer>();
                mf.sharedMesh = laneMesh;
                mr.sharedMaterial = roadSurfaceMaterial ?? GetFallbackMaterial();

                // swapped names
                SpawnMarkingDecals(ExtractLeftSideVertices(laneMesh), "LaneMarking_Right", laneObj.transform);
                SpawnMarkingDecals(ExtractRightSideVertices(laneMesh), "LaneMarking_Left", laneObj.transform);

                var ctrl = laneObj.AddComponent<LaneSegmentDecalController>();
                ctrl.solidDepth = 3f;
                ctrl.brokenDepth = 1.5f;
            }
        }

        // junctions
        int junctionCounter = 0;
        foreach (RoadJunctionData j in junctionRecords.Values)
        {
            if (j.shapePoints.Count < 3) continue;

            var verts2D = new Vector2[j.shapePoints.Count];
            for (int i = 0; i < j.shapePoints.Count; i++)
            {
                double[] xy = j.shapePoints[i];
                verts2D[i] = new Vector2((float)(xy[0] - originX), (float)(xy[1] - originY));
            }

            // cache for clipping
            _junctionPolys2D.Add((Vector2[])verts2D.Clone());

            MeshTriangulator triangulator = new MeshTriangulator(verts2D);
            int[] triIndices = triangulator.GenerateIndices();

            var verts3D = new Vector3[verts2D.Length];
            for (int i = 0; i < verts2D.Length; i++)
                verts3D[i] = new Vector3(verts2D[i].x, 0f, verts2D[i].y);

            Mesh junctionMesh = new Mesh
            {
                name = $"Junction_{j.junctionId}",
                vertices = verts3D,
                triangles = triIndices
            };
            junctionMesh.RecalculateNormals();
            junctionMesh.RecalculateBounds();

            Bounds b = junctionMesh.bounds;
            var uvArr = new Vector2[verts3D.Length];
            for (int i = 0; i < verts3D.Length; i++)
                uvArr[i] = new Vector2(
                    (verts3D[i].x - b.min.x) / b.size.x,
                    (verts3D[i].z - b.min.z) / b.size.z);
            junctionMesh.uv = uvArr;

            GameObject jObj = new GameObject($"Junction_{junctionCounter++}");
            jObj.transform.SetParent(roadNetworkRoot.transform);
            if (groundLayer >= 0) jObj.layer = groundLayer;           // ★ NEW
            var jMf = jObj.AddComponent<MeshFilter>();
            var jMr = jObj.AddComponent<MeshRenderer>();
            jMf.mesh = junctionMesh;
            jMr.material = junctionSurfaceMaterial ?? GetFallbackMaterial();
        }

        // ★ NEW: make sure every child built above is on the Ground layer
        SetLayerRecursively(roadNetworkRoot, groundLayer);
    }

    // -------------------- helper --------------------------------------------
    private static void SetLayerRecursively(GameObject obj, int layer)
    {
        if (layer < 0) return;
        obj.layer = layer;
        foreach (Transform child in obj.transform)
            SetLayerRecursively(child.gameObject, layer);
    }
    // ------------------------------------------------------------------------

    private Vector3 ToUnity(double x, double y) => new((float)(x - originX), 0f, (float)(y - originY));

    private void BuildPolygonGameObject(PolygonShapeData shapeData, string polygonId, string polygonType)
    {
        var vertices2D = new Vector2[shapeData.polygonPoints.Count];
        for (int i = 0; i < shapeData.polygonPoints.Count; i++)
        {
            double[] xy = shapeData.polygonPoints[i];
            vertices2D[i] = new Vector2((float)(xy[0] - originX), (float)(xy[1] - originY));
        }

        MeshTriangulator tri = new MeshTriangulator(vertices2D);
        int[] indices = tri.GenerateIndices();

        var vertices3D = new Vector3[vertices2D.Length];
        for (int i = 0; i < vertices2D.Length; i++)
            vertices3D[i] = new Vector3(vertices2D[i].x, 0f, vertices2D[i].y);

        Mesh polyMesh = new Mesh
        {
            name = $"Polygon_{polygonId}",
            vertices = vertices3D,
            triangles = indices
        };
        polyMesh.RecalculateNormals();

        if (polyMesh.normals.Length > 0 && polyMesh.normals[0].y < 0f)
        {
            FlipTriangleWinding(polyMesh);
            polyMesh.RecalculateNormals();
        }
        polyMesh.RecalculateBounds();

        Bounds mBounds = polyMesh.bounds;
        var uvs = new Vector2[vertices3D.Length];
        for (int i = 0; i < vertices3D.Length; i++)
            uvs[i] = new Vector2(
                (vertices3D[i].x - mBounds.min.x) / mBounds.size.x,
                (vertices3D[i].z - mBounds.min.z) / mBounds.size.z);
        polyMesh.uv = uvs;

        GameObject polyGO = new GameObject($"Shape_{polygonId}");
        polyGO.transform.SetParent(roadNetworkRoot.transform);
        if (groundLayer >= 0) polyGO.layer = groundLayer;             // ★ NEW

        if (!string.IsNullOrEmpty(polygonType) && polygonType.ToLowerInvariant().Contains("terrain"))
            polyGO.transform.localPosition = new Vector3(0f, -0.02f, 0f);
        if (!string.IsNullOrEmpty(polygonType) && polygonType.ToLowerInvariant().Contains("roadside"))
            polyGO.transform.localPosition = new Vector3(0f, -0.01f, 0f);
        if (!string.IsNullOrEmpty(polygonType) && polygonType.ToLowerInvariant().Contains("wood"))
            polyGO.transform.localPosition = new Vector3(0f, -0.01f, 0f);
        if (!string.IsNullOrEmpty(polygonType) && polygonType.ToLowerInvariant().Contains("residential"))
            polyGO.transform.localPosition = new Vector3(0f, -0.01f, 0f);

        var mf = polyGO.AddComponent<MeshFilter>();
        var mr = polyGO.AddComponent<MeshRenderer>();
        mf.sharedMesh = polyMesh;
        mr.sharedMaterial = GetPolygonMaterial(polygonType);

        if (!string.IsNullOrEmpty(polygonType) && (polygonType.Equals("terrain", StringComparison.OrdinalIgnoreCase)
            || polygonType.ToLowerInvariant().Contains("terrain")))
        {
            var meshCol = polyGO.AddComponent<MeshCollider>();
            meshCol.sharedMesh = polyMesh;
            meshCol.convex = false;
        }
    }

    private void FlipTriangleWinding(Mesh mesh)
    {
        int[] tris = mesh.triangles;
        for (int i = 0; i < tris.Length; i += 3)
            (tris[i], tris[i + 2]) = (tris[i + 2], tris[i]);
        mesh.triangles = tris;
    }

    private Material GetPolygonMaterial(string type)
    {
        string t = (type ?? string.Empty).ToLowerInvariant();
        if (t.Contains("wood") && polygonWoodMaterial != null) return polygonWoodMaterial;
        if (t.Contains("terrain") && polygonTerrainMaterial != null) return polygonTerrainMaterial;
        if (t.Contains("roadside") && polygonRoadsideMaterial != null) return polygonRoadsideMaterial;
        if (t.Contains("residential") && polygonResidentialMaterial != null) return polygonResidentialMaterial;
        return polygonFallbackMaterial ?? GetFallbackMaterial();
    }

    private bool IsKnownPolygonType(string t)
    {
        if (string.IsNullOrEmpty(t)) return false;
        var l = t.ToLowerInvariant();
        return l.Contains("wood") || l.Contains("terrain") || l.Contains("roadside") || l.Contains("residential");
    }

    private Material GetFallbackMaterial() => new Material(Shader.Find("Standard"));

    private Mesh CreateLaneMesh(Vector3[] lanePoints, float roadWidth, float uvScaleU, float uvScaleV)
    {
        if (lanePoints.Length < 2) return null;

        int segmentCount = lanePoints.Length - 1;
        var vertices = new Vector3[segmentCount * 4];
        var uvs = new Vector2[segmentCount * 4];
        var triangles = new int[segmentCount * 6];

        float accumulatedDist = 0f;

        for (int i = 0; i < segmentCount; i++)
        {
            Vector3 p0 = lanePoints[i];
            Vector3 p1 = lanePoints[i + 1];
            Vector3 dir = (p1 - p0).normalized;
            float segLen = Vector3.Distance(p0, p1);
            Vector3 perp = new Vector3(-dir.z, 0f, dir.x) * (roadWidth * 0.5f);

            Vector3 leftA = p0 + perp;
            Vector3 rightA = p0 - perp;
            Vector3 leftB = p1 + perp;
            Vector3 rightB = p1 - perp;

            int v = i * 4;
            vertices[v] = leftA;
            vertices[v + 1] = rightA;
            vertices[v + 2] = leftB;
            vertices[v + 3] = rightB;

            int t = i * 6;
            triangles[t] = v;
            triangles[t + 1] = v + 2;
            triangles[t + 2] = v + 1;
            triangles[t + 3] = v + 2;
            triangles[t + 4] = v + 3;
            triangles[t + 5] = v + 1;

            float v0 = accumulatedDist / uvScaleV;
            float v1 = (accumulatedDist + segLen) / uvScaleV;
            float u0 = 0f;
            float u1 = roadWidth / uvScaleU;

            uvs[v] = new Vector2(u0, v0);
            uvs[v + 1] = new Vector2(u1, v0);
            uvs[v + 2] = new Vector2(u0, v1);
            uvs[v + 3] = new Vector2(u1, v1);

            accumulatedDist += segLen;
        }

        Mesh mesh = new Mesh
        {
            name = "LaneMesh",
            vertices = vertices,
            uv = uvs,
            triangles = triangles
        };
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    private Vector3[] ExtractLeftSideVertices(Mesh laneMesh)
    {
        Vector3[] v = laneMesh.vertices;
        if (v.Length < 4) return Array.Empty<Vector3>();
        int segCount = v.Length / 4;
        var pts = new Vector3[segCount + 1];
        for (int i = 0; i < segCount; i++) pts[i] = v[i * 4];
        pts[segCount] = v[(segCount - 1) * 4 + 2];
        return pts;
    }

    private Vector3[] ExtractRightSideVertices(Mesh laneMesh)
    {
        Vector3[] v = laneMesh.vertices;
        if (v.Length < 4) return Array.Empty<Vector3>();
        int segCount = v.Length / 4;
        var pts = new Vector3[segCount + 1];
        for (int i = 0; i < segCount; i++) pts[i] = v[i * 4 + 1];
        pts[segCount] = v[(segCount - 1) * 4 + 3];
        return pts;
    }

    // ───────────────────────── Decal spawning (SPAN based) ───────────────────
    private struct Span { public float s, e; public Span(float s, float e) { this.s = s; this.e = e; } }

    private void SpawnMarkingDecals(Vector3[] boundaryPts, string name, Transform parent)
    {
        if (roadMarkingMaterial == null)
        {
            Debug.LogWarning("No 'roadMarkingMaterial' assigned!");
            return;
        }
        if (boundaryPts == null || boundaryPts.Length < 2) return;

        const float stepSize = 3f;        // spacing between decals (m)
        const float sampleStep = 0.25f;     // resolution for span detection (m)
        Vector3 baseSize = new Vector3(0.1f, 0.2f, 3f);

        int count = boundaryPts.Length;
        float[] cum = new float[count];
        cum[0] = 0f;
        for (int i = 1; i < count; i++)
            cum[i] = cum[i - 1] + Vector3.Distance(boundaryPts[i - 1], boundaryPts[i]);

        float total = cum[count - 1];

        // 1) Detect outside spans
        var spans = new List<Span>();
        bool wasOutside = false;
        float spanStart = 0f;

        for (float d = 0f; d <= total; d += sampleStep)
        {
            GetPointOnPolyline(boundaryPts, cum, d, out Vector3 pos, out _);
            bool inside = IsInsideAnyJunction(pos);
            if (!inside && !wasOutside)
            {
                wasOutside = true;
                spanStart = d;
            }
            else if (inside && wasOutside)
            {
                wasOutside = false;
                spans.Add(new Span(spanStart, d));
            }
        }
        if (wasOutside) spans.Add(new Span(spanStart, total));

        // 2) Spawn decals inside spans only
        foreach (var sp in spans)
        {
            for (float d = sp.s; d <= sp.e; d += stepSize)
            {
                GetPointOnPolyline(boundaryPts, cum, d, out Vector3 center, out Vector3 dir);

                float halfLen = baseSize.z * 0.5f;

                // clamp projector length if near span edges
                float maxBack = Mathf.Min(halfLen, d - sp.s);
                float maxFwd = Mathf.Min(halfLen, sp.e - d);
                float length = maxBack + maxFwd;
                if (length < 0.11f) continue;

                // shift center so the shortened projector still lies fully in the span
                center += dir * (maxFwd - maxBack) * 0.5f;
                center += Vector3.up * 0.01f;

                GameObject decalObj = new GameObject($"{name}_Decal");
                decalObj.transform.SetParent(parent != null ? parent : roadNetworkRoot.transform);
                if (groundLayer >= 0) decalObj.layer = groundLayer;     // ★ NEW
                decalObj.transform.position = center;
                decalObj.transform.rotation = Quaternion.LookRotation(-dir, Vector3.up);

                var proj = decalObj.AddComponent<DecalProjector>();
                proj.material = roadMarkingMaterial;
                proj.size = new Vector3(baseSize.x, baseSize.y, length * 2f); // because length we computed is halfBack+halfFwd
                proj.drawDistance = 250f;
            }
        }
    }

    private static void GetPointOnPolyline(Vector3[] pts, float[] cum, float dist, out Vector3 pos, out Vector3 dir)
    {
        int idx = FindMarkingSegmentIndex(cum, dist);
        if (idx < 0 || idx >= pts.Length - 1)
        {
            pos = pts[pts.Length - 1];
            dir = Vector3.forward;
            return;
        }

        float segStart = cum[idx];
        float segLen = cum[idx + 1] - segStart;
        float t = segLen <= Mathf.Epsilon ? 0f : (dist - segStart) / segLen;

        Vector3 p0 = pts[idx];
        Vector3 p1 = pts[idx + 1];
        pos = Vector3.Lerp(p0, p1, t);
        dir = (p1 - p0).normalized;
    }

    private static int FindMarkingSegmentIndex(float[] cum, float dist)
    {
        int n = cum.Length;
        if (dist > cum[n - 1]) return -1;
        for (int i = 0; i < n - 1; i++)
            if (dist <= cum[i + 1]) return i;
        return -1;
    }

    private bool IsInsideAnyJunction(Vector3 worldPos)
    {
        Vector2 p = new Vector2(worldPos.x, worldPos.z);
        foreach (var poly in _junctionPolys2D)
        {
            if (PointInPolygon(p, poly)) return true;
        }
        return false;
    }

    private static bool PointInPolygon(Vector2 p, Vector2[] poly)
    {
        bool inside = false;
        for (int i = 0, j = poly.Length - 1; i < poly.Length; j = i++)
        {
            Vector2 pi = poly[i];
            Vector2 pj = poly[j];
            bool intersect = ((pi.y > p.y) != (pj.y > p.y)) &&
                             (p.x < (pj.x - pi.x) * (p.y - pi.y) / (pj.y - pi.y + Mathf.Epsilon) + pi.x);
            if (intersect) inside = !inside;
        }
        return inside;
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// LaneSegmentDecalController  (LEFT/RIGHT names swapped logic)
// ─────────────────────────────────────────────────────────────────────────────
[ExecuteInEditMode]
[DisallowMultipleComponent]
public class LaneSegmentDecalController : MonoBehaviour
{
    [Tooltip("Broken Lines in LEFT lane marking (acts on objects named *Right* after swap).")]
    public bool brokenLeft;
    [Tooltip("Broken Lines in RIGHT lane marking (acts on objects named *Left* after swap).")]
    public bool brokenRight;

    [HideInInspector] public float solidDepth = 3f;
    [HideInInspector] public float brokenDepth = 1.5f;

    private bool _prevBrokenLeft;
    private bool _prevBrokenRight;

    private void OnValidate()
    {
        if (brokenLeft != _prevBrokenLeft)
        {
            SetDepthForSide("LaneMarking_Right_Decal", brokenLeft);
            _prevBrokenLeft = brokenLeft;
        }

        if (brokenRight != _prevBrokenRight)
        {
            SetDepthForSide("LaneMarking_Left_Decal", brokenRight);
            _prevBrokenRight = brokenRight;
        }
    }

    private void SetDepthForSide(string prefix, bool broken)
    {
        float targetDepth = broken ? brokenDepth : solidDepth;

        var decals = GetComponentsInChildren<DecalProjector>(true);
        foreach (var d in decals)
        {
            if (!d.name.StartsWith(prefix, StringComparison.Ordinal)) continue;
            Vector3 s = d.size;
            if (Math.Abs(s.z - targetDepth) < 0.0001f) continue;
            s.z = targetDepth;
            d.size = s;
#if UNITY_EDITOR
            EditorUtility.SetDirty(d);
#endif
        }
    }
}
#endif
