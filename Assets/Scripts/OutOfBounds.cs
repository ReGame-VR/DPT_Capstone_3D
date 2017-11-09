using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Destroys balls when they go out of bounds, and sends an event to begin a new trial.
/// </summary>
public class OutOfBounds : MonoBehaviour {

    // Sound that plays when the ball goes out of bounds
    private AudioSource miss;

    // an event that plays when the ball leaves the out of bounds collider.
    public delegate void LeaveBounds();

    public static LeaveBounds OnOutOfBounds;

    /// <summary>
    /// Get the audio source from the GameObject.
    /// </summary>
    public void Start()
    {
        miss = GetComponent<AudioSource>();
    }
	
    /// <summary>
    /// Play the miss sound and broadcast the out of bounds event when the ball leaves bounds.
    /// </summary>
    /// <param name="other"></param> The collider of the ball leaving bounds.
    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Ball")) {
            miss.Play();

            if (OnOutOfBounds != null)
            {
                OnOutOfBounds();
            }
        }
    }
}
