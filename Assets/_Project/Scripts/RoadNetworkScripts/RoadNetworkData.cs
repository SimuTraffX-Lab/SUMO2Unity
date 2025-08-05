#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using NetFile;  // For JunctionTypeType, etc.

namespace Assets.Scripts.SUMOImporter.NetFileComponents
{
    //===============================================================
    // 1) RoadLaneData (was NetFileLane)
    //===============================================================
    public class RoadLaneData
    {
        public string laneId;
        public int laneIndex;
        public double speedLimit;
        public double laneLength;
        public double laneWidth;      // <-- new
        public List<double[]> shapePoints;

        public RoadLaneData(string identifier)
        {
            laneId = identifier;
        }

        public RoadLaneData(string identifier, int index, double speed, double length, double width, string shapeStr)
        {
            laneId = identifier;
            laneIndex = index;
            speedLimit = speed;
            laneLength = length;
            laneWidth = width;       // <-- set it here
            ParseShapeCoordinates(shapeStr);
        }

        private void ParseShapeCoordinates(string shapeStr)
        {
            shapePoints = new List<double[]>();
            foreach (string chunk in shapeStr.Split(' '))
            {
                string[] xy = chunk.Split(',');
                double xC = Convert.ToDouble(xy[0]);
                double yC = Convert.ToDouble(xy[1]);
                shapePoints.Add(new double[] { xC, yC });
            }
        }

        internal void UpdateLaneData(int index, double speed, double length, double width, string shapeStr)
        {
            laneIndex = index;
            speedLimit = speed;
            laneLength = length;
            laneWidth = width;    // <-- updated
            ParseShapeCoordinates(shapeStr);
        }
    }

    //===============================================================
    // 2) RoadJunctionData (was NetFileJunction)
    //===============================================================
    public class RoadJunctionData
    {
        public string junctionId;
        public JunctionTypeType junctionType;
        public float xPos;
        public float yPos;
        public float zPos;

        public List<RoadLaneData> incomingLanes;
        public List<double[]> shapePoints;

        public RoadJunctionData(
            string juncId,
            JunctionTypeType jtType,
            float xCoord,
            float yCoord,
            float zCoord,
            string incLanes,
            string shapeCoordinates)
        {
            junctionId = juncId;
            junctionType = jtType;
            xPos = xCoord;
            yPos = yCoord;
            zPos = zCoord;

            var builder = RoadNetworkBuilder.Singleton;
            if (builder == null)
            {
                Debug.LogError("RoadNetworkBuilder.Singleton is null! Ensure a GameObject with RoadNetworkBuilder is in the scene.");
                return;
            }

            // 1) Parse incoming lanes
            incomingLanes = new List<RoadLaneData>();
            if (!string.IsNullOrEmpty(incLanes))
            {
                foreach (string laneIdentifier in incLanes.Split(' '))
                {
                    RoadLaneData laneObj = new RoadLaneData(laneIdentifier);
                    incomingLanes.Add(laneObj);

                    // Also add to global dictionary if missing
                    if (!builder.laneRecords.ContainsKey(laneObj.laneId))
                    {
                        builder.laneRecords.Add(laneObj.laneId, laneObj);
                    }
                }
            }

            // 2) Parse shape coordinates
            shapePoints = new List<double[]>();
            if (!string.IsNullOrEmpty(shapeCoordinates))
            {
                foreach (string coordPair in shapeCoordinates.Split(' '))
                {
                    string[] coords = coordPair.Split(',');
                    double xC = Convert.ToDouble(coords[0]);
                    double yC = Convert.ToDouble(coords[1]);
                    shapePoints.Add(new double[] { xC, yC });
                }
            }
        }
    }

    //===============================================================
    // 3) RoadEdgeData (was NetFileEdge)
    //===============================================================
    public class RoadEdgeData
    {
        private string edgeId;
        private RoadJunctionData fromJunction;
        private RoadJunctionData toJunction;
        private int edgePriority;
        private List<RoadLaneData> laneDataList;

        public RoadEdgeData(string eId, string fromId, string toId, string priority, string shapeStr)
        {
            edgeId = eId;
            edgePriority = Convert.ToInt32(priority);
            laneDataList = new List<RoadLaneData>();

            var builder = RoadNetworkBuilder.Singleton;
            if (builder == null)
            {
                Debug.LogError("RoadNetworkBuilder.Singleton is null! Make sure RoadNetworkBuilder is in the scene before creating RoadEdgeData.");
                return;
            }

            // Validate fromId
            if (string.IsNullOrEmpty(fromId))
            {
                Debug.LogError($"Edge {eId} has a null/empty 'fromId' value.");
            }
            else if (!builder.junctionRecords.ContainsKey(fromId))
            {
                Debug.LogError($"Edge {eId}: 'from' junction key '{fromId}' not found in builder.junctionRecords.");
            }
            else
            {
                fromJunction = builder.junctionRecords[fromId];
            }

            // Validate toId
            if (string.IsNullOrEmpty(toId))
            {
                Debug.LogError($"Edge {eId} has a null/empty 'toId' value.");
            }
            else if (!builder.junctionRecords.ContainsKey(toId))
            {
                Debug.LogError($"Edge {eId}: 'to' junction key '{toId}' not found in builder.junctionRecords.");
            }
            else
            {
                toJunction = builder.junctionRecords[toId];
            }
        }

        public int GetEdgePriority()
        {
            return edgePriority;
        }

