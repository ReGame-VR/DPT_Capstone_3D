using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Destroys balls when they go out of bounds, and sends an event to begin a new trial.
/// </summary>
public class OutOfBounds : MonoBehaviour {

    private AudioSource miss;

    public delegate void LeaveBounds();

    public static LeaveBounds OnOutOfBounds;

    public void Start()
    {
        miss = GetComponent<AudioSource>();
    }
	
    public void OnTriggerExit(Collider other)
    {
        miss.Play();

        if (OnOutOfBounds != null)
        {
            OnOutOfBounds();
        }
    }
}
