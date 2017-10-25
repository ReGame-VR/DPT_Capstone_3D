using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Slightly misleading title, but this class runs the trials.
/// </summary>
public class UIController : MonoBehaviour {

    // - - - - - PUBLIC VARIABLES - - - - -

    public Canvas frontCanvas;

    public Canvas leftCanvas;

    public Canvas rightCanvas;

    public Canvas ceilingCanvas;

    public Canvas floorCanvas;

    public GameObject targetCollider;

    public Rigidbody Ball; // the ball 

    public float speed; // the speed of the ball coming towards the target

    public GameObject cameraRig; // the camerarig prefab

    public Text text;

    public float timer = 10f;

    public float spawnDistance = 7f;

    // - - - - - DELEGATES AND EVENTS - - - - -

    public delegate void TrialsComplete();

    public static TrialsComplete OnTrialsComplete;

    public delegate void TrialComplete(int trialNum, float catchTime,
        float throwTime, bool wasCaught, bool wasThrown, bool hitTarget);

    public static TrialComplete RecordData;

    // - - - - - PRIVATE VARIABLES - - - - -

    private Rigidbody newBall; // the new ball that is instantiated 

    private int targetDirection = 1; // store target position for move ball and reset methods

    private GameObject obj; // an empty object in grab range that the ball will move towards

    private float targetWidth = 6.5f; // the width of the target when at 100% scale.

    private bool isGameOver = false; // have the set number of trials been completed?

    private int numTrials; // the number of trials to play

    private int currTrial = 1; // which trial is currently running

    private float timeLeft; // the time left for the trial

    private float restPeriod; // the amount of times (in seconds) to rest between trials

    private int score = 0; // the user score

    private int difficulty; // the difficulty the trial is playing at - 0, 1, 2, or 3

    private int scoreDecay; // subtract from score every scoreDecay frames

    private int numFrames = 0; // this will measure the number of frames that have passed
                               // since ball entered reachable area

    private AudioSource onTimeUp; // played when time is up

    private float fieldWidth = 12;

    private float fieldDepth = 9;

    private float fieldHeight = 9;

    private float offsetSize;

    // private bool decay = false;

    // for data recording 
    private bool caught = false;

    private bool thrown = false;

    private bool targetHit = false;

    private float catchTime = float.PositiveInfinity;

    private float throwTime = float.PositiveInfinity;

	/// <summary>
    /// Subscribe to events and initialize values.
    /// </summary>
	void Awake () {
        TargetCollision.OnTargetHit += this.TargetHit;
        ControllerHandler.OnBallGrab += this.BallCaught;
        ControllerHandler.OnBallRelease += this.BallReleased;
        OutOfBounds.OnOutOfBounds += this.OnOutOfBounds;
        // ReachCollider.IsInReach += this.IsReachable;
        // ReachCollider.IsOutOfReach += this.IsNotReachable;

        onTimeUp = GetComponent<AudioSource>();
        obj = new GameObject();
        this.numTrials = GameControl.Instance.numTrials;

        // some difficulty parameters
        difficulty = GameControl.Instance.difficulty;
        restPeriod = Difficulty.delay[difficulty];
        scoreDecay = Difficulty.scoreDecay[difficulty];
        speed = speed * Difficulty.velocityScale[difficulty];

        // scale targets according to difficulty:
        frontCanvas.transform.localScale = frontCanvas.transform.localScale 
            * Difficulty.sunScale[difficulty];
        leftCanvas.transform.localScale = leftCanvas.transform.localScale 
            * Difficulty.sunScale[difficulty];
        rightCanvas.transform.localScale = rightCanvas.transform.localScale 
            * Difficulty.sunScale[difficulty];
        ceilingCanvas.transform.localScale = ceilingCanvas.transform.localScale 
            * Difficulty.sunScale[difficulty];
        floorCanvas.transform.localScale = floorCanvas.transform.localScale 
            * Difficulty.sunScale[difficulty];

        targetCollider.transform.localScale = targetCollider.transform.localScale 
            * Difficulty.sunScale[difficulty];

        offsetSize = targetWidth * Difficulty.sunScale[difficulty] / 2;
        
        // disable all targets except for front
        frontCanvas.GetComponent<Image>().enabled = true;
        leftCanvas.GetComponent<Image>().enabled = false;
        rightCanvas.GetComponent<Image>().enabled = false;
        ceilingCanvas.GetComponent<Image>().enabled = false;
        floorCanvas.GetComponent<Image>().enabled = false;

        timer += restPeriod;
        timeLeft = timer;

        MoveTarget();
        CreateBall();
    }

