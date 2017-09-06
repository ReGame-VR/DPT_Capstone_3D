using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControl : MonoBehaviour {

    public static GameControl Instance;

    public float rightMax, leftMax, forwardMax, backwardsMax, heightMax;

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
