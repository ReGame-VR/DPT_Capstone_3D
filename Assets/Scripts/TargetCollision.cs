using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetCollision : MonoBehaviour {

    private AudioSource onHit;

    public delegate void TargetHit();

    public static TargetHit OnTargetHit;

    public void Start()
    {
         onHit = GetComponent<AudioSource>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ball")
        {
            onHit.Play();

            if (OnTargetHit != null)
            {
                OnTargetHit();
            }
        }

    }
}