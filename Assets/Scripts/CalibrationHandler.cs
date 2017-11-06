using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// A class for handling user reach calibration. It currently tracks left and right reach,
/// height reach, and forward reach. It should be attached to the controller(s) in the
/// SteamVR prefab in the CALIBRATION scene.
/// </summary>
public class CalibrationHandler : MonoBehaviour {

    // bounds that player can reach
    private float leftCal, rightCal, heightCal, forwardCal;

    // the controller
    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    /// <summary>
    /// Called when controllers are enabled. Initializes all fields.
    /// </summary>
    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        leftCal = 0f;
        rightCal = 0f;
        heightCal = 0f;
        forwardCal = 0f;
    }
	
	/// <summary>
    /// Called once per frame. Tracks the controller position, and updates calibration 
    /// parameters if the position sets a new maximum for any of them. Also loads next
    /// scene when the spacebar is hit.
    /// </summary>
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

        // reach calibration
        if (posn.z > forwardCal)
        {
            forwardCal = posn.z;
        }

        // move onto next scene if spacebar is hit
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SaveData();
            SceneManager.LoadScene("environment1");
        }
    }

    /// <summary>
    /// Saves all calibration info to the Game Controller singleton.
    /// </summary>
    private void SaveData()
    {
        GameControl.Instance.leftMax = leftCal;
        GameControl.Instance.rightMax = rightCal;
        GameControl.Instance.heightMax = heightCal;
        GameControl.Instance.reachMax = forwardCal;
    }
}
