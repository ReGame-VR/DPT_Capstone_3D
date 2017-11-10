using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Creates the trigger collider that changes the color of the ball when in reach.
/// </summary>
public class ReachCollider : MonoBehaviour {

    // The red material to texture the ball when it is not within reach.
    public Material outOfBounds;

    // The green material to texture the ball when it is within reach.
    public Material inBounds;

    // The SteamVR camera rig prefab
    public GameObject cameraRig;

    // The box collider marking user reach
    private BoxCollider bc;

    // has the ball been caught?
    private bool caught; // once ball is caught, stay green

    // broadcast an event when the ball is within reach
    public delegate void InReach();

    public static InReach IsInReach;

    // broadcast an event when the ball leaves in reach bounds
    public delegate void OutOfReach();

    public static OutOfReach IsOutOfReach;

	/// <summary>
    /// Subscribe to ball grab event and resize the box collider to encompass user reach
    /// </summary>
	void Awake () {
        ControllerHandler.OnBallGrab += wasCaught;

        bc = gameObject.GetComponent<BoxCollider>();

        bc.center = new Vector3(cameraRig.transform.position.x , 
            cameraRig.transform.position.y + GameControl.Instance.heightMax / 2, 
            cameraRig.transform.position.z);
        bc.size = new Vector3(System.Math.Abs(GameControl.Instance.leftMax) 
            + GameControl.Instance.rightMax + 0.2f, GameControl.Instance.heightMax + 0.1f,
            GameControl.Instance.reachMax * 2 + 0.2f);

	}

    /// <summary>
    /// Unsubscribe to ball grab event.
    /// </summary>
    void OnDisable()
    {
        ControllerHandler.OnBallGrab -= wasCaught;
    }

    /// <summary>
    /// If a ball enters the collider, call the ToInBounds function.
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            ToInBounds(other.gameObject);
        }
    }

    /// <summary>
    /// If a ball stays in the collider, call the ToInBounds function.
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            ToInBounds(other.gameObject);
        }
    }

    /// <summary>
    /// If a ball leaves the collider, call the ToOutOfBounds function.
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            ToOutofBounds(other.gameObject);
        }
    }

    /// <summary>
    /// Change the ball material to the in bounds material and broadcast the in bounds event.
    /// </summary>
    /// <param name="ball"></param> The ball in bounds
    private void ToInBounds(GameObject ball)
    {
        ball.GetComponent<Renderer>().material = inBounds;
        if (IsInReach != null)
        {
            IsInReach();
        }
    }

    /// <summary>
    /// Change the ball material to the out of bounds material if the ball has not been caught 
    /// and broadcast the out of bounds event.
    /// </summary>
    /// <param name="ball"></param>
    private void ToOutofBounds(GameObject ball)
    {
        if (!caught)
        {
            ball.GetComponent<Renderer>().material = outOfBounds;
        }
        if (IsOutOfReach != null)
        {
            IsOutOfReach();
        }
    }

    // Upon recieving ball caught event, change caught to true so material will no longer change on out of bounds.
    private void wasCaught()
    {
        caught = true;
    }
}
