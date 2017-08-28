using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class makeTargets : MonoBehaviour {
    public GameObject newTarget;

    public GameObject cameraRig;

    public delegate void TargetHit();

    public static TargetHit OnTargetHit;

    public delegate void TargetMove(int direction);

    public static TargetMove OnTargetMove;

    public float fieldWidth;

    public float fieldDepth;

    public float fieldHeight;

    private int prevDirection;

    // Use this for initialization
    void Start () {
        UIController.OnTimeUp += moveTarget;
        // moveTarget();
	}
	
	// Update is called once per frame
	void Update () {
        UIController.OnTimeUp -= moveTarget;
    }

    private void moveTarget()
    {
        int direction = Random.Range(1, 6);

        float x, y, z;
        /*
        // reset back to no rotation so that it will rotate properly
        switch (prevDirection)
        {
            // front - do nothing 
            case 1:
                break;
            // left - rotate back to front
            case 2:
                transform.Rotate(0, -90, 0);
                break;
            // right
            case 3:
                transform.Rotate(0, 90, 0);
                break;
            // floor
            case 4:
                transform.Rotate(0, 0, -90);
                break;
            // ceiling
            default:
                transform.Rotate(0, 0, 90);
                break;
        }

        // now rotate and move to appropriate spot
        switch (direction)
        {
            // front - do nothing
            case 1:
                x = Random.Range(cameraRig.transform.position.x - fieldWidth / 2f, cameraRig.transform.position.x + fieldWidth / 2f);
                y = Random.Range(cameraRig.transform.position.y, fieldHeight);
                z = cameraRig.transform.position.z + fieldDepth;
                break;
            // left
            case 2:
                transform.Rotate(0, 90, 0);
                break;
            // right
            case 3:
                transform.Rotate(0, -90, 0);
                break;
            // floor
            case 4:
                transform.Rotate(0, 0, 90);
                break;
            // ceiling
            default:
                transform.Rotate(0, 0, -90);
                break;
        }*/

        prevDirection = direction;

        if (OnTargetMove != null)
        {
            OnTargetMove(prevDirection);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ball") {
            moveTarget();

            if (OnTargetHit != null)
            {
                OnTargetHit();
            }
        }
        
    }
}
