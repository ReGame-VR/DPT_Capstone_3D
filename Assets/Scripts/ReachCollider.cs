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

    public delegate void InReach();

    public static InReach IsInReach;

    public delegate void OutOfReach();

    public static OutOfReach IsOutOfReach;

	// Use this for initialization
	void Start () {
        bc = gameObject.GetComponent<BoxCollider>();

        bc.center = new Vector3(cameraRig.transform.position.x, 
            cameraRig.transform.position.y + GameControl.Instance.heightMax / 2, 
            cameraRig.transform.position.z);
        bc.size = new Vector3(System.Math.Abs(GameControl.Instance.leftMax) 
            + GameControl.Instance.rightMax, GameControl.Instance.heightMax,
            System.Math.Abs(GameControl.Instance.leftMax) + GameControl.Instance.rightMax);

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
        ball.GetComponent<Renderer>().material = outOfBounds;
    }
}
