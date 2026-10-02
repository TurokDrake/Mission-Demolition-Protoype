using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 游戏模式
public enum GameMode
{
    idle, // 闲置
    playing, // 正在玩
    levelEnd // 关卡结束
}

// 控制整个游戏流程
public class MissionDemolition : MonoBehaviour
{
    public static MissionDemolition S; // 单例

    public GameObject[] castles; // 所有城堡
    public Text gtLevel; // UI文本，关卡，格式为“ x of y level ”， x是当前关卡序号，y是总关卡数
    public Text gtScore; // UI文本，分数
    public Vector3 castlePos; // 城堡位置

    public bool _____; // 分隔符

    public int level; // 关卡序号
    public int levelMax; // 最大关卡数
    public int shotsTaken; // 射击的次数
    public GameObject castle; // 城堡
    public GameMode mode = GameMode.idle; // 初始的游戏模式
    public string showing = "SlingShot"; // 当前视图，摄像机聚焦在哪

    private void Start()
    {
        S = this;
        level = 0;
        levelMax = castles.Length;
        StartLevel();
    }

    void StartLevel()
    {
        // 删除旧的castle
        if (castle != null)
        {
            Destroy(castle);
        }
        // 找到旧的弹球并删掉
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Projectile");
        foreach(GameObject pTemp in gos)
        {
            Destroy(pTemp);
        }

        // 新创建一个城堡，改变城堡的位置，重置射击数
        castle = Instantiate<GameObject>(castles[level]);
        castle.transform.position = castlePos;
        shotsTaken = 0;

        // 切换到默认聚焦点，这个时候能看到弹弓和城堡，并删除旧线条
        SwitchView("Both");
        ProjectileLine.S.Clear();

        // 目标默认是没有被击中的，显示UI，切换成游玩中模式
        Goal.goalMet = false;
        ShowGT();
        mode = GameMode.playing;
    }

    /// <summary>
    /// 显示UI文本
    /// </summary>
    void ShowGT()
    {
        gtLevel.text = "Level:" + (level + 1) + "of" + levelMax;
        gtScore.text = "Shots Taken: " + shotsTaken;
    }

    private void Update()
    {
        // 显示 UI 文本
        ShowGT();

        // 检测是否满足通关条件，满足就切换至下一关
        if (mode == GameMode.playing && Goal.goalMet)
        {
            mode = GameMode.levelEnd;

            SwitchView("Both");
            Invoke("NextLevel", 2f);
        }
    }

    /// <summary>
    /// 切换至下一关
    /// </summary>
    void NextLevel()
    {
        level++;
        if (level == levelMax)
        {
            level = 0;
        }
        StartLevel();
    }

    void OnGUI()
    {
        // 绘制按钮，分别用于切换聚焦点
        Rect buttonRect = new Rect((Screen.width / 2) - 50, 10, 100, 24);
        switch (showing)
        {
            case "SlingShot":
                if (GUI.Button(buttonRect, "查看城堡"))
                {
                    SwitchView("Castle");
                }
                break;
            case "Castle":
                if(GUI.Button(buttonRect,"查看全部"))
                {
                    SwitchView("Both");
                }
                break;
            case "Both":
                if (GUI.Button(buttonRect, "查看弹弓"))
                {
                    SwitchView("SlingShot");
                }
                break;
        }
    }

    /// <summary>
    /// 切换聚焦点
    /// </summary>
    /// <param name="eView">指定的聚焦点</param>
    public static void SwitchView(string eView)
    {
        S.showing = eView;
        // 根据聚焦点，更改相机的聚焦位置
        switch (S.showing)
        {
            case "SlingShot":
                FollowCam.S.poi = null;
                break;
            case "Castle":
                FollowCam.S.poi = S.castle;
                break;
            case "Both":
                // 在场景中已经创建了一个对象，这个对象刚好在
                FollowCam.S.poi = GameObject.Find("ViewBoth");
                break;
        }
    }

    /// <summary>
    /// 记录射击的次数
    /// </summary>
    public static void ShotFired()
    {
        S.shotsTaken++;
    }
}
