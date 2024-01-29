using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private SumoUnityController sumoUnity;
    #region Pathway Variables
    public bool isDriveThroughOn = true;
    public Transform[] pathwayPositions;
    public int pathwayIndex;
    public float lerpValue;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        sumoUnity = FindAnyObjectByType<SumoUnityController>();
        setPathwayPositions();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (isDriveThroughOn)
        {
            if (pathwayIndex < pathwayPositions.Length - 2)
            {
                float distanceToNextPosition = Vector3.Distance(sumoUnity.mainCamera.transform.position, pathwayPositions[pathwayIndex].position);

                // Adjust lerpValue based on speed and distance
                lerpValue = 10f * Time.deltaTime / distanceToNextPosition;

                // Move towards next position
                sumoUnity.mainCamera.transform.position = Vector3.Lerp(sumoUnity.mainCamera.transform.position, pathwayPositions[pathwayIndex].position, lerpValue);
                sumoUnity.mainCamera.transform.rotation = Quaternion.Lerp(sumoUnity.mainCamera.transform.rotation, pathwayPositions[pathwayIndex].rotation, lerpValue);


                // Check if reached the current target position
                if (Vector3.Distance(sumoUnity.mainCamera.transform.position, pathwayPositions[pathwayIndex].position) < 0.1f)
                {
                    pathwayIndex++;
                    if (pathwayIndex > pathwayPositions.Length)
                    {
                        isDriveThroughOn = false;
                    }
                }
            }

        }
    }

    private void setPathwayPositions()
    {
        pathwayPositions = new Transform[transform.childCount + 1];
        for (int i = 1; i <= transform.childCount; i++)
        {
            pathwayPositions[i - 1] = transform.Find($"{i}");
        }

    }
}
