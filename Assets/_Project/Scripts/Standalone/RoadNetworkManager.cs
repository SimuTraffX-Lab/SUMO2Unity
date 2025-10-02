using UnityEngine;

public class RoadNetworkManager : MonoBehaviour
{
    private static RoadNetworkManager instance;

    void Awake()
    {
        // Ensure only one instance exists across scenes
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // This keeps the object when switching scenes
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
        }
    }
}
