using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecorderController : MonoBehaviour
{
    // Start is called before the first frame update
    private SumoUnityController sumoUnity;

    public bool recordingOn = false;
    

    #region Data Calculation Variable
    private float currentThrottle = 0;
    private float acceleration = 5f;

    public float maxSteeringAngle = 45f;
    public float steeringSpeed = 5f;
    private float currentSteeringInput = 0f;

    public float maxBrakeInput = 1f;
    public float brakeSensitivity = 5f;
    private float currentBrakeInput = 0f;
    #endregion

    private RenderTexture renderTexture;

    private float imageCaptureInterval = 1f;
    private float dataCaptureInterval = 0.1f;
    private int imageCounter = 0;

    private Queue<Texture2D> imageTextureData = new Queue<Texture2D>();
    int jpegQuality = 25;

    private string pathToSaveDataImage;


    void Start()
    {
        sumoUnity = FindAnyObjectByType<SumoUnityController>();
    }

    private string GetDataPath(string timeStamp)
    {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SumoUnity", timeStamp);
    }

    //Starts recording data and images. Images are taken but not saved when recording to improve performance.
    public void StartRecording()
    {
        pathToSaveDataImage = GetDataPath(DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss tt"));
        if (!Directory.Exists(pathToSaveDataImage))
        {
            Directory.CreateDirectory(pathToSaveDataImage);
        }
        StreamWriter outStream = File.CreateText($"{pathToSaveDataImage}\\data.txt"); 
        string line = "Timestamp,                      Speed(KPH),    Throttle,     Steering Input      Brake";
        outStream.WriteLine(line);
        outStream.Close();
        InvokeRepeating("ImageCaptureProcess", 0, imageCaptureInterval);
        InvokeRepeating("CalculateData", 0, dataCaptureInterval);
       
    }

    // Saving pictures is done after stopping recording to improve performance
    public void StopRecording()
    {
        StartCoroutine(SavePicturesCoroutine());
        CancelInvoke("ImageCaptureProcess");
        CancelInvoke("CalculateData");
    }


    private void CalculateData()
    {
        float egoCarSpeedKMH = sumoUnity.simulatorCar.GetComponent<Rigidbody>().velocity.magnitude * 3.6f;

        float verticalInput = Input.GetAxis("Vertical");
        currentThrottle = Mathf.Clamp01(currentThrottle + verticalInput * acceleration * Time.deltaTime);

       
        float horizontalInput = Input.GetAxis("Horizontal");
        currentSteeringInput = Mathf.Clamp(horizontalInput, -1f, 1f);
        currentSteeringInput *= steeringSpeed;
        currentSteeringInput = Mathf.Clamp(currentSteeringInput, -maxSteeringAngle, maxSteeringAngle);
        
        currentBrakeInput = Mathf.Clamp01(-verticalInput);
        currentBrakeInput *= brakeSensitivity;

        string line = $"{DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss-fff tt")}        {egoCarSpeedKMH.ToString("F2")}          {currentThrottle.ToString("F2")}             {currentSteeringInput.ToString("F2")}            {currentBrakeInput.ToString("F2")}";
        AddNewLine(line);

    }

    private void ImageCaptureProcess()
    {
        renderTexture = new RenderTexture(sumoUnity.mainCamera.pixelWidth, sumoUnity.mainCamera.pixelHeight, 24);
        sumoUnity.mainCamera.targetTexture = renderTexture;
        sumoUnity.mainCamera.Render();

        Texture2D screenshot = new Texture2D(sumoUnity.mainCamera.pixelWidth, sumoUnity.mainCamera.pixelHeight, TextureFormat.RGB24, false);
        RenderTexture.active = renderTexture;
        screenshot.ReadPixels(new Rect(0, 0, sumoUnity.mainCamera.pixelWidth, sumoUnity.mainCamera.pixelHeight), 0, 0);
        screenshot.Apply();

        sumoUnity.mainCamera.targetTexture = null;
        RenderTexture.active = null;

        Destroy(renderTexture);
        imageTextureData.Enqueue(screenshot);
    }

    public IEnumerator SavePicturesCoroutine()
    {
        while(imageTextureData.Count != 0)
        {
            string fileName = $"{pathToSaveDataImage}\\screenshot_{imageCounter}.png";
            Texture2D currentImageTex = imageTextureData.Dequeue();
            byte[] imageBytes = currentImageTex.EncodeToJPG(jpegQuality);
            File.WriteAllBytes(fileName, imageBytes);
            imageCounter++;
            yield return null;
        }
        
    }

    public void AddNewLine(string line)
    {
        StreamWriter outStream = File.AppendText($"{pathToSaveDataImage}\\data.txt");
        outStream.WriteLine(line);
        outStream.Close();
    }
}
