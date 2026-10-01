using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudCrafter : MonoBehaviour
{
    public int numClouds = 40; // Number of cloud.
    public GameObject[] cloudPrefabs;
    public Vector3 cloudPosMin;
    public Vector3 cloudPosMax;
    public float cloudScaleMin = 1f;
    public float cloudScaleMax = 5f;
    public float cloudSpeedMult = 0.5f;
    public bool __________;

    public GameObject[] cloudInstances;

    private void Awake()
    {
        cloudInstances = new GameObject[numClouds];
        GameObject anchor = GameObject.Find("CloudAnchor");
        GameObject cloud;
        for(int i = 0; i < numClouds; i++)
        {
            // Instantiate cloud randomly.
            int prefabNum = Random.Range(0, cloudPrefabs.Length);
            cloud = Instantiate<GameObject>(cloudPrefabs[prefabNum]);

            // Set position of cloud.
            Vector3 cPos = Vector3.zero;
            cPos.x = Random.Range(cloudPosMin.x, cloudPosMax.x);
            cPos.y = Random.Range(cloudPosMin.y, cloudPosMax.y);

            // Set scale of cloud.
            float scaleU = Random.value;
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
        foreach (GameObject cloud in cloudInstances)
        {
            float scaleVal = cloud.transform.localScale.x;
            Vector3 cPos = cloud.transform.position;

            cPos.x -= scaleVal * Time.deltaTime * cloudSpeedMult;

            
            if (cPos.x <= cloudPosMin.x)
            {
                cPos.x = cloudPosMax.x;
            }

            // Update cloud's position.
            cloud.transform.position = cPos;
        }
    }
}
