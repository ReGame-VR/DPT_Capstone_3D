using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class makeBalls : MonoBehaviour {
    public Rigidbody Ball; // the ball 

    public float speed; // the speed of the ball coming towards the target

    public GameObject target; // the target prefab

    public GameObject cameraRig; // the camerarig prefab

    public float fieldWidth; 

    public float fieldDepth;

    public float fieldHeight;

    private bool isBall; // is there currently a ball?

    private Rigidbody newBall; // the new ball that is instantiated 

    private int targetDirection = 1;

    // initialization of another ball
    void Start () {
        UIController.OnTimeUp += Reset;
        MoveTarget();
        CreateBall();
    }

    void OnDisable()
    {
        UIController.OnTimeUp -= Reset;
    }

    private void CreateBall()
    {
        isBall = true;
        int direction = Random.Range(1, 4);

        // target and ball should not come from same direction 
        while (direction == targetDirection)
        {
            direction = Random.Range(1, 4);
        }
        Debug.Log(direction);

        float x, y, z;

        y = Random.Range(cameraRig.transform.position.y, fieldHeight);

        switch (direction)
        {
            // comes from front
            case 1:
                x = Random.Range(cameraRig.transform.position.x - fieldWidth / 2f, cameraRig.transform.position.x + fieldWidth / 2f);
                z = cameraRig.transform.position.z + fieldDepth;
                break;
            
            // comes from left
            case 2:
                x = cameraRig.transform.position.x - fieldWidth / 2;
                z = Random.Range(cameraRig.transform.position.z, cameraRig.transform.position.z + fieldDepth);
                break;

            // comes from right
            default:
                x = cameraRig.transform.position.x + fieldWidth / 2;
                z = Random.Range(cameraRig.transform.position.z, cameraRig.transform.position.z + fieldDepth);
                break;
        }

        newBall = Instantiate(Ball, new Vector3(x, y, z), Ball.transform.rotation);
        newBall.transform.LookAt(cameraRig.transform);
        newBall.AddRelativeForce(Vector3.forward*speed, ForceMode.Acceleration);
    }

    private void MoveTarget()
    {
        int direction = Random.Range(1, 6);
        // Debug.Log(direction);

        direction = 2;

        float x, y, z;
        x = 0;
        y = 0;
        z = 0;
        
        // reset back to no rotation so that it will rotate properly
        /* switch (targetDirection)
        {
            // front - do nothing 
            case 1:
                break;
            // left - rotate back to front
            case 2:
                target.transform.Rotate(0, 90, 0);
                break;
            // right
            case 3:
                target.transform.Rotate(0, -90, 0);
                break;
            // floor
            case 4:
                target.transform.Rotate(0, 0, -90);
                break;
            // ceiling
            default:
                target.transform.Rotate(0, 0, 90);
                break;
        }*/

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
                transform.Rotate(0, -90, 0, Space.World);
                x = cameraRig.transform.position.x - fieldWidth / 2;
                y = Random.Range(cameraRig.transform.position.y, fieldHeight);
                z = Random.Range(cameraRig.transform.position.z, cameraRig.transform.position.z + fieldDepth);
                break;
            // right
            case 3:
                transform.Rotate(0, 90, 0);
                break;
            // floor
            case 4:
                transform.Rotate(0, 0, 90);
                break;
            // ceiling
            default:
                transform.Rotate(0, 0, -90);
                break;
        }

        targetDirection = direction;

        target.transform.position = new Vector3(x, y, z);

    }

    private void DestroyBall()
    {
        Destroy(newBall.gameObject);
        isBall = false;
    } 

    private void Reset()
    {
        MoveTarget();
        DestroyBall();
        CreateBall();
    }
}