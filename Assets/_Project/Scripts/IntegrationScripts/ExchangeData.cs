using AsyncIO;
using NetMQ;
using NetMQ.Sockets;
using System.Threading;
using UnityEngine;
using System;

[System.Serializable]
public class CommonMessage
{
    public string type;    // "command" or "vehicles"
    public string command; // Used if type == "command"
}

public static class RecordingManager
{
    public static bool startRecordingFromZero = false;
    public static float recordingStartTime = 0f;
}

public class ExchangeData : MonoBehaviour
{
    private SimulationController _SimulationController;

    // Thread for background communication
    private Thread _communicationThread;
    private bool _isRunning = false;


    public void Start()
    {
        _SimulationController = GetComponent<SimulationController>();

        // Start the communication thread
        _isRunning = true;
        _communicationThread = new Thread(Run);
        _communicationThread.Start();
    }

    void OnDestroy()
    {
        // Stop the communication thread
        _isRunning = false;
        if (_communicationThread != null && _communicationThread.IsAlive)
        {
            _communicationThread.Join();
        }

        NetMQConfig.Cleanup();
        Debug.Log("ExchangeData thread terminated gracefully.");
    }

    private void Run()
    {
        ForceDotNet.Force();

        try
        {
            using (var subSocket = new SubscriberSocket())
            using (var dealerSocket = new DealerSocket())
            {
                // Connect to SUMO's PUB socket
                subSocket.Connect("tcp://localhost:5556");
                subSocket.Subscribe("");
                subSocket.Options.ReceiveHighWatermark = 1000;

                // Connect to SUMO's ROUTER socket
                dealerSocket.Connect("tcp://localhost:5557");
                dealerSocket.Options.SendHighWatermark = 1000;

                while (_isRunning)
                {
                    try
                    {
                        // --- Send Data to SUMO ---
                        string vehicleDataJson = _SimulationController.GetVehicleDataJson();

                        bool sendSuccess = dealerSocket.TrySendFrame(vehicleDataJson);
                        if (!sendSuccess)
                        {
                            Debug.LogError("Failed to send data to SUMO.");
                            _isRunning = false; // Gracefully stop the thread
                            break;
                        }

                        // --- Receive Data from SUMO ---
                        string sumoDataJson;
                        bool gotMessage = subSocket.TryReceiveFrameString(out sumoDataJson);

                        int messageCount = 0;
                        float lastLogTime = 0f;

                        if (gotMessage)
                        {

                            // Enqueue the message to be handled on the main thread
                            _SimulationController.EnqueueOnMainThread(sumoDataJson);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Exception in background thread loop: {ex.Message}\n{ex.StackTrace}");
                        _isRunning = false;
                        break;
                    }

                    // Sleep briefly to prevent 100% CPU usage
                    Thread.Sleep(1);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Exception in background thread: {ex.Message}\n{ex.StackTrace}");
        }
        finally
        {
            NetMQConfig.Cleanup();
            Debug.Log("ExchangeData thread terminated gracefully.");
        }
    }
}
