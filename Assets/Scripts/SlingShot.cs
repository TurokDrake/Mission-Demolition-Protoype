using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlingShot : MonoBehaviour
{
    public static SlingShot S;
    public GameObject prefabProjectile;
    public float velocityMult = 4f;
    public bool __________; // 本书中经常使用这样的形式，表示在Inspector面板中分开成员的分隔符

    public GameObject launchPoint;
    public Vector3 launchPos;
    public GameObject projectile;
    public bool aimingMode;

    // public Rigidbody rbOfProjectile; // 弹丸的刚体组件，由于弹丸是在鼠标点击后创建的，所有刚开始时是无法获得其组件的，因此这里注释掉该行代码
    public SphereCollider colliderOfLaunchPoint; // 发射点的碰撞体组件，在开始时就获得其引用，就不用在Update()中每一帧都调用GetComponent()方法

    private void Awake()
    {
        S = this;
        Transform launchPointTrans = transform.Find("LaunchPoint"); // 找到LaunchPoint（发射点）的Transform组件
        launchPoint = launchPointTrans.gameObject; // 绑定发射点的游戏对象
        launchPoint.SetActive(false); // 将发射点这个游戏对象失活，不让它在场景中显示，当我们鼠标在这个位置时才显示
        launchPos = launchPointTrans.position;
    }
    private void Start()
    {
        colliderOfLaunchPoint = this.GetComponent<SphereCollider>();
        // rbOfProjectile = projectile.GetComponent<Rigidbody>();
    }
    private void Update()
    {
        if (!aimingMode) // 若未处于瞄准模式，直接返回，不执行下面的代码
            return;

        Vector3 mousePos2D = Input.mousePosition; // 获取鼠标坐标
        mousePos2D.z = -Camera.main.transform.position.z; // 设置鼠标的z轴位置
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D); // 从屏幕坐标转化成世界坐标

        Vector3 mouseDelta = mousePos3D - launchPos; // 计算鼠标位置到弹丸发射点的矢量
        float maxMagnitude = colliderOfLaunchPoint.radius; // 获取发射点的球体组件的半径

        // 这里是限制弹丸的位置，以免超出碰撞检测区域
        if (mouseDelta.magnitude > maxMagnitude) // 如果鼠标位置与弹丸发射点位置的距离大于碰撞检测区域的半径
        {
            mouseDelta.Normalize(); // 归一化矢量，让矢量的模长变为1；这里让鼠标位置与弹丸发射点位置的距离变为1
            mouseDelta *= maxMagnitude; // 让这个模长为1的矢量的大小和碰撞检测区域的半径一致
        }

        // 修改弹丸的位置
        Vector3 projPos = launchPos + mouseDelta; // 跟着鼠标动
        projectile.transform.position = projPos;

        // 松开鼠标左键那一刻
        if (Input.GetMouseButtonUp(0))
        {
            aimingMode = false; // 不再处于瞄准模式
            projectile.GetComponent<Rigidbody>().isKinematic = false; // 开始受到重力影响
            projectile.GetComponent<Rigidbody>().velocity = -mouseDelta * velocityMult; // 松开后有一个反向的速度，模拟弹弓往自己的方向拉弓，弹丸向远离自己的方向运动
            FollowCam.S.poi = projectile; // When button(0) up, set camera's poi.
            projectile = null; // 能被控制的弹丸已经发射出去了，需要我们再次点击鼠标左键创建一个可以控制的弹丸
            MissionDemolition.ShotFired();
        }
    }

    // 当Collider被设置为Trigger（触发器）时，这两个方法才有效
    // 当鼠标进入collider时会调用该函数
    private void OnMouseEnter()
    {
        // print("SlingShot: OnMouseEnter()");
        launchPoint.SetActive(true); // 当鼠标进入碰撞体时，显示发射点，我们为发射点绑定了一个Halo（光环）组件，因此可以形象地看出
    }

    // 当鼠标离开collider时调用该函数
    private void OnMouseExit()
    {
        // print("SlingShot: OnMouseExit");
        launchPoint.SetActive(false); // 当鼠标离开碰撞体时，隐藏发射点，我们为发射点绑定了一个Halo（光环）组件，因此可以形象地看出

    }

    // 当鼠标在collider上点击时调用该函数
    private void OnMouseDown()
    {
        aimingMode = true; // 标识当前是瞄准模式
        projectile = Instantiate<GameObject>(prefabProjectile); // 实例化弹丸预制体
        projectile.transform.position = launchPos; // 设置弹丸预制体的位置
        projectile.GetComponent<Rigidbody>().isKinematic = true; //设置弹丸刚体组件的isKinematic属性，kinematic为运动学刚体，这种状态下，对象的运动不会自动遵循物理原理，但仍属于物理模拟的构成部分（运动不会收到重力和碰撞的影响，但会影响其他非运动学刚体的运动）
    }
}
