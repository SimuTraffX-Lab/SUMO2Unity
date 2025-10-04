// FreeFlyCamera.cs

using UnityEngine;

public class FreeFlyCamera : MonoBehaviour
{
    public float moveSpeed = 50.0f;
    public float sprintSpeed = 150.0f;
    public float lookSpeed = 2.0f;

    private float rotationX = 0.0f;
    private float rotationY = 0.0f;

    void Update()
    {
        // --- Mouselook ---
        // Only look around when the right mouse button is held down.
        if (Input.GetMouseButton(1))
        {
            rotationX += Input.GetAxis("Mouse X") * lookSpeed;
            rotationY -= Input.GetAxis("Mouse Y") * lookSpeed;
            rotationY = Mathf.Clamp(rotationY, -90, 90); // Clamp vertical rotation

            transform.localRotation = Quaternion.Euler(rotationY, rotationX, 0);
        }

        // --- Movement ---
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;

        // Forward / Backward
        transform.position += transform.forward * Input.GetAxis("Vertical") * currentSpeed * Time.deltaTime;
        // Left / Right
        transform.position += transform.right * Input.GetAxis("Horizontal") * currentSpeed * Time.deltaTime;

        // Up / Down
        if (Input.GetKey(KeyCode.E))
        {
            transform.position += transform.up * currentSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            transform.position -= transform.up * currentSpeed * Time.deltaTime;
        }
    }
}