    /// <summary>
    /// Unsubscribe to events.
    /// </summary>
    void OnDisable()
    {
        TargetCollision.OnTargetHit -= this.TargetHit;
        ControllerHandler.OnBallGrab -= this.BallCaught;
        ControllerHandler.OnBallRelease -= this.BallReleased;
        OutOfBounds.OnOutOfBounds -= this.OnOutOfBounds;
        // ReachCollider.IsInReach += this.IsReachable;
        // ReachCollider.IsOutOfReach += this.IsNotReachable;
    }

    /// <summary>
    /// Update is called every frame and handles the timer, delay between trials, 
    /// and trial reset.
    /// </summary>
    void Update()
    {

        if (!isGameOver) {
            // numFrames++;
            timeLeft -= Time.deltaTime;
            text.text = /*"Trial " + currTrial + " of " + numTrials +
                "\nTime Left: " + Mathf.Round(timeLeft) +*/ "Score: " + score;

            if (timeLeft > restPeriod)
            {
                if (scoreDecay != 0)
                {
                    numFrames++;

                    if (numFrames % scoreDecay == 0)
                    {
                        score--;
                    }
                }
            }

            else
            { 
                // delete ball and play sound, but only if ball has not been destroyed by other functions
                if (newBall)
                {
                    onTimeUp.Play();
                    DestroyBall();
                    MoveTarget();
                }
                if (timeLeft <= 0)
                {
                    Reset();
                }
            }
        }
    }

    private void OnOutOfBounds()
    {
        timeLeft = restPeriod;
        DestroyBall();
        MoveTarget();
    }


    /// <summary>
    /// Record that the ball was caught and at what time.
    /// </summary>
    private void BallCaught()
    {
        caught = true;
        catchTime = timer - timeLeft;
    }

    /// <summary>
    /// Record that the ball was thrown and at what time.
    /// </summary>
    private void BallReleased()
    {
        // Debug.Log("Thrown: " + newBall.GetComponent<Rigidbody>().velocity.magnitude);
        thrown = true;
        throwTime = timer - timeLeft - catchTime;
    }

    /// <summary>
    /// Reset for the next trial, and record the data from this trial.
    /// </summary>
    private void Reset()
    {
        if (RecordData != null)
        {
            RecordData(currTrial, catchTime, throwTime, caught, thrown, targetHit);
        }

        caught = false;
        thrown = false;
        targetHit = false;
        throwTime = float.PositiveInfinity;
        catchTime = float.PositiveInfinity;

        if (currTrial < numTrials)
        {
            CreateBall();
            timeLeft = timer;
            currTrial++;
        }
        else
        {
            isGameOver = true;
            OnTrialsOver();
        }
    }

    /// <summary>
    /// Called when the target is hit, and increases score if the ball was thrown.
    /// </summary>
    private void TargetHit()
    {
        if (caught && thrown) {
            targetHit = true;
            score += 100;

            DestroyBall();
            MoveTarget();

            timeLeft = restPeriod;
        }
    }

