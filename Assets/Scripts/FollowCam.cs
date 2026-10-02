using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public static FollowCam S; // 单例
    public float easing = 0.05f; // 平滑程度
    public Vector2 minXY; // 范围
    

    public bool _____;

    public GameObject poi; //兴趣点
    public float camZ; // 相机的z坐标

    private void Awake()
    {
        S = this;
        camZ = this.transform.position.z;
    }

    // 由于Update()每一帧都会执行，所以在弹球运动时，弹球的物理运动还没开始，摄像机就已经先更新了，就会产生卡顿现象
    // 选择FixedUpdate()是为了让摄像机在固定物理帧间隔下运行，保证摄像机跟踪时不会卡顿
    private void FixedUpdate()
    {
        Vector3 destination;
        // 如果兴趣点不在，就返回到原点
        if (poi == null)
        {
            destination = Vector3.zero;
        }
        // 否则将向兴趣点移动
        else
        {
            destination = poi.transform.position;
            // 特别地，如果跟踪的是弹球，那么还得看弹球是不是在运动，如果不在运动，我们得回到弹弓的地方，而不是继续盯着弹球
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

        // 线性插值，改变相机的位置
        destination = Vector3.Lerp(transform.position, destination,easing); 
        destination.z = camZ;
        this.transform.position = destination; 
        // 保证地面始终在屏幕上显示，弹球如果飞太高，而相机跟随着弹球移动，如果没有地面作为参考，玩家很难感受到弹球飞了多高
        Camera.main.orthographicSize = destination.y + 10; 
    }
}
