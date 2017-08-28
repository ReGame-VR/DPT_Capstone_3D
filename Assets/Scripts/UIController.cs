using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    public Text text;

    public float timer = 10f;

    private float timeLeft;

    private int score;

    public delegate void TimeUp();

    public static TimeUp OnTimeUp;

	// Use this for initialization
	void Start () {
        makeTargets.OnTargetHit -= this.OnTargetHit;

        timeLeft = timer;
        score = 0;
	}

    void OnDisable()
    {
        makeTargets.OnTargetHit -= this.OnTargetHit;
    }

    // Update is called once per frame
    void Update()
    {
        timeLeft -= Time.deltaTime;
        text.text = "Time Left:" + Mathf.Round(timeLeft) + "\nScore: " + score;

        if (timeLeft <= 0)
        {
            if (OnTimeUp != null)
            {
                OnTimeUp();
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
        timeLeft = timer;
    }
}
