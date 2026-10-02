using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudCrafter : MonoBehaviour
{
    public int numClouds = 40; // 云的数量
    public GameObject[] cloudPrefabs; // 云预制体

    // 限制云出现的范围
    public Vector3 cloudPosMin; 
    public Vector3 cloudPosMax; 

    // 限制云缩放范围
    public float cloudScaleMin = 1f;
    public float cloudScaleMax = 5f;
    
    public float cloudSpeedMult = 0.5f; // 云移动速度

    public bool __________; // 分隔符

    public GameObject[] cloudInstances; // 实例化到场景的云

    private void Awake()
    {
        cloudInstances = new GameObject[numClouds];
        GameObject anchor = GameObject.Find("CloudAnchor");
        GameObject cloud;
        for(int i = 0; i < numClouds; i++)
        {
            // 随机实例化不同形状的云
            int prefabNum = Random.Range(0, cloudPrefabs.Length);
            cloud = Instantiate<GameObject>(cloudPrefabs[prefabNum]);

            // 设置云的位置
            Vector3 cPos = Vector3.zero;
            cPos.x = Random.Range(cloudPosMin.x, cloudPosMax.x);
            cPos.y = Random.Range(cloudPosMin.y, cloudPosMax.y);

            // 让云的大小动态变化
            float scaleU = Random.value; // 返回从0~1的浮点数
            float scaleVal = Mathf.Lerp(cloudScaleMin, cloudScaleMax, scaleU);

            // 较小的云朵离地面近
            cPos.y = Mathf.Lerp(cloudPosMin.y, cPos.y, scaleU);

            // 较大的云朵离地面远
            cPos.z = 100 - 90 * scaleU;

            cloud.transform.position = cPos;
            cloud.transform.localScale = Vector3.one * scaleVal;

            cloud.transform.parent = anchor.transform;

            cloudInstances[i] = cloud;
        }
    }

    private void Update()
    {
        // 每一帧检测云的位置，让云动起来
        foreach (GameObject cloud in cloudInstances)
        {
            float scaleVal = cloud.transform.localScale.x;
            Vector3 cPos = cloud.transform.position;

            cPos.x -= scaleVal * Time.deltaTime * cloudSpeedMult;

            
            if (cPos.x <= cloudPosMin.x)
            {
                cPos.x = cloudPosMax.x;
            }

            // 更新云的位置
            cloud.transform.position = cPos;
        }
    }
}
