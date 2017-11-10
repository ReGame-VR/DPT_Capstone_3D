using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Destroys balls when they go out of bounds, and sends an event to begin a new trial.
/// </summary>
public class OutOfBounds : MonoBehaviour {

    // Sound that plays when the ball goes out of bounds
    private AudioSource miss;

    private bool caught = false;

    private bool thrown = false;

    // an event that plays when the ball leaves the out of bounds collider.
    public delegate void LeaveBounds();

    public static LeaveBounds OnOutOfBounds;

    /// <summary>
    /// Get the audio source from the GameObject.
    /// </summary>
    public void Awake()
    {
        ControllerHandler.OnBallGrab += WasCaught;
        ControllerHandler.OnBallRelease += WasThrown;
        UIController.RecordData += Reset;
        miss = GetComponent<AudioSource>();
    }

    public void OnDisable()
    {
        ControllerHandler.OnBallGrab -= WasCaught;

        UIController.RecordData -= Reset;
    }
	
    /// <summary>
    /// Play the miss sound and broadcast the out of bounds event when the ball leaves bounds.
    /// </summary>
    /// <param name="other"></param> The collider of the ball leaving bounds.
    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Ball")) {
            if (!caught || (caught && thrown)) {
                miss.Play();

                if (OnOutOfBounds != null)
                {
                    OnOutOfBounds();
                }
            }
        }
    }
    /// <summary>
    /// The ball was caught, don't destroy on out of bounds/
    /// </summary>
    private void WasCaught()
    {
        caught = true;
    }

    private void WasThrown()
    {
        thrown = true;
    }

    /// <summary>
    /// Reset on new trial. None of the params in this message matter to this class, only that
    /// a new trial started.
    /// </summary>
    /// <param name="trialNum"></param>
    /// <param name="catchTime"></param>
    /// <param name="throwTime"></param>
    /// <param name="wasCaught"></param>
    /// <param name="wasThrown"></param>
    /// <param name="hitTarget"></param>
    /// <param name="score"></param>
    private void Reset(int trialNum, float catchTime, float throwTime, bool wasCaught, 
        bool wasThrown, bool hitTarget, int score)
    {
        caught = false;
        thrown = true;
    }
}
