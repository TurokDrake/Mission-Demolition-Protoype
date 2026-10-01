using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameMode
{
    idle,
    playing,
    levelEnd
}

public class MissionDemolition : MonoBehaviour
{
    public static MissionDemolition S;

    public GameObject[] castles;
    public Text gtLevel;
    public Text gtScore;
    public Vector3 castlePos;

    public bool _____;

    public int level;
    public int levelMax;
    public int shotsTaken;
    public GameObject castle;
    public GameMode mode = GameMode.idle;
    public string showing = "SlingShot";

    private void Start()
    {
        S = this;
        level = 0;
        levelMax = castles.Length;
        StartLevel();
    }

    void StartLevel()
    {
        if (castle != null)
        {
            Destroy(castle);
        }
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Projectile");
        foreach(GameObject pTemp in gos)
        {
            Destroy(pTemp);
        }

        castle = Instantiate<GameObject>(castles[level]);
        castle.transform.position = castlePos;
        shotsTaken = 0;

        SwitchView("Both");
        ProjectileLine.S.Clear();

        Goal.goalMet = false;
        ShowGT();
        mode = GameMode.playing;
    }

    void ShowGT()
    {
        gtLevel.text = "Level:" + (level + 1) + "of" + levelMax;
        gtScore.text = "Shots Taken: " + shotsTaken;
    }

    private void Update()
    {
        ShowGT();
        if (mode == GameMode.playing && Goal.goalMet)
        {
            mode = GameMode.levelEnd;

            SwitchView("Both");
            Invoke("NextLevel", 2f);
        }
    }

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

    public static void SwitchView(string eView)
    {
        S.showing = eView;
        switch (S.showing)
        {
            case "SlingShot":
                FollowCam.S.poi = null;
                break;
            case "Castle":
                FollowCam.S.poi = S.castle;
                break;
            case "Both":
                FollowCam.S.poi = GameObject.Find("ViewBoth");
                break;
        }
    }

    public static void ShotFired()
    {
        S.shotsTaken++;
    }
}
