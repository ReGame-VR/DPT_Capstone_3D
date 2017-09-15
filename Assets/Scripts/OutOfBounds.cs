using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBounds : MonoBehaviour {

    private AudioSource miss;

    public delegate void LeaveBounds();

    public static LeaveBounds OnOutOfBounds;

    public void Start()
    {
        miss = GetComponent<AudioSource>();
    }
	
    public void OnTriggerEnter(Collider other)
    {
        miss.Play();

        if (OnOutOfBounds != null)
        {
            OnOutOfBounds();
        }
    }
}
