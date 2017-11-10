using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// This class handles the trials as well as updating the UI with score.
/// </summary>
public class UIController : MonoBehaviour {

    // - - - - - PUBLIC VARIABLES - - - - -

    // the UI canvas that displays the front target (when active)
    public Canvas frontCanvas;

    // the UI canvas that displays the left target (when active)
    public Canvas leftCanvas;

    // the UI canvas that displays the right target (when active)
    public Canvas rightCanvas;

    // the UI canvas that displays the ceiling/top target (when active)
    public Canvas ceilingCanvas;

    // the UI canvas that displays the floor/bottom target (when active)
    public Canvas floorCanvas;

    // the trigger collider - moves with active target and scales with target scale
    public GameObject targetCollider;

    // the ball prefab
    public Rigidbody Ball; 

    // the BASE speed that the ball moves at. Scales with difficulty
    public float speed; 

    // the SteamVR camera rig prefab
    public GameObject cameraRig; 

    // the UI text that displays the score and endgame message
    public Text text;

    // the total amount of time to complete a single trial
    public float timer = 10f;

    // the z-axis distance that the ball spawns in front of the user
    public float spawnDistance = 7f;

    //the collider for the playing field
    public Collider playField;

    // - - - - - DELEGATES AND EVENTS - - - - -

    // An event to broadcast when ALL trials are complete
    public delegate void TrialsComplete(int score, int numSuccesses);

    public static TrialsComplete OnTrialsComplete;

    // An event to broadcast after a single trial is complete
    public delegate void TrialComplete(int trialNum, float catchTime,
        float throwTime, bool wasCaught, bool wasThrown, bool hitTarget, int score);

    public static TrialComplete RecordData;

    // - - - - - PRIVATE VARIABLES - - - - -

    // the new ball that is instantiated
    private Rigidbody newBall;

    // store target position for move ball and reset methods
    private int targetDirection = 1;

    // an empty object in grab range that the ball will move towards
    private GameObject obj;

    // the width of the target when at 100% scale.
    private float targetWidth = 6.5f;

    // have the set number of trials been completed?
    private bool isGameOver = false;

    // the number of trials to play
    private int numTrials;

    // which trial is currently running
    private int currTrial = 1;

    // the time left for the trial
    private float timeLeft;

    // the amount of times (in seconds) to rest between trials
    private float restPeriod;

    // the user score
    private int score = 0;

    // the difficulty the trial is playing at, represented by an integer index
    // this int can be used to retrieve information from the Difficulty class arrays by index
    private int difficulty;

    // subtract from score every scoreDecay frames
    private int scoreDecay;

    // this will measure the number of frames that have passed since ball entered reachable area
    private int numFrames = 0;

    // played when time is up
    private AudioSource onTimeUp;

    // the width (x axis) in Unity units that targets can spawn in
    private float fieldWidth = 12;

    // the depth (z axis) in Unity units that targets can spawn in
    private float fieldDepth = 9;

    // the height (y axis) in Unity units that the targets can spawn in
    private float fieldHeight = 9;

    // the offset is based off of the target size and ensures targets do not extend outside 
    // playable area
    private float offsetSize;

    // the number of trials where the ball was: successfully caught, successfully thrown, 
    // thrown into the target
    private int numCaught, numThrown, numSuccesses;

    // an offset value so that the ball spawns above mid thigh height 
    private float minBallHeight = 1f;

    // the total score during the previous trial (used to calculate per-trial score)
    private int prevScore = 0;

    // the amount of time to throw the ball once caught before it is destroyed (if 0, no limit)
    private float throwLim;

    // the amount of time the user held onto the ball after being caught
    private float holdTime = 0;

    // did they catch the ball this trial?
    private bool caught = false;

    // did they throw the ball this trial?
    private bool thrown = false;

    // did they hit the target this trial?
    private bool targetHit = false;

    // the time it took to catch the ball from spawn
    private float catchTime = float.PositiveInfinity;

    // the time it took to throw the ball from catch
    private float throwTime = float.PositiveInfinity;

    // the z-posn of the back collider wall
    private float zposn;

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

        numCaught = 0;
        numThrown = 0;
        numSuccesses = 0;

        zposn = playField.bounds.center.z - playField.bounds.size.z / 2 - 0.2f;

        onTimeUp = GetComponent<AudioSource>();
        obj = new GameObject();
        this.numTrials = GameControl.Instance.numTrials;

        // some difficulty parameters
        difficulty = GameControl.Instance.difficulty;
        restPeriod = Difficulty.delay[difficulty];
        scoreDecay = Difficulty.scoreDecay[difficulty];
        speed = speed * Difficulty.velocityScale[difficulty];
        throwLim = Difficulty.throwLim[difficulty];

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

            // if there is a score decay, decay
            if (scoreDecay != 0)
            {
                numFrames++;

                if (numFrames % scoreDecay == 0)
                {
                    score--;
                }
            }

            if (timeLeft > restPeriod)
            {
                // if the ball has not been caught, move it towards the obj
                if (!caught)
                {
                    float step = speed * Time.deltaTime;
                    newBall.transform.position = Vector3.MoveTowards(newBall.transform.position, 
                        obj.transform.position, step);
                }

                // if there is a time limit to throwing, check for it
                if (throwLim != 0 && caught && !thrown)
                {
                    holdTime += Time.deltaTime;

                    if (holdTime >= throwLim)
                    {
                        holdTime = 0;
                        timeLeft = restPeriod;
                        onTimeUp.Play();
                        DestroyBall();
                        MoveTarget();
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

    /// <summary>
    /// When the ball goes out of bounds, destroy it, set time to the rest period and then move 
    /// the target to get ready for next trial.
    /// </summary>
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
            RecordData(currTrial, catchTime, throwTime, caught, thrown, targetHit, score - prevScore);
        }

        prevScore = score;

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

            numSuccesses++;

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
            OnTrialsComplete(score, numSuccesses);
        }
    }

    /// <summary>
    /// Create a ball that comes from a spawnDistance * spawnDistance  square spawnDistance units
    /// in front of player, then move towards an object behind the player.
    /// </summary>
    private void CreateBall()
    {
        if (!isGameOver)
        {

            // set the gameobject that the ball will move towards
            obj.transform.position = new Vector3(Random.Range(GameControl.Instance.leftMax + 0.3f, 
                GameControl.Instance.rightMax - 0.3f),Random.Range(minBallHeight, 
                GameControl.Instance.heightMax - 0.3f), zposn);

            float x, y, z;

            x = Random.Range(cameraRig.transform.position.x - spawnDistance, 
                cameraRig.transform.position.x + spawnDistance);
            z = cameraRig.transform.position.z + spawnDistance;
            y = Random.Range(cameraRig.transform.position.y, spawnDistance);

            caught = false;
            newBall = Instantiate(Ball, new Vector3(x, y, z), Ball.transform.rotation);
            newBall.transform.localScale = newBall.transform.localScale * Difficulty.ballScale[difficulty];
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

    /// <summary>
    /// Destroys the current existing ball (if it has not already been destroyed).
    /// </summary>
    private void DestroyBall()
    {
        if (newBall) {
            Destroy(newBall.gameObject);
        }
    }
}
