using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public static bool goalMet = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Projectile")
        {
            Goal.goalMet = true;
            Color c = GetComponent<Renderer>().material.color;
            c.a = 1;
            GetComponent<Renderer>().material.color = c;
        }
    }
}
