using UnityEngine;

public class WheelAnimator : MonoBehaviour
{
    [SerializeField] Transform[] wheelMeshes;
    [SerializeField] float wheelRadius = 0.32f;   // metres
    Rigidbody rb;
    float wheelCirc;

    void Awake()
    {
        rb = GetComponentInParent<Rigidbody>();   // or assign from spawner
        wheelCirc = 2f * Mathf.PI * wheelRadius;
    }

    void FixedUpdate()
    {
        float speed = Vector3.Dot(rb.linearVelocity, rb.rotation * Vector3.right);
        float delta = (speed / wheelCirc) * 360f * Time.fixedDeltaTime;
        foreach (var w in wheelMeshes) w.Rotate(0f, delta, 0f, Space.Self);
    }
}
