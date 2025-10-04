using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Camera mainCamera;
    private GameObject[] objs;

    void Start()
    {
        mainCamera = Camera.main;
        objs = new GameObject[transform.childCount];
        for(int i = 0; i < transform.childCount; i++)
        {
            objs[i] = transform.GetChild(i).gameObject;
        }
    }

    void Update()
    {
        foreach(GameObject obj in objs)
        {
            if(obj != null)
            {
                obj.transform.LookAt(obj.transform.position + mainCamera.transform.rotation * Vector3.forward,
            mainCamera.transform.rotation * Vector3.up);
            }
            if (obj.name == "401Crown")
            {
                Vector3 newRotation = obj.transform.eulerAngles;
                newRotation.x += 90f;
                obj.transform.eulerAngles = newRotation;
            }
        }
        
    }
}