        // Modified signature to accept width
        public void AddLaneData(string laneId, string index, float speed, float length, float width, string shapeStr)
        {
            var builder = RoadNetworkBuilder.Singleton;
            if (builder == null)
            {
                Debug.LogError("RoadNetworkBuilder.Singleton is null in AddLaneData!");
                return;
            }

            // Update the global dictionary entry
            RoadLaneData laneObj = builder.laneRecords[laneId];
            laneObj.UpdateLaneData(
                Convert.ToInt32(index),
                Convert.ToDouble(speed),
                Convert.ToDouble(length),
                Convert.ToDouble(width),
                shapeStr
            );

            // Also store a local copy with width
            laneDataList.Add(
                new RoadLaneData(
                    laneId,
                    Convert.ToInt32(index),
                    speed,
                    length,
                    width,
                    shapeStr
                )
            );
        }

        public List<RoadLaneData> GetLaneDataList() => laneDataList;


        public RoadJunctionData GetFromJunction()
        {
            return fromJunction;
        }

        public RoadJunctionData GetToJunction()
        {
            return toJunction;
        }

        public string GetEdgeId()
        {
            return edgeId;
        }
    }

    //===============================================================
    // 4) PolygonShapeData (was Shape)
    //===============================================================
    public class PolygonShapeData
    {
        public List<double[]> polygonPoints = new List<double[]>();

        public PolygonShapeData()
        {
            polygonPoints.Clear();
        }

        public void AddPoint(double x, double y)
        {
            polygonPoints.Add(new double[] { x, y });
        }

        /// <summary>
        /// If the last coordinate is a duplicate of the first (common in SUMO),
        /// remove it to avoid repeated vertices.
        /// </summary>
        public void RemoveDuplicateEndPoint()
        {
            if (polygonPoints.Count > 1)
            {
                double[] first = polygonPoints[0];
                double[] last = polygonPoints[polygonPoints.Count - 1];

                bool isDup =
                    (Math.Abs(first[0] - last[0]) < 0.00001) &&
                    (Math.Abs(first[1] - last[1]) < 0.00001);

                if (isDup)
                {
                    polygonPoints.RemoveAt(polygonPoints.Count - 1);
                }
            }
        }
    }
}

//===============================================================
// 5) MeshTriangulator (was Triangulator)
//===============================================================
public class MeshTriangulator
{
    private readonly List<Vector2> polygonVertices = new List<Vector2>();

    public MeshTriangulator(Vector2[] points)
    {
        polygonVertices = new List<Vector2>(points);
    }

    public int[] GenerateIndices()
    {
        List<int> indices = new List<int>();
        int n = polygonVertices.Count;
        if (n < 3)
            return indices.ToArray();

        int[] V = new int[n];
        if (CalculateArea() > 0)
        {
            for (int v = 0; v < n; v++)
                V[v] = v;
        }
        else
        {
            for (int v = 0; v < n; v++)
                V[v] = (n - 1) - v;
        }

        int nv = n;
        int count = 2 * nv;
        for (int m = 0, v = nv - 1; nv > 2;)
        {
            if ((count--) <= 0)
                return indices.ToArray();

            int u = v;
            if (nv <= u)
                u = 0;
            v = u + 1;
            if (nv <= v)
                v = 0;
            int w = v + 1;
            if (nv <= w)
                w = 0;

            if (CanSnipTriangle(u, v, w, nv, V))
            {
                int a, b, c, s, t;
                a = V[u];
                b = V[v];
                c = V[w];
                indices.Add(a);
                indices.Add(b);
                indices.Add(c);
                m++;

                for (s = v, t = v + 1; t < nv; s++, t++)
                    V[s] = V[t];

                nv--;
                count = 2 * nv;
            }
        }

        indices.Reverse();
        return indices.ToArray();
    }

    private float CalculateArea()
    {
        int n = polygonVertices.Count;
        float A = 0.0f;
        for (int p = n - 1, q = 0; q < n; p = q++)
        {
            Vector2 pval = polygonVertices[p];
            Vector2 qval = polygonVertices[q];
            A += pval.x * qval.y - qval.x * pval.y;
        }
        return (A * 0.5f);
    }

    private bool CanSnipTriangle(int u, int v, int w, int n, int[] V)
    {
        Vector2 A = polygonVertices[V[u]];
        Vector2 B = polygonVertices[V[v]];
        Vector2 C = polygonVertices[V[w]];

        if (Mathf.Epsilon > (((B.x - A.x) * (C.y - A.y)) - ((B.y - A.y) * (C.x - A.x))))
            return false;

        for (int p = 0; p < n; p++)
        {
            if ((p == u) || (p == v) || (p == w))
                continue;

            Vector2 P = polygonVertices[V[p]];
            if (IsPointInTriangle(A, B, C, P))
                return false;
        }
        return true;
    }

    private bool IsPointInTriangle(Vector2 A, Vector2 B, Vector2 C, Vector2 P)
    {
        float ax = C.x - B.x;
        float ay = C.y - B.y;
        float bx = A.x - C.x;
        float by = A.y - C.y;
        float cx = B.x - A.x;
        float cy = B.y - A.y;
        float apx = P.x - A.x;
        float apy = P.y - A.y;
        float bpx = P.x - B.x;
        float bpy = P.y - B.y;
        float cpx = P.x - C.x;
        float cpy = P.y - C.y;

        float aCrossBP = ax * bpy - ay * bpx;
        float cCrossAP = cx * apy - cy * apx;
        float bCrossCP = bx * cpy - by * cpx;

        return (aCrossBP >= 0.0f && bCrossCP >= 0.0f && cCrossAP >= 0.0f);
    }
}
#endif
