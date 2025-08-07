using UnityEngine;

public class VehicleController : MonoBehaviour
{
    private Rigidbody rb;

    private Vector3 lastPos;
    private Quaternion lastRot;
    private Vector3 curPos;
    private Quaternion curRot;
    private float lastTime;
    private float curTime;

    private float curLong, curVert, curLat;
    // set at runtime, after the Inspector value is known
    private float stepLen;
    private float turnThresholdDeg;

    private const float FadeTime = 0.05f;          // how long to ease out spin

    private Vector3 residualAngularVel;           // ★ keeps turn’s leftover spin
    private float residualTimer;                // ★ fade-out countdown

    private void Start()
    {
        rb = GetComponent<Rigidbody>() ?? gameObject.AddComponent<Rigidbody>();

        rb.isKinematic = false;
        rb.useGravity = false;
        rb.linearDamping = 1f;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        curPos = lastPos = transform.position;
        curRot = lastRot = transform.rotation;
        lastTime = curTime = Time.time;
    }

    public void UpdateTarget(Vector3 pos, Quaternion rot,
                             float longSpd, float vertSpd, float latSpd)
    {
        lastPos = curPos; lastRot = curRot; lastTime = curTime;
        curPos = pos; curRot = rot; curTime = Time.time;

        curLong = longSpd; curVert = vertSpd; curLat = latSpd;
    }
    void Awake()
    {
        // Look for the first SimulationController in the scene
        SimulationController sim = FindObjectOfType<SimulationController>();

        if (sim == null)
        {
            Debug.LogError("SimulationController not found!");
            return;
        }

        stepLen = sim.unityStepLength;                 // ← value set in Inspector
        turnThresholdDeg = Mathf.Clamp(stepLen * 40f, 0.25f, 10f);
    }
    private void FixedUpdate()
    {
        float dt = curTime - lastTime;
        if (dt <= 0f)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.MoveRotation(curRot);
            return;
        }

        float headingDelta = Quaternion.Angle(lastRot, curRot);   // degrees

        if (headingDelta < turnThresholdDeg)                      // ─ straight
        {
            /* linear vel from local-axis speeds (ultra smooth) */
            Vector3 vLong = curRot * (Vector3.right * curLong);
            Vector3 vLat = curRot * (Vector3.forward * curLat);
            Vector3 vUp = Vector3.up * curVert;
            rb.linearVelocity = vLong + vLat + vUp;

            /* ① damp residual spin, don’t kill instantly */
            if (residualTimer > 0f)
            {
                residualTimer -= Time.fixedDeltaTime;
                float k = Mathf.Clamp01(residualTimer / FadeTime);
                rb.angularVelocity = residualAngularVel * k;
                if (k <= 0f) rb.MoveRotation(curRot);            // fully aligned
            }
            else
            {
                rb.angularVelocity = Vector3.zero;
                rb.MoveRotation(curRot);
            }
        }
        else                                                      // ─ turning
        {
            rb.linearVelocity = (curPos - lastPos) / dt;
            residualAngularVel = CalcAngularVel(lastRot, curRot, dt); // ★ store
            rb.angularVelocity = residualAngularVel;
            residualTimer = FadeTime;                          // ★ reset
        }

        /* original ultra-smooth positional blend */
        transform.localPosition =
            Vector3.Lerp(transform.localPosition, curPos, 0.02f);
    }

    private static Vector3 CalcAngularVel(Quaternion from, Quaternion to, float dt)
    {
        Quaternion dq = to * Quaternion.Inverse(from);
        dq.ToAngleAxis(out float angDeg, out Vector3 axis);
        if (angDeg > 180f) angDeg -= 360f;
        return axis.normalized * Mathf.Deg2Rad * angDeg / Mathf.Max(dt, 0.0001f);
    }
}
