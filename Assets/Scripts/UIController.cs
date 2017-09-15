using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    [SerializeField]
    private Text text;

    [SerializeField]
    private float timer = 5f;

    private float timeLeft;

    private int score;

    public delegate void TimeUp();

    public static TimeUp OnReset;

    private AudioSource onTimeUp;

	// Use this for initialization
	void Start () {
        TargetCollision.OnTargetHit += this.OnTargetHit;
        ControllerHandler.OnBallGrab += this.UpdateScore;

        onTimeUp = GetComponent<AudioSource>();

        timeLeft = timer;
        score = 0;
	}

    void OnDisable()
    {
        TargetCollision.OnTargetHit -= this.OnTargetHit;
        ControllerHandler.OnBallGrab -= UpdateScore;
    }

    // Update is called once per frame
    void Update()
    {
        timeLeft -= Time.deltaTime;
        text.text = "Time Left:" + Mathf.Round(timeLeft) + "\nScore: " + score;

        if (timeLeft <= 0)
        {
            onTimeUp.Play();

            if (OnReset != null)
            {
                OnReset();
            }

            timeLeft = timer;
        }
    }

    private void UpdateScore()
    {
        score += 5;
    }

    private void OnTargetHit()
    {
        UpdateScore();

        if (OnReset != null)
        {
            OnReset();
        }

        timeLeft = timer;
    }
}
