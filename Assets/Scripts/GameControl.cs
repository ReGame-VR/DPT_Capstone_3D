using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControl : MonoBehaviour {

    public static GameControl Instance;

    public float rightMax = 1f;
    public float leftMax = 1f;
    public float heightMax = 1.5f;
    public float reachMax = 1f;
    public int difficulty = 0;
    public int numTrials = 10;
    public string participantID = "default";
    public MenuController.SessionLabels label = MenuController.SessionLabels.ACQUISITION;

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
