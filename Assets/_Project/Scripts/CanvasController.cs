using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    // Start is called before the first frame update
    private RecorderController recorderController;

    public TMP_Text recorderButtonText;
    void Start()
    {
        recorderController = FindAnyObjectByType<RecorderController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RecorderButtonClick()
    {
        recorderController.recordingOn = !recorderController.recordingOn;
        if (recorderController.recordingOn)
        {
            recorderController.StartRecording();
            recorderButtonText.text = "Stop Rec";
        }
        else
        {
            recorderController.StopRecording();
            recorderButtonText.text = "Start Rec";
        }
        
        
    }
}
