using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public static bool goalMet = false; // 标识目标是否已击中

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Projectile")
        {
            // 当弹球与目标接触时，改变目标的颜色
            Goal.goalMet = true;
            Color c = GetComponent<Renderer>().material.color;
            c.a = 1;
            GetComponent<Renderer>().material.color = c;
        }
    }
}
