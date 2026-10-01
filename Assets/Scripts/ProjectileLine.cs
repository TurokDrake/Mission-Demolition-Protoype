using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLine : MonoBehaviour
{
    public static ProjectileLine S; // 单例
    public float minDist = 0.1f;

    public bool _____; // 通常用这个分隔成员，在此之前的会在检视面板中调整，在此之后的会通过代码动态调整

    public LineRenderer line;
    private GameObject _poi;
    public List<Vector3> points;

    private void Awake()
    {
        S = this;
        line = GetComponent<LineRenderer>();
        line.enabled = false;
        points = new List<Vector3>();
    }

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
        if (poi == null)
        {
            if (FollowCam.S.poi != null)
            {
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
        Vector3 pt = _poi.transform.position;
        if (points.Count > 0 && (pt - lastPoint).magnitude < minDist)
        {
            return;
        }

        if (points.Count == 0)
        {
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
