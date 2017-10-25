using UnityEngine;
using UnityEngine.VR;
using UnityEngine.SceneManagement;

/// <summary>
/// Houses the UI functions that change settings in GameControl or load 
/// the next scene.Disables VR for menu, then enables it when moving to 
/// the next scene.
/// </summary>
public class MenuController : MonoBehaviour {

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
                break;
            case 1:
                GameControl.Instance.label = SessionLabels.ACQUISITION;
                GameControl.Instance.difficulty = 1;
                break;
            case 2:
                GameControl.Instance.label = SessionLabels.RETENTION;
                GameControl.Instance.difficulty = 1;
                break;
            case 3:
                GameControl.Instance.label = SessionLabels.RETENTION_DISTRACTION;
                GameControl.Instance.difficulty = 1;
                break;
            case 4:
                GameControl.Instance.label = SessionLabels.TRANSFER;
                GameControl.Instance.difficulty = 2;
                break;
        }
    }

    public void StartTrials()
    {
        SceneManager.LoadScene("Calibrate");
    }

    public void SetNumTrials(string num)
    {
        GameControl.Instance.numTrials = int.Parse(num);
    }

	// Use this for initialization
	void Start () {
        VRSettings.enabled = false;
    }

    void OnDisable()
    {
        VRSettings.enabled = true;
    }

    public enum SessionLabels
    {
        BASELINE, ACQUISITION, RETENTION, RETENTION_DISTRACTION, TRANSFER
    };
}
