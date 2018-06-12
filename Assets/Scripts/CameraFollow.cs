using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smooth;
    public Transform bossPos;
    public Vector3 offset;
    public bool boss;

    void LateUpdate ()
    {
        if(!boss)
        {
            Vector3 desiredPos = target.position + offset;
            Vector3 smoothPos = Vector3.Lerp(transform.position, desiredPos, smooth * Time.deltaTime);
            this.transform.position = smoothPos;
        }
        else
        {
            Vector3 desiredPos = bossPos.position + new Vector3(0, 0, -10);
            Vector3 smoothPos = Vector3.Lerp(transform.position, desiredPos, smooth * Time.deltaTime / 2);
            this.transform.position = smoothPos;
        }

    }

    public void BossPosition()
    {
        boss = true;
        GetComponent<PerfectPixelCamera>().TexturePixelsPerWorldUnit = 60;
    }

}
