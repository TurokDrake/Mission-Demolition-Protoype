using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLine : MonoBehaviour
{
    public static ProjectileLine S; // 单例
    public float minDist = 0.1f; // 点和点之间的最小距离，min Distance

    public bool _____; // 通常用这个分隔成员，在此之前的会在检视面板中调整，在此之后的会通过代码动态调整

    public LineRenderer line; // 渲染线条的组件
    private GameObject _poi; // 焦点
    public List<Vector3> points; // 点

    private void Awake()
    {
        S = this;
        line = GetComponent<LineRenderer>();
        // 只有我们发射弹丸时，才显示弹丸的轨迹
        line.enabled = false;
        points = new List<Vector3>();
    }

    // 焦点
    public GameObject poi
    {
        get
        {
            return (_poi);
        }
        set
        {
            _poi = value;
            if (_poi != null)
            {
                line.enabled = false;
                points = new List<Vector3>();
                AddPoint();
            }
        }
    }

    // 上一个点
    public Vector3 lastPoint
    {
        get
        {
            if (points == null)
            {
                return (Vector3.zero);
            }
            return points[points.Count - 1];
        }
    }

    private void FixedUpdate()
    {
        // 焦点是否为空
        if (poi == null)
        {
            // 检查相机焦点
            if (FollowCam.S.poi != null)
            {
                // 检查相机焦点是不是弹丸，是的话就设置为轨迹渲染的焦点
                if (FollowCam.S.poi.tag == "Projectile")
                {
                    poi = FollowCam.S.poi;
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }

        AddPoint();
        // 如果弹丸不动了，就不绘制其轨迹了
        if (poi.GetComponent<Rigidbody>().IsSleeping())
        {
            poi = null;
        }
    }

    // 清除线条
    public void Clear()
    {
        _poi = null;
        line.enabled = false;
        points = new List<Vector3>();
    }

    // 添加线条
    public void AddPoint()
    {
        // 获得弹丸的位置
        Vector3 pt = _poi.transform.position;
        // 若有记录点，而且弹丸的位置距离上一个记录点的位置小于我们规定的最小距离，就不添加新点
        if (points.Count > 0 && (pt - lastPoint).magnitude < minDist)
        {
            return;
        }
        // 若在此之前未曾添加过点
        if (points.Count == 0)
        {
            // 获取发射点的位置，并计算发射点到弹丸位置的向量
            Vector3 launchPos = SlingShot.S.launchPoint.transform.position;
            Vector3 launchPosDiff = pt - launchPos;

            points.Add(pt + launchPosDiff);
            points.Add(pt);
            line.positionCount = 2;

            line.SetPosition(0, points[0]);
            line.SetPosition(1, points[1]);

            line.enabled = true;
        }
        else
        {
            points.Add(pt);
            line.positionCount = points.Count;
            line.SetPosition(points.Count - 1, lastPoint);
            line.enabled = true;
        }
    }
}
