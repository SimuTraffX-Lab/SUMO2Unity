using UnityEngine;
using CodingConnected.TraCI.NET;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

public class SumoUnityController : MonoBehaviour
{    
    public GameObject simulatorCar;
    private List<GameObject> carlist = new List<GameObject>();

    public List<GameObject> npcVehicleList;

    public Camera mainCamera;
    public TraCIClient client;
    public float timeStep;

    [SerializeField]
    private GameObject junctions;

    [SerializeField]
    private float npcCarVisibilityDistance;
    private float squaredVisibilityDistance;

    public fpsLimits fpsLimit;


    public enum fpsLimits
    {
        noLimit = 0,
        limit30 = 30,
        limit60 = 60,
        limit90 = 90,
        limit120 = 120,
        limit240 = 240,
    }



    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = (int)fpsLimit;

        OpenSumoBackground();
        client = new TraCIClient();
        client.Connect("127.0.0.1", 4001); 
        client.Gui.TrackVehicle("View #0", "0");
        client.Gui.SetZoom("View #0", 1200); 
        client.Control.SimStep();
        client.Control.SimStep();
        client.Vehicle.SetSpeed("0", 0); 
        var shape = client.Vehicle.GetPosition("0").Content;
        var angle = client.Vehicle.GetAngle("0").Content;
        simulatorCar.transform.position = new Vector3((float)shape.X, 0.001270017f, (float)shape.Y);
        simulatorCar.transform.rotation = Quaternion.Euler(0, (float)angle, 0);
        carlist.Add(simulatorCar);

