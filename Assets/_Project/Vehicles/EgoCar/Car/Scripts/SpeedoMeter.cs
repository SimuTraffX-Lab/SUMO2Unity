using UnityEngine;

using UnityEngine.UI; // Required for working with Unity UI Text


public class Speedometer : MonoBehaviour
{
    public GameObject TrafficObject;           // Assign via Inspector.
    public float updateInterval = 0.1f;          // Interval in seconds at which to update speed.

    private Rigidbody rb;
    private Text m_text;
    private float timeSinceLastUpdate = 0f;
    private float m_Speed = 0f;

    void Start()
    {
        rb = TrafficObject.GetComponent<Rigidbody>();
        m_text = GetComponentInChildren<Text>();

        if (m_text == null)
        {
            Debug.LogWarning("Text component not found in children.");
        }
    }

    void Update()
    {
        // Accumulate time
        timeSinceLastUpdate += Time.deltaTime;

        // Check if it�s time to update the speed
        if (timeSinceLastUpdate >= updateInterval)
        {
            // Calculate speed in km/h (example: velocity.magnitude is m/s, multiply by 3.6 to get km/h)
            m_Speed = Mathf.Round(rb.linearVelocity.magnitude * 3.6f);

            // Update the text if reference is available
            if (m_text != null)
            {
                // Format to always show two digits, e.g. "05", "10"
                m_text.text = string.Format("{0:00} km/h", m_Speed);
            }

            // Reset the timer
            timeSinceLastUpdate = 0f;
        }
    }
}
