using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class makeTargets : MonoBehaviour {
    public GameObject newTarget;

    public delegate void TargetHit();

    public static TargetHit OnTargetHit;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnTriggerEnter(Collider other)
    {
        if (OnTargetHit != null)
        {
            OnTargetHit();
        }
    }
}
