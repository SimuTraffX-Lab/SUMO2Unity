using Unity.VisualScripting;
using UnityEngine;
using static SimulationController;

public class VehicleController : MonoBehaviour
{
    private Rigidbody rb;

    private Vector3 lastKnownPosition;
    private Quaternion lastKnownRotation;
    private Vector3 currentKnownPosition;
    private Quaternion currentKnownRotation;
    private float lastUpdateTime;
    private float currentUpdateTime;

    private float currentLongSpeed = 0f; // Speed obtained from SimulationController
    private float currentVerticalSpeed = 0f;
    private float currentLateralSpeed = 0f;

    private float lastRecordedTime;
    private float unityComputedSpeed;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        rb.isKinematic = false;
        rb.useGravity = false;
        rb.linearDamping = 1f;

        currentKnownPosition = transform.position;
        lastKnownPosition = transform.position;
        currentKnownRotation = transform.rotation;
        lastKnownRotation = transform.rotation;

        float startTime = Time.time;
        lastUpdateTime = startTime;
        currentUpdateTime = startTime;
    }

    public void UpdateTarget(Vector3 newTargetPosition, Quaternion newTargetRotation, float long_speed, float vertical_speed, float lateral_speed)
    {
        float currentTime = Time.time;

        // Move current data to last
        lastKnownPosition = currentKnownPosition;
        lastKnownRotation = currentKnownRotation;
        lastUpdateTime = currentUpdateTime;

        // Set the new data as current
        currentKnownPosition = newTargetPosition;
        currentKnownRotation = newTargetRotation;
        currentUpdateTime = currentTime;

        // Update current speed from SUMO data
        currentLongSpeed = long_speed;
        currentVerticalSpeed = vertical_speed;
        currentLateralSpeed = lateral_speed;

        //float deltaTime = Time.time - lastRecordedTime;
        //if (name == "f_1.0")
        //{
        //    float distanceToNewTarget = Vector3.Distance(lastKnownPosition, currentKnownPosition);
        //    unityComputedSpeed = distanceToNewTarget / deltaTime;


        //    Debug.Log($"[VehicleController: {name}] UpdateTarget() at {currentTime:F3}s: " +
        //              $"Speed: {unityComputedSpeed:F2} m/s, " +
        //              $"Distance: {distanceToNewTarget:F3}, " +
        //              $"Old Pos: {lastKnownPosition}, New Pos: {currentKnownPosition}");
        //    // Update references
        //    lastRecordedTime = Time.time;
        //}

    }

    private void FixedUpdate()
    {
        float interval = currentUpdateTime - lastUpdateTime;
        if (interval <= 0f)
        {
            rb.linearVelocity = Vector3.zero;
            rb.MoveRotation(currentKnownRotation);
            return;
        }

        rb.MoveRotation(currentKnownRotation);

        // Your forward direction might differ; assuming x-axis is forward and y is up:
        Vector3 longVelocity = transform.TransformDirection(Vector3.right * currentLongSpeed);
        Vector3 verticalVelocity = transform.TransformDirection(Vector3.up * currentVerticalSpeed);
        Vector3 lateralSpeed = transform.TransformDirection(Vector3.forward * currentLateralSpeed);

        rb.linearVelocity = longVelocity + verticalVelocity + lateralSpeed;
        //Debug.Log($"currentLongSpeed+ {longVelocity}, verticalVelocity: {verticalVelocity}, lateralSpeed + {lateralSpeed}");

        // If you also want the vehicle to smoothly move to target position:
        transform.localPosition = Vector3.Lerp(transform.localPosition, currentKnownPosition, 0.02f);
    }
}