        squaredVisibilityDistance = npcCarVisibilityDistance * npcCarVisibilityDistance;
    }

    private void StopSumoBackground()
    {
        string processName = $"sumo-gui";
        Process[] processes = Process.GetProcessesByName(processName);

        if (processes.Length > 0)
        {
            foreach (Process process in processes)
            {
                process.Kill();
            }
        }
    }

   

    private void OnApplicationQuit()
    {
        client.Control.Close();
        StopSumoBackground();
    }

    void FixedUpdate()
    {
        var newvehicles = client.Simulation.GetDepartedIDList("0").Content; 
        var vehiclesleft = client.Simulation.GetArrivedIDList("0").Content; 
        for (int j = 0; j < vehiclesleft.Count; j++)
        {
            GameObject toremove = simulatorCar.transform.Find(vehiclesleft[j]).gameObject;
            if (toremove)
            {
                RemoveLeftCar(toremove);
            }
        }

        var road = client.Vehicle.GetRoadID(simulatorCar.name).Content;
        var lane = client.Vehicle.GetLaneIndex(simulatorCar.name).Content;

        // Update the position of ego car in Sumo.
        client.Vehicle.MoveToXY("0", road, lane, (double)simulatorCar.transform.position.x,
           (double)simulatorCar.transform.position.z, (double)simulatorCar.transform.eulerAngles.y, 2);

        for (int carid = 1; carid < carlist.Count; carid++)
        {
            var carpos = client.Vehicle.GetPosition(carlist[carid].name).Content;
            if (carpos != null)
            {
                carlist[carid].transform.position = new Vector3((float)carpos.X, 0f, (float)carpos.Y);
                var newangle = client.Vehicle.GetAngle(carlist[carid].name).Content;
                carlist[carid].transform.rotation = Quaternion.Euler(0f, (float)newangle, 0f);
                double carSpeed = client.Vehicle.GetSpeed(carlist[carid].name).Content;
                RotateCarWheels(FindChildRecursive(carlist[carid].transform, "Wheels"), (float)carSpeed);
            }
            else
            {
                RemoveLeftCar(carlist[carid]);
            }

        }

        for (int i = 0; i < newvehicles.Count; i++)
        {
            var newcarposition = client.Vehicle.GetPosition(newvehicles[i]).Content; 
            string carName = GetSubstringUntilCharacter(newvehicles[i], '_');
            GameObject newcar = setNPCCarPrefab(carName);
            newcar.transform.parent = simulatorCar.transform;
            newcar.transform.position = new Vector3((float)newcarposition.X, 0.0f, (float)newcarposition.Y);
            var newangle = client.Vehicle.GetAngle(newvehicles[i]).Content;
            newcar.transform.rotation = Quaternion.Euler(0f, (float)newangle, 0f);
            newcar.name = newvehicles[i];
            carlist.Add(newcar);
        }

        for (int i = 0; i < junctions.transform.childCount; i++)
        {
            GameObject currentJunction = junctions.transform.GetChild(i).gameObject;
            if (currentJunction.activeInHierarchy)
            {
                string junctionName = currentJunction.name;
                var currentphase = client.TrafficLight.GetCurrentPhase(junctionName);
                ChangeTrafficStatus(junctionName, currentphase.Content);
            }
            else
            {
                UnityEngine.Debug.Log($"junction {currentJunction.name} is inactive. Activate it in the heirarchy to display traffic signals");
            }

        }
        client.Control.SimStep();
    }

    private void RemoveLeftCar(GameObject toremove)
    {
        carlist.Remove(toremove);
        Destroy(toremove);
    }

    private void ChangeTrafficStatus(string junctionID, int state)
    {
        Dictionary<int, char> trafficLightColor = new Dictionary<int, char>();
        GameObject newjunction = junctions.transform.Find(junctionID).gameObject;

        string newstate = client.TrafficLight.GetState(junctionID).Content;
        int i = 0;
        foreach (char c in newstate)
        {
            trafficLightColor[i] = char.ToLower(c);
            i++;
        }
        for (i = 0; i < newjunction.transform.childCount; i++)
        {
            GameObject childLight = newjunction.transform.GetChild(i).gameObject;
            int index = int.Parse(childLight.name);
            SetSignalState(trafficLightColor[index], childLight);
        }
    }

    void SetSignalState(char state, GameObject curTrafficSignal)
    {
        GameObject GreenOn = FindChildRecursive(curTrafficSignal.transform, "Light_Green");
        GameObject RedOn = FindChildRecursive(curTrafficSignal.transform, "Light_Red");
        GameObject YellowOn = FindChildRecursive(curTrafficSignal.transform, "Light_Orange");

        switch (state)
        {
            case 'r':
                RedOn.SetActive(true);
                YellowOn.SetActive(false);
                GreenOn.SetActive(false);
                break;

            case 'y':
                RedOn.SetActive(false);
                YellowOn.SetActive(true);
                GreenOn.SetActive(false);

                break;

            case 'g':
                RedOn.SetActive(false);
                YellowOn.SetActive(false);
                GreenOn.SetActive(true);

                break;
        }
    }

    private string GetSubstringUntilCharacter(string inputString, char delimiter)
    {
        int index = inputString.IndexOf(delimiter);
        if (index == -1)
        {
            return inputString;
        }
        return inputString.Substring(0, index);
    }

    private GameObject FindChildRecursive(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
            {
                return child.gameObject;
            }
            GameObject result = FindChildRecursive(child, childName);
            if (result != null)
            {
                return result;
            }
        }
        return null;
    }

    private void OpenSumoBackground()
    {
        string folderPath = $"{Application.dataPath}/_Project/Sumo_Data";
        string command = $"cd /d {folderPath} && sumo-gui -c SUMO2UNITY.sumocfg --start --remote-port 4001 --step-length {timeStep}";

        Process process = new Process();
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = "cmd.exe"; // Specify the command prompt executable
        startInfo.Arguments = "/c " + command;
        startInfo.RedirectStandardOutput = true;
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true;

        process.StartInfo = startInfo;
        process.Start();

        process.Close();
        Thread.Sleep(3000);
    }

    private GameObject setNPCCarPrefab(string carName)
    {
        GameObject newcar = null;
        switch (carName)
        {
            case "AlmaBlue":
                newcar = (GameObject)Instantiate(npcVehicleList[0]); 
                break;
            case "AlmaGrey":
                newcar = (GameObject)Instantiate(npcVehicleList[1]); 
                break;
            case "ElkaGrey":
                newcar = (GameObject)Instantiate(npcVehicleList[2]); 
                break;
            case "ElkaRed":
                newcar = (GameObject)Instantiate(npcVehicleList[3]);
                break;
            case "EloraBlue":
                newcar = (GameObject)Instantiate(npcVehicleList[4]); 
                break;
            case "EloraWhite":
                newcar = (GameObject)Instantiate(npcVehicleList[5]); 
                break;
            default:
                newcar = (GameObject)Instantiate(npcVehicleList[0]);
                break;
        }
        return newcar;
    }

    private void RotateCarWheels(GameObject wheels, float speed)
    {
        int wheelCount = wheels.transform.childCount;
        Transform[] wheelTransforms = new Transform[wheelCount];
        for (int i=0; i < wheelCount; i++)
        {
            wheelTransforms[i] = wheels.transform.GetChild(i);
            Transform wheelTransform = wheelTransforms[i];
            if (wheelTransform.name.ToLower() != "body")
            {
                wheelTransform.Rotate(Vector3.right * (speed));
            }
        }
    }

    //checks if npc cars are farther away from ego car.
    private bool isNpcCarFar(Vector3 npcCarPosition)
    {
        bool carIsFar = false;
        float distanceSqr = Vector3.SqrMagnitude(simulatorCar.transform.position - npcCarPosition);
        if (distanceSqr > squaredVisibilityDistance)
        {
            carIsFar = true;
        }
        return carIsFar;
    }
}

