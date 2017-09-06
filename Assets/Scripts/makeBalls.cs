using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class makeBalls : MonoBehaviour {
    public Canvas frontCanvas;

    public Canvas leftCanvas;

    public Canvas rightCanvas;

    public Canvas ceilingCanvas;

    public Canvas floorCanvas;

    public Rigidbody Ball; // the ball 

    public float speed; // the speed of the ball coming towards the target

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

        // disable all targets
        frontCanvas.GetComponent<Image>().enabled = true;
        leftCanvas.GetComponent<Image>().enabled = false;
        rightCanvas.GetComponent<Image>().enabled = false;
        ceilingCanvas.GetComponent<Image>().enabled = false;
        floorCanvas.GetComponent<Image>().enabled = false;

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

        // disable whichever target was previously enabled
        switch (targetDirection)
        {
            case 1:
                frontCanvas.GetComponent<Image>().enabled = false;
                break;
            case 2:
                leftCanvas.GetComponent<Image>().enabled = false;
                break;
            case 3:
                rightCanvas.GetComponent<Image>().enabled = false;
                break;
            case 4:
                floorCanvas.GetComponent<Image>().enabled = false;
                break;
            default:
                ceilingCanvas.GetComponent<Image>().enabled = false;
                break;
        }

        // now rotate and move to appropriate spot
        switch (direction)
        {
            // front
            case 1:
                frontCanvas.GetComponent<Image>().enabled = true;
                frontCanvas.GetComponent<Image>().GetComponent<RectTransform>().position 
                    = new Vector3(Random.Range(- fieldWidth / 2, fieldWidth / 2), 
                    frontCanvas.GetComponent<Image>().GetComponent<RectTransform>().position.y,
                    frontCanvas.GetComponent<Image>().GetComponent<RectTransform>().position.z);
                break;
            // left side
            case 2:
                leftCanvas.GetComponent<Image>().enabled = true;
                leftCanvas.GetComponent<Image>().GetComponent<RectTransform>().position
                    = new Vector3(leftCanvas.GetComponent<Image>().GetComponent<RectTransform>().position.x,
                    leftCanvas.GetComponent<Image>().GetComponent<RectTransform>().position.y,
                    Random.Range(0, fieldDepth));
                break;
            // right side
            case 3:
                rightCanvas.GetComponent<Image>().enabled = true;
                rightCanvas.GetComponent<Image>().GetComponent<RectTransform>().position
                    = new Vector3(rightCanvas.GetComponent<Image>().GetComponent<RectTransform>().position.x,
                    rightCanvas.GetComponent<Image>().GetComponent<RectTransform>().position.y,
                    Random.Range(0, fieldDepth));
                break;
            // bottom
            case 4:
                floorCanvas.GetComponent<Image>().enabled = true;
                floorCanvas.GetComponent<Image>().GetComponent<RectTransform>().position
                    = new Vector3(Random.Range(-fieldWidth / 2, fieldWidth / 2),
                    floorCanvas.GetComponent<Image>().GetComponent<RectTransform>().position.y,
                    floorCanvas.GetComponent<Image>().GetComponent<RectTransform>().position.z);
                break;
            // top
            default:
                ceilingCanvas.GetComponent<Image>().enabled = true;
                ceilingCanvas.GetComponent<Image>().GetComponent<RectTransform>().position
                    = new Vector3(Random.Range(-fieldWidth / 2, fieldWidth / 2),
                    ceilingCanvas.GetComponent<Image>().GetComponent<RectTransform>().position.y,
                    ceilingCanvas.GetComponent<Image>().GetComponent<RectTransform>().position.z);
                break;
        }

        targetDirection = direction;

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