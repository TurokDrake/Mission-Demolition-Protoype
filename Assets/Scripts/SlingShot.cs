using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlingShot : MonoBehaviour
{
    public static SlingShot S; // 单例
    public GameObject prefabProjectile; // 弹球预制体
    public float velocityMult = 4f; // 鼠标松开后弹球的速度
    public bool __________; // 本书中经常使用这样的形式，表示在Inspector面板中分开成员的分隔符

    public GameObject launchPoint; // 发射点
    public Vector3 launchPos; // 发射位置
    public GameObject projectile; // 弹球
    public bool aimingMode; // 是否在瞄准

    // public Rigidbody rbOfProjectile; // 弹丸的刚体组件，由于弹丸是在鼠标点击后创建的，所有刚开始时是无法获得其组件的，因此这里注释掉该行代码
    public SphereCollider colliderOfLaunchPoint; // 发射点的碰撞体组件，在开始时就获得其引用，就不用在Update()中每一帧都调用GetComponent()方法

    private void Awake()
    {
        // 绑定单例对象
        S = this;
        // 找到LaunchPoint（发射点）
        Transform launchPointTrans = transform.Find("LaunchPoint"); 
        launchPoint = launchPointTrans.gameObject;
        // 默认不显示发射点，鼠标点击时才显示
        launchPoint.SetActive(false); 
        launchPos = launchPointTrans.position;
    }
    private void Start()
    {
        // 获取Collider组件
        colliderOfLaunchPoint = this.GetComponent<SphereCollider>();
        // rbOfProjectile = projectile.GetComponent<Rigidbody>();
    }
    private void Update()
    {
        // 若未处于瞄准模式，直接返回，不执行下面的代码
        if (!aimingMode) 
            return;

        // 获取鼠标位置，并转化成世界坐标
        Vector3 mousePos2D = Input.mousePosition; 
        mousePos2D.z = -Camera.main.transform.position.z; 
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D); 

        // 计算从发射点指向鼠标位置的向量，后面会用到其大小和方向
        Vector3 mouseDelta = mousePos3D - launchPos; 
        
        float maxMagnitude = colliderOfLaunchPoint.radius; 

        // 这里是限制弹丸的位置，以免超出碰撞检测区域
        // magnitude是向量的模长
        if (mouseDelta.magnitude > maxMagnitude) 
        {
            // 先对向量归一化处理
            mouseDelta.Normalize(); 
            // 再重置其大小
            mouseDelta *= maxMagnitude; 
        }

        // 让弹丸跟着鼠标移动
        Vector3 projPos = launchPos + mouseDelta; 
        projectile.transform.position = projPos;

        // 松开鼠标左键时执行以下逻辑
        if (Input.GetMouseButtonUp(0))
        {
            // 脱离瞄准模式
            aimingMode = false; 
            // 改变弹丸刚体的isKinematic属性，这样弹丸就会受到重力影响
            projectile.GetComponent<Rigidbody>().isKinematic = false; 
            // 模拟弹弓效果，弹丸会以 与拉弓相反 的方向移动
            projectile.GetComponent<Rigidbody>().velocity = -mouseDelta * velocityMult; 
            // 每次发射弹丸，都让相机聚焦于弹丸
            FollowCam.S.poi = projectile; 
            projectile = null; 
            // 记录射击的次数
            MissionDemolition.ShotFired();
        }
    }

    // 当Collider被设置为Trigger（触发器）时，这两个方法才有效
    // 当鼠标进入collider时会调用该函数
    private void OnMouseEnter()
    {
        // 测试用 print("SlingShot: OnMouseEnter()");
        // 当鼠标进入碰撞体时，显示发射点，我们为发射点绑定了一个Halo（光环）组件，因此可以形象地看出
        launchPoint.SetActive(true); 
    }

    // 当鼠标离开collider时调用该函数
    private void OnMouseExit()
    {
        // 测试用 print("SlingShot: OnMouseExit");
        // 当鼠标离开碰撞体时，隐藏发射点
        launchPoint.SetActive(false); 

    }

    // 当鼠标在collider上点击时调用该函数
    private void OnMouseDown()
    {
        //进入瞄准模式
        aimingMode = true;
        projectile = Instantiate<GameObject>(prefabProjectile);
        projectile.transform.position = launchPos;
        //设置弹丸刚体组件的isKinematic属性，kinematic为运动学刚体，这种状态下，对象的运动不会自动遵循物理原理，但仍属于物理模拟的构成部分（运动不会收到重力和碰撞的影响，但会影响其他非运动学刚体的运动）
        projectile.GetComponent<Rigidbody>().isKinematic = true; 
    }
}
