using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CalibrationHandler : MonoBehaviour {

    public Text text;

    // bounds that player can reach
    private float forwardCal, backwardsCal, leftCal, rightCal, heightCal;

    // the controller
    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        forwardCal = 0f;
        backwardsCal = 0f;
        leftCal = 0f;
        rightCal = 0f;
        heightCal = 0f;
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 posn = gameObject.transform.position;

        // front / back calibration
        if (posn.z > forwardCal)
        {
            forwardCal = posn.z;
        }
        else if (posn.z < backwardsCal)
        {
            backwardsCal = posn.z;
        }
        
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

        text.text = "max forward: " + forwardCal
            + "\nmax backward: " + backwardsCal
            + "\nmax left: " + leftCal
            + "\nmax right: " + rightCal
            + "\nmax height: " + heightCal;

        // if properly calibrated
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SaveData();
            SceneManager.LoadScene("catchthrow");
        }
    }

    private void SaveData()
    {
        GameControl.Instance.forwardMax = forwardCal;
        GameControl.Instance.backwardsMax = backwardsCal;
        GameControl.Instance.leftMax = leftCal;
        GameControl.Instance.rightMax = rightCal;
        GameControl.Instance.heightMax = heightCal;
    }
}