    /// <summary>
    /// Disable the last target.
    /// </summary>
    private void disableTarget()
    {
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

    /// <summary>
    /// Called once all trials are over
    /// </summary>
    private void OnTrialsOver()
    {
        disableTarget();

        text.text = "Congratulations!\nScore: " + score;

        if (OnTrialsComplete != null)
        {
            OnTrialsComplete();
        }
    }

    /// <summary>
    /// Create a ball coming from a random direction.
    /// </summary>
    private void CreateBall()
    {
        if (!isGameOver)
        {
            // int direction = Random.Range(1, 4);

            // set the gameobject that the ball will move towards
            Vector3 posn = new Vector3(Random.Range(GameControl.Instance.leftMax + 0.3f, GameControl.Instance.rightMax - 0.3f),
                Random.Range(0, GameControl.Instance.heightMax), cameraRig.transform.position.z);
            obj.transform.position = posn;

            float x, y, z;

            x = Random.Range(cameraRig.transform.position.x - spawnDistance, 
                cameraRig.transform.position.x + spawnDistance);
            z = cameraRig.transform.position.z + spawnDistance;
            y = Random.Range(cameraRig.transform.position.y, spawnDistance);

            /*
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
            }*/

            caught = false;
            newBall = Instantiate(Ball, new Vector3(x, y, z), Ball.transform.rotation);
            newBall.transform.LookAt(obj.transform);
            newBall.AddRelativeForce(Vector3.forward * speed, ForceMode.Acceleration);
        }
    }

    /// <summary>
    /// Creates target in a random direction.
    /// </summary>
    private void MoveTarget()
    {
        int direction = Random.Range(1, 6);

        BoxCollider col = targetCollider.GetComponent<BoxCollider>();

        disableTarget();

        if (!isGameOver)
        {
            // now rotate and move to appropriate spot along with collider
            switch (direction)
            {
                // front
                case 1:
                    frontCanvas.GetComponent<Image>().enabled = true;
                    frontCanvas.GetComponent<Image>().GetComponent<RectTransform>().position
                        = new Vector3(Random.Range(-fieldWidth / 2 + offsetSize, 
                        fieldWidth / 2 - offsetSize),
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
                        Random.Range(offsetSize, fieldDepth - offsetSize));
                    col.size = new Vector3(0.3f, targetWidth, targetWidth);
                    targetCollider.transform.position = leftCanvas.transform.position;
                    break;
                // right side
                case 3:
                    rightCanvas.GetComponent<Image>().enabled = true;
                    rightCanvas.GetComponent<Image>().GetComponent<RectTransform>().position
                        = new Vector3(rightCanvas.GetComponent<Image>().GetComponent<RectTransform>().position.x,
                        rightCanvas.GetComponent<Image>().GetComponent<RectTransform>().position.y,
                        Random.Range(offsetSize, fieldDepth - offsetSize));
                    col.size = new Vector3(0.3f, targetWidth, targetWidth);
                    targetCollider.transform.position = rightCanvas.transform.position;
                    break;
                // bottom
                case 4:
                    floorCanvas.GetComponent<Image>().enabled = true;
                    floorCanvas.GetComponent<Image>().GetComponent<RectTransform>().position
                        = new Vector3(Random.Range(-fieldWidth / 2 + offsetSize, fieldWidth / 2 - offsetSize),
                        floorCanvas.GetComponent<Image>().GetComponent<RectTransform>().position.y,
                        floorCanvas.GetComponent<Image>().GetComponent<RectTransform>().position.z);
                    col.size = new Vector3(targetWidth, 0.3f, targetWidth);
                    targetCollider.transform.position = floorCanvas.transform.position;
                    break;
                // top
                default:
                    ceilingCanvas.GetComponent<Image>().enabled = true;
                    ceilingCanvas.GetComponent<Image>().GetComponent<RectTransform>().position
                        = new Vector3(Random.Range(-fieldWidth / 2 + offsetSize, fieldWidth / 2 - offsetSize),
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
        if (newBall) {
            Destroy(newBall.gameObject);
        }
    }
}
