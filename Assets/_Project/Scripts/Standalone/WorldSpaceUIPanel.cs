// WorldSpaceUIPanel.cs

using UnityEngine;

public class WorldSpaceUIPanel : MonoBehaviour
{
    private GameObject targetObject;

    // The SelectionManager will call this to give us the selected object
    public void Initialize(GameObject target)
    {
        targetObject = target;
    }

    // Create public methods for your buttons to call
    public void OnDeleteButtonClicked()
    {
        if (targetObject != null)
        {
            Debug.Log("Deleting object: " + targetObject.name);
            Destroy(targetObject);
            Destroy(this.gameObject); // Destroy the UI panel itself
        }
    }

    public void OnChangeMaterialButtonClicked()
    {
        if (targetObject != null)
        {
            Debug.Log("Changing material of: " + targetObject.name);
            // Example: Change to a bright red material
            var renderer = targetObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                // You would load a material from Resources here for it to work in a build
                Material redMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                redMaterial.color = Color.red;
                renderer.material = redMaterial;
            }
        }
    }
}