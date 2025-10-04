using UnityEngine;

public class AddMeshColliders : MonoBehaviour
{
    void Start()
    {
        AddCollidersToChildren(transform);
    }

    private void AddCollidersToChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            // Add a MeshCollider if the child has a MeshFilter but no Collider.
            if (child.GetComponent<MeshFilter>() != null && child.GetComponent<Collider>() == null)
            {
                MeshCollider meshCollider = child.gameObject.AddComponent<MeshCollider>();
                meshCollider.sharedMesh = child.GetComponent<MeshFilter>().sharedMesh;
            }

            // Recursively call this function for all children.
            if (child.childCount > 0)
            {
                AddCollidersToChildren(child);
            }
        }
    }
}