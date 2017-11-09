using UnityEngine;
using UnityEngine.VR;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Houses the UI functions that change settings in GameControl or load the next scene. Disables
/// VR for menu, then enables it when moving to the next scene.
/// </summary>
public class MenuController : MonoBehaviour {

    // the UI slider to choose number of trial repetitions in the Aquisition 2 level
    public GameObject acqu2Slider;
    // the text for the UI slider
    public Text sliderText;

    /// <summary>
    /// Sets the slider to inactive.
    /// </summary>
    void Awake()
    {
        acqu2Slider.SetActive(false);
    }

    /// <summary>
    /// Upon pressing enter after typing in the text box, the participant ID is recorded in 
    /// GameControl.
    /// </summary>
    /// <param name="id"></param> the ID entered
    public void SetParticipantID(string id)
    {
        GameControl.Instance.participantID = id;
    }

    /// <summary>
    /// Sets session label in GameControl when an option is chosen from the dropdown. Also
    /// assigns difficulty and number of trials except for ACQUISITION 2, which is chosen by 
    /// the user and put in by operator.
    /// </summary>
    /// <param name="type"></param> the index of the chosen option
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

    /// <summary>
    /// Sets the number of trials from 0 to 100.
    /// </summary>
    /// <param name="i"></param>
    public void SetNumTrials(float i)
    {
        GameControl.Instance.numTrials = (int)i;
        sliderText.text = i.ToString();
    }

    /// <summary>
    /// Loads the next scene.
    /// </summary>
    public void StartTrials()
    {
        SceneManager.LoadScene("Calibrate");
    }

	/// <summary>
    /// Disable VR for menu scene
    /// </summary>
	void Start () {
        UnityEngine.XR.XRSettings.enabled = false;
    }

    /// <summary>
    /// Enable VR before moving to next scene.
    /// </summary>
    void OnDisable()
    {
        UnityEngine.XR.XRSettings.enabled = true;
    }

    /// <summary>
    /// An enumeration of the possible session labels.
    /// </summary>
    public enum SessionLabels
    {
        BASELINE, ACQUISITION1, ACQUISITION2, RETENTION, RETENTION_DISTRACTION, TRANSFER
    };
}
