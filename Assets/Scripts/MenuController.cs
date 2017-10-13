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
        switch (type)
        {
            case 0:
                GameControl.Instance.label = SessionLabels.ACQUISITION;
                break;
            case 1:
                GameControl.Instance.label = SessionLabels.RETENTION;
                break;
            case 2:
                GameControl.Instance.label = SessionLabels.RETENTION_DISTRACTION;
                break;
            case 3:
                GameControl.Instance.label = SessionLabels.TRANSFER;
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
        ACQUISITION, RETENTION, RETENTION_DISTRACTION, TRANSFER
    };
}
