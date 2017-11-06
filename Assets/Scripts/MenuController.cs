﻿using UnityEngine;
using UnityEngine.VR;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Houses the UI functions that change settings in GameControl or load 
/// the next scene.Disables VR for menu, then enables it when moving to 
/// the next scene.
/// </summary>
public class MenuController : MonoBehaviour {

    public GameObject acqu2Slider;
    public Text sliderText;

    void Awake()
    {
        acqu2Slider.SetActive(false);
    }

    public void SetParticipantID(string id)
    {
        GameControl.Instance.participantID = id;
    }

    public void SetDifficulty(int diff)
    {
        GameControl.Instance.difficulty = diff;
    }

    public void SetSessionType(int type)
    {
        //GameControl.Instance.difficulty = type;

        switch (type)
        {
            case 0:
                GameControl.Instance.label = SessionLabels.BASELINE;
                GameControl.Instance.difficulty = 0;
                GameControl.Instance.numTrials = 10;
                break;
            case 1:
                GameControl.Instance.label = SessionLabels.ACQUISITION1;
                GameControl.Instance.difficulty = 1;
                GameControl.Instance.numTrials = 200;
                break;
            case 2:
                GameControl.Instance.label = SessionLabels.ACQUISITION2;
                acqu2Slider.SetActive(true);
                GameControl.Instance.difficulty = 1;
                break;
            case 3:
                GameControl.Instance.label = SessionLabels.RETENTION;
                GameControl.Instance.difficulty = 1;
                GameControl.Instance.numTrials = 20;
                break;
            case 4:
                GameControl.Instance.label = SessionLabels.RETENTION_DISTRACTION;
                GameControl.Instance.difficulty = 1;
                GameControl.Instance.numTrials = 20;
                break;
            case 5:
                GameControl.Instance.label = SessionLabels.TRANSFER;
                GameControl.Instance.difficulty = 2;
                GameControl.Instance.numTrials = 20;
                break;
        }
    }

    public void SetNumTrials(float i)
    {
        GameControl.Instance.numTrials = (int)i;
        sliderText.text = i.ToString();
    }

    public void StartTrials()
    {

        // Debug.Log(GameControl.Instance.numTrials);
        SceneManager.LoadScene("Calibrate");
    }

	// Use this for initialization
	void Start () {
        UnityEngine.XR.XRSettings.enabled = false;
    }

    void OnDisable()
    {
        UnityEngine.XR.XRSettings.enabled = true;
    }

    public enum SessionLabels
    {
        BASELINE, ACQUISITION1, ACQUISITION2, RETENTION, RETENTION_DISTRACTION, TRANSFER
    };
}
