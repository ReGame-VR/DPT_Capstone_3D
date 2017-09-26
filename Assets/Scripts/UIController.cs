using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

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

    private Rigidbody newBall; // the new ball that is instantiated 

    private int targetDirection = 1;

    private GameObject obj;

    private float targetWidth = 6.5f;

    private bool isGameOver = false;

    public int numTrials = 40;

    private int currTrial = 1;

    public Text text;

    public float timer = 5f;

    private float timeLeft;

    private int score;

    public delegate void TimeUp();

    public static TimeUp OnReset;

    public delegate void TrialsComplete();

    public static TrialsComplete OnTrialsComplete;

    public delegate void TrialComplete (int trialNum, float catchTime,
        float throwTime, bool wasCaught, bool wasThrown, bool hitTarget);

    public static TrialComplete RecordData;

    private AudioSource onTimeUp;

    private bool caught;

	// Use this for initialization
	void Start () {
        TargetCollision.OnTargetHit += this.Reset;
        ControllerHandler.OnBallGrab += this.BallCaught;
        OutOfBounds.OnOutOfBounds += this.OnFail;

        onTimeUp = GetComponent<AudioSource>();

        obj = new GameObject();

        // disable all targets except for front
        frontCanvas.GetComponent<Image>().enabled = true;
        leftCanvas.GetComponent<Image>().enabled = false;
        rightCanvas.GetComponent<Image>().enabled = false;
        ceilingCanvas.GetComponent<Image>().enabled = false;
        floorCanvas.GetComponent<Image>().enabled = false;

        timeLeft = timer;
        score = 0;
        MoveTarget();
        CreateBall();
    }

    void OnDisable()
    {
        TargetCollision.OnTargetHit -= this.Reset;
        ControllerHandler.OnBallGrab -= this.BallCaught;
        OutOfBounds.OnOutOfBounds -= this.OnFail;
    }

    // Update is called once per frame
    void Update()
    {

        if (currTrial < numTrials) {
            timeLeft -= Time.deltaTime;
            text.text = "Trial " + currTrial + " of " + numTrials +
                "\nTime Left:" + Mathf.Round(timeLeft) + "\nScore: " + score;

            if (timeLeft <= 0)
            {
                onTimeUp.Play();
                currTrial++;

                if (OnReset != null)
                {
                    OnReset();
                }

                timeLeft = timer;

            }
        }
        else
        {
            OnTrialsOver();
        }
    }

    private void UpdateScore()
    {
        score += 100;
    }

    private void BallCaught()
    {
        // UpdateScore();
        caught = true;
    }

    private void Reset()
    {

        DestroyBall();
        caught = false;
        UpdateScore();

        if (currTrial < numTrials)
        {

            if (OnReset != null)
            {
                OnReset();
            }

            MoveTarget();
            CreateBall();
            timeLeft = timer;
            currTrial++;
        }
        else
        {
            OnTrialsOver();
        }
    }

    private void OnFail()
    {
        DestroyBall();
        caught = false;

        if (currTrial < numTrials)
        {
            MoveTarget();
            CreateBall();
            currTrial++;
            timeLeft = timer;
        }
        else
        {
            OnTrialsOver();
        }
    }

    private void OnTrialsOver()
    {
        // disable last target
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

        text.text = "Congratulations!\nScore: " + score;

        if (OnTrialsComplete != null)
        {
            OnTrialsComplete();
        }
    }

    private void CreateBall()
    {
        if (!isGameOver)
        {
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

        if (!isGameOver)
        {

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
    }
}
