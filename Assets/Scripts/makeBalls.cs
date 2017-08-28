using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class makeBalls : MonoBehaviour {
    public Rigidbody Ball; // the ball 

    public float speed; // the speed of the ball coming towards the target

    public GameObject cameraRig; // the camerarig prefab

    public float fieldWidth; 

    public float fieldDepth;

    public float fieldHeight;

    private bool isBall; // is there currently a ball?

    private Rigidbody newBall; // the new ball that is instantiated 

    private int targetDirection = 0;

    // initialization of another ball
    void Start () {
        makeTargets.OnTargetMove += OnTargetMove;
        UIController.OnTimeUp += DestroyBall;

        CreateBall();
    }

    void OnDisable()
    {
        makeTargets.OnTargetMove -= OnTargetMove;
        UIController.OnTimeUp -= DestroyBall;
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

    private void DestroyBall()
    {
        Destroy(newBall.gameObject);
        isBall = false;
    }

    private void OnTargetMove(int direction)
    {
        targetDirection = direction;
        CreateBall();
    }
}