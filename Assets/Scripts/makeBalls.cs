using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class makeBalls : MonoBehaviour {
    public Rigidbody Ball; // the ball 

    private int score;

    public float speed;

    public GameObject cameraRig; // the camerarig prefab

    public float fieldWidth;

    public float fieldDepth;

    public float fieldHeight;

    public Text text;

    private bool isBall; // is there currently a ball?

    private float timeLeft = 10f;

    private Rigidbody newBall; // the new ball that is instantiated 

    // initialization of another ball
    void Start () {
        ControllerHandler.OnBallGrab += BallCaught;
        CreateBall();
        score = 0;
    }
    
    // Update is called once per frame
    void Update() {
        timeLeft -= Time.deltaTime;
        text.text = "Time Left:" + Mathf.Round(timeLeft) + "\nScore: " + score;

        if (timeLeft < 0)
        {
            DestroyBall();
            timeLeft = 10f;
        }

        if (!isBall)
        {
            CreateBall();
        }
    }

    void OnDisable()
    {
        ControllerHandler.OnBallGrab -= BallCaught;
    }

    private void CreateBall()
    {
        isBall = true;
        int direction = Random.Range(1, 4);

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

    // if the ball comes into contact with the target
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "BlueCircle" || collision.gameObject.name == "GreenCircle") //collides with a certain circle
        {

        }
    }

    private void BallCaught()
    {
        score += 5;
    }
}