using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetCollision : MonoBehaviour
{
    public delegate void TargetHit();

    public static TargetHit OnTargetHit;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ball")
        {
            // moveTarget();

            if (OnTargetHit != null)
            {
                OnTargetHit();
            }
        }

    }
}