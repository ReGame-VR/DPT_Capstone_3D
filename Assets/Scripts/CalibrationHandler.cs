using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CalibrationHandler : MonoBehaviour {

    // bounds that player can reach
    private float leftCal, rightCal, heightCal;

    // the controller
    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        leftCal = 0f;
        rightCal = 0f;
        heightCal = 0f;
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 posn = gameObject.transform.position;
        
        // side to side calibration
        if (posn.x > rightCal)
        {
            rightCal = posn.x;
        }
        else if (posn.x < leftCal)
        {
            leftCal = posn.x;
        }

        // height calibration
        if (posn.y > heightCal)
        {
            heightCal = posn.y;
        }

        // if properly calibrated
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SaveData();
            SceneManager.LoadScene("environment1");
        }
    }

    private void SaveData()
    {
        GameControl.Instance.leftMax = leftCal;
        GameControl.Instance.rightMax = rightCal;
        GameControl.Instance.heightMax = heightCal;

        Debug.Log("left cal: " + leftCal);
        Debug.Log("Right cal: " + rightCal);
        Debug.Log("Height cal: " + heightCal);
    }
}
