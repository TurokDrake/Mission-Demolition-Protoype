using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public static FollowCam S; // 单例
    public float easing = 0.05f;
    public Vector2 minXY;
    

    public bool _____;

    public GameObject poi; //兴趣点
    public float camZ; // z axis of camera

    private void Awake()
    {
        S = this;
        camZ = this.transform.position.z;
    }

    // To solve the problem resulted from that Update() will be called every frame, We use FixedUpdate()
    // 选择FixedUpdate()是为了让摄像机在固定物理帧间隔下运行，保证摄像机跟踪时不会卡顿
    private void FixedUpdate()
    {
        Vector3 destination;
        // 如果兴趣点不在，就返回到原点
        if (poi == null)
        {
            destination = Vector3.zero;
        }
        else
        {
            destination = poi.transform.position;
            if (poi.tag == "Projectile")
            {
                if (poi.GetComponent<Rigidbody>().IsSleeping())
                {
                    poi = null;
                    return;
                }
            }
        }


        // 限制目标位置
        destination.x = Mathf.Max(minXY.x, destination.x);
        destination.y = Mathf.Max(minXY.y, destination.y);

        destination = Vector3.Lerp(transform.position, destination,easing); // Linear Lerp
        destination.z = camZ;
        this.transform.position = destination; // Set position of camera to destination.
        Camera.main.orthographicSize = destination.y + 10; // Set orthographicSize of camera to make ground in the view of camera all the way.
    }
}
