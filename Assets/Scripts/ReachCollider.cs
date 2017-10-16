using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Creates the trigger collider that changes the color of the ball when in reach.
/// </summary>
public class ReachCollider : MonoBehaviour {

    public Material outOfBounds;

    public Material inBounds;

    public GameObject cameraRig;

    private BoxCollider bc;

    private bool caught; // once ball is caught, stay green

    public delegate void InReach();

    public static InReach IsInReach;

    public delegate void OutOfReach();

    public static OutOfReach IsOutOfReach;

	// Use this for initialization
	void Awake () {
        ControllerHandler.OnBallGrab += wasCaught;

        bc = gameObject.GetComponent<BoxCollider>();

        bc.center = new Vector3(cameraRig.transform.position.x, 
            cameraRig.transform.position.y + GameControl.Instance.heightMax / 2, 
            cameraRig.transform.position.z);
        bc.size = new Vector3(System.Math.Abs(GameControl.Instance.leftMax) 
            + GameControl.Instance.rightMax, GameControl.Instance.heightMax,
            GameControl.Instance.reachMax * 2);

	}

    void OnDisable()
    {
        ControllerHandler.OnBallGrab -= wasCaught;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            ToInBounds(other.gameObject);
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            ToInBounds(other.gameObject);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            ToOutofBounds(other.gameObject);
        }
    }

    private void ToInBounds(GameObject ball)
    {
        ball.GetComponent<Renderer>().material = inBounds;
    }

    private void ToOutofBounds(GameObject ball)
    {
        if (!caught)
        {
            ball.GetComponent<Renderer>().material = outOfBounds;
        }
    }

    private void wasCaught()
    {
        caught = true;
    }
}
