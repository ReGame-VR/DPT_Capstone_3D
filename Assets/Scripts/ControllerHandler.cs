using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles catching and throwing balls (without trigger requirements). Should be placed on
/// the controller(s) in the SteamVR prefab in the TASK scene.
/// </summary>
public class ControllerHandler : MonoBehaviour {
    // the velocity threshold for throwing - increase to require harder throws to detatch ball
    private float threshold = 3.5f;

    // the sound played when the ball is caught
    private AudioSource caughtSound;
    
    // the ball currently colliding with the controller trigger
    private GameObject collidingObject;

    // should the user be able to catch the ball at this time?
    private bool catchable;

    // the controller rigidbody
    private Rigidbody rb;

    // broadcast an event every time a ball is grabbed
    public delegate void BallGrabbed();

    public static BallGrabbed OnBallGrab;

    // broadcast an event every time a ball is released
    public delegate void BallReleased();

    public static BallReleased OnBallRelease;

    // the controller
    private SteamVR_TrackedObject trackedObj;

    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    /// <summary>
    /// Called when controllers are enabled. Subscribes class to the data recording event so it
    /// knows when a new ball is spawned and initializes all fields.
    /// </summary>
    void Awake()
    {
        UIController.RecordData += newBall;

        rb = GetComponent<Rigidbody>();
        rb.detectCollisions = true;

        trackedObj = GetComponent<SteamVR_TrackedObject>();
        caughtSound = GetComponent<AudioSource>();

        catchable = true;
    }

    /// <summary>
    /// Called when controllers are disabled (when the game is stopped). Unsubscribes to events.
    /// </summary>
    void OnDisable()
    {
        UIController.RecordData -= newBall;
    }

    /// <summary>
    /// Changes catchable to true because a new ball was spawned.
    /// </summary>
    /// <param name="trialNum"></param>
    /// <param name="catchTime"></param>
    /// <param name="throwTime"></param>
    /// <param name="wasCaught"></param>
    /// <param name="wasThrown"></param>
    /// <param name="hitTarget"></param>
    /// <param name="score"></param>
    private void newBall(int trialNum, float catchTime,
        float throwTime, bool wasCaught, bool wasThrown, bool hitTarget, int score)
    {
        catchable = true;
    }

    /// <summary>
    /// When a ball enters the trigger collider, check to make sure there isn't already an
    /// object colliding before assigning the collider to collidingObject.
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerEnter(Collider other)
    {
        if (collidingObject || !other.GetComponent<Rigidbody>())
        {
            return;
        }

        collidingObject = other.gameObject;
        GrabObject();
    }

    public void OnTriggerStay(Collider other)
    {
        if (collidingObject || !other.GetComponent<Rigidbody>())
        {
            return;
        }

        collidingObject = other.gameObject;
        GrabObject();
    }

    public void OnTriggerExit(Collider other)
    {
        if (!collidingObject)
        {
            return;
        }

        // collidingObject = null;
        catchable = true;
    }

    private void GrabObject()
    {
        if (catchable) {
            caughtSound.Play();

            if (OnBallGrab != null)
            {
                OnBallGrab();
            }

            // objectInHand = collidingObject;
            // collidingObject = null;

            var joint = AddFixedJoint();
            joint.connectedBody = collidingObject.GetComponent<Rigidbody>();
            catchable = false;
        }
    }

    private FixedJoint AddFixedJoint()
    {
        FixedJoint fx = gameObject.AddComponent<FixedJoint>();
        fx.breakForce = 20000;
        fx.breakTorque = 20000;
        return fx;
    }

    private void ReleaseObject()
    {
        if (OnBallRelease != null)
        {
            OnBallRelease();
        }

        if (GetComponent<FixedJoint>())
        {
            GetComponent<FixedJoint>().connectedBody = null;
            Destroy(GetComponent<FixedJoint>());

            collidingObject.GetComponent<Rigidbody>().velocity = Controller.velocity;
            collidingObject.GetComponent<Rigidbody>().angularVelocity = Controller.angularVelocity;
        }

        collidingObject = null;
    }

    // Update is called once per frame
    void Update () {

        if (Controller.velocity.magnitude >= threshold)
        {
            if (collidingObject)
            {
                ReleaseObject();
            }
        }
    }
}
