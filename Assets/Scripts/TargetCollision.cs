using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Detects and handles ball and target collision
/// </summary>
public class TargetCollision : MonoBehaviour {

    // The sound that plays on a successful target hit
    private AudioSource onHit;

    // The event broadcast when a target is successfully hit
    public delegate void TargetHit();

    public static TargetHit OnTargetHit;

    /// <summary>
    /// Gets the audio component of the gameobject
    /// </summary>
    public void Start()
    {
         onHit = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Plays the target hit sound and broadcasts a successful hit message if a ball collides with 
    /// the target.
    /// </summary>
    /// <param name="other"></param> The colliding object
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ball")
        {
            onHit.Play();

            if (OnTargetHit != null)
            {
                OnTargetHit();
            }
        }

    }
}