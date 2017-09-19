﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class makeBalls : MonoBehaviour {
    public Canvas frontCanvas;

    public Canvas leftCanvas;

    public Canvas rightCanvas;

    public Canvas ceilingCanvas;

    public Canvas floorCanvas;

    public GameObject targetCollider;

    public Rigidbody Ball; // the ball 

    public float speed; // the speed of the ball coming towards the target

    public GameObject cameraRig; // the camerarig prefab

    public float fieldWidth; 

    public float fieldDepth;

    public float fieldHeight;

    private bool isBall; // is there currently a ball?

    private Rigidbody newBall; // the new ball that is instantiated 

    private int targetDirection = 1;

    private GameObject obj;

    private float targetWidth = 6.5f;

    private bool isGameOver = false;

    // initialization of another ball
    void Start () {
        UIController.OnReset += Reset;
        OutOfBounds.OnOutOfBounds += Reset;
        UIController.OnTrialsComplete += GameOver;

        obj = new GameObject();

        // disable all targets except for front
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
        UIController.OnReset -= Reset;
        OutOfBounds.OnOutOfBounds -= Reset;
        UIController.OnTrialsComplete -= GameOver;
    }

    private void CreateBall()
    {
        if (!isGameOver) {
            isBall = true;
            int direction = Random.Range(1, 4);

            // set the gameobject that the ball will move towards
            Vector3 posn = new Vector3(Random.Range(GameControl.Instance.leftMax, GameControl.Instance.rightMax),
                Random.Range(0, GameControl.Instance.heightMax), cameraRig.transform.position.z);
            obj.transform.position = posn;

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
            newBall.transform.LookAt(obj.transform);
            newBall.AddRelativeForce(Vector3.forward * speed, ForceMode.Acceleration);
        }
    }

    private void MoveTarget()
    {
        int direction = Random.Range(1, 6);

        BoxCollider col = targetCollider.GetComponent<BoxCollider>();

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

        if (!isGameOver) {

            // now rotate and move to appropriate spot along with collider
            switch (direction)
            {
                // front
                case 1:
                    frontCanvas.GetComponent<Image>().enabled = true;
                    frontCanvas.GetComponent<Image>().GetComponent<RectTransform>().position
                        = new Vector3(Random.Range(-fieldWidth / 2, fieldWidth / 2),
                        frontCanvas.GetComponent<Image>().GetComponent<RectTransform>().position.y,
                        frontCanvas.GetComponent<Image>().GetComponent<RectTransform>().position.z);
                    col.size = new Vector3(targetWidth, targetWidth, 0.3f);
                    targetCollider.transform.position = frontCanvas.transform.position;
                    break;
                // left side
                case 2:
                    leftCanvas.GetComponent<Image>().enabled = true;
                    leftCanvas.GetComponent<Image>().GetComponent<RectTransform>().position
                        = new Vector3(leftCanvas.GetComponent<Image>().GetComponent<RectTransform>().position.x,
                        leftCanvas.GetComponent<Image>().GetComponent<RectTransform>().position.y,
                        Random.Range(0, fieldDepth));
                    col.size = new Vector3(0.3f, targetWidth, targetWidth);
                    targetCollider.transform.position = leftCanvas.transform.position;
                    break;
                // right side
                case 3:
                    rightCanvas.GetComponent<Image>().enabled = true;
                    rightCanvas.GetComponent<Image>().GetComponent<RectTransform>().position
                        = new Vector3(rightCanvas.GetComponent<Image>().GetComponent<RectTransform>().position.x,
                        rightCanvas.GetComponent<Image>().GetComponent<RectTransform>().position.y,
                        Random.Range(0, fieldDepth));
                    col.size = new Vector3(0.3f, targetWidth, targetWidth);
                    targetCollider.transform.position = rightCanvas.transform.position;
                    break;
                // bottom
                case 4:
                    floorCanvas.GetComponent<Image>().enabled = true;
                    floorCanvas.GetComponent<Image>().GetComponent<RectTransform>().position
                        = new Vector3(Random.Range(-fieldWidth / 2, fieldWidth / 2),
                        floorCanvas.GetComponent<Image>().GetComponent<RectTransform>().position.y,
                        floorCanvas.GetComponent<Image>().GetComponent<RectTransform>().position.z);
                    col.size = new Vector3(targetWidth, 0.3f, targetWidth);
                    targetCollider.transform.position = floorCanvas.transform.position;
                    break;
                // top
                default:
                    ceilingCanvas.GetComponent<Image>().enabled = true;
                    ceilingCanvas.GetComponent<Image>().GetComponent<RectTransform>().position
                        = new Vector3(Random.Range(-fieldWidth / 2, fieldWidth / 2),
                        ceilingCanvas.GetComponent<Image>().GetComponent<RectTransform>().position.y,
                        ceilingCanvas.GetComponent<Image>().GetComponent<RectTransform>().position.z);
                    col.size = new Vector3(targetWidth, 0.3f, targetWidth);
                    targetCollider.transform.position = ceilingCanvas.transform.position;
                    break;
            }

            targetDirection = direction;

        }

    }

    private void DestroyBall()
    {
        Destroy(newBall.gameObject);
        isBall = false;
    } 

    private void Reset()
    {
        if (!isGameOver) {
            MoveTarget();
            DestroyBall();
            CreateBall();
        }
    }

    private void GameOver()
    {
        isGameOver = true;

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
    }
}