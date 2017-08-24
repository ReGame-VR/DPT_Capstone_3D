using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class makeBalls : MonoBehaviour {
    public Rigidbody Ball; // the ball 
    int count = 0; // the number of balls in use, should no more than 1
    private Rigidbody newBall; // the new ball that is instantiated 
    public static int score = 0; // the total score 

    // initialization of another ball
    void Start () {
        newBall = Instantiate(Ball);
        count = 1;
        float Xspeed = Random.Range(-150f, 150f); //range of x directions for the ball
        float Yspeed = Random.Range(-130f, -100f); //range of y directions
        float Zspeed = Random.Range(-250f, -500f); //range of the ball speed
        newBall.AddForce(Xspeed, Yspeed, Zspeed);
    }
    
    // Update is called once per frame
    void Update() {
        if (newBall.position.z < -2.5)
        {
            Destroy(newBall.gameObject); // remove out of bounds ball 
            count = 0;
        }
        if (newBall.position.z > 3)
        {
            Destroy(newBall.gameObject); // remove out of bounds ball 
            count = 0;
        }
        if (count == 0)
        {
            Start(); // instantiate another ball 
        }
	}

    // if the ball comes into contact with the target
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "BlueCircle" || collision.gameObject.name == "GreenCircle") //collides with a certain circle
        {
            score++; 
            print("score: " + score); //prints to the console
        }
    }
}