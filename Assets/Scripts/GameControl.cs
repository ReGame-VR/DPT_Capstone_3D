using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A singleton class that stores global information on the game such as difficulty level,
/// calibration parameters, and participant ID. 
/// </summary>
public class GameControl : MonoBehaviour {
    // The single instance of this class
    public static GameControl Instance;

    // max distance the user can reach to the right
    public float rightMax = 1f;

    // max distance the user can reach to the left
    public float leftMax = 1f;

    // max distance the user can reach upwards
    public float heightMax = 1.5f;

    // max distance the user can reach straight forward
    public float reachMax = 1f;

    // the difficulty of the trial - tied to session label
    public int difficulty = 0;

    // the total number of trials in the session - determined by session label EXCEPT for AQU 2
    public int numTrials = 10;

    // the participant ID - entered in the menu
    public string participantID = "default";
    public MenuController.SessionLabels label = MenuController.SessionLabels.BASELINE;

    /// <summary>
    /// If there is no Instance, assign this object to it and don't destroy when new scene loads.
    /// If there is already an Instance, destroy this object because there can only be one.
    /// </summary>
	void Awake () {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }

        else if (Instance != this)
        {
            Destroy(gameObject);
        }
	}
	
}
