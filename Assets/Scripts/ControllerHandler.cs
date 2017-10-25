using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerHandler : MonoBehaviour {
    // the velocity threshold for throwing
    private float threshold = 5f;

    // play a sound when caught
    private AudioSource caughtSound;
    
    // the ball that the user can grab
    private GameObject collidingObject;

    // can you catch the ball?
    private bool catchable;

    // broadcast an event every time a ball is grabbed
    public delegate void BallGrabbed();

    public static BallGrabbed OnBallGrab;

    // broadcast an event every time a ball is let go of
    public delegate void BallReleased();

    public static BallReleased OnBallRelease;

    // the controller
    private SteamVR_TrackedObject trackedObj;

    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake()
    {
        UIController.RecordData += newBall;

        trackedObj = GetComponent<SteamVR_TrackedObject>();
        caughtSound = GetComponent<AudioSource>();

        catchable = true;
    }

    void OnDisable()
    {
        UIController.RecordData -= newBall;
    }

    private void newBall(int trialNum, float catchTime,
        float throwTime, bool wasCaught, bool wasThrown, bool hitTarget)
    {
        catchable = true;
    }


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
