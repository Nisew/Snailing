﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PukePath : MonoBehaviour
{
    public GameObject TrajectoryPointPrefab;
    Player player;

    float power = 1.5f;

    int numOfTrajectoryPoints = 15;
    List<GameObject> trajectoryPoints;

    void Start()
    {
        player = GetComponent<Player>();
        trajectoryPoints = new List<GameObject>();

        for (int i = 0; i < numOfTrajectoryPoints; i++)
        {
            GameObject dot = Instantiate(TrajectoryPointPrefab);
            dot.GetComponentInChildren<SpriteRenderer>().enabled = false;
            trajectoryPoints.Add(dot);
        }
    }

    public void DrawProjectileTrajectory()
    {
        Vector2 vel = GetForceFrom(player.GetPukePoint(), Camera.main.ScreenToWorldPoint(Input.mousePosition));
        
        if (player.powered)
        {
            if (vel.x >= 10) vel.x = 10;
            if (vel.y >= 8) vel.y = 8;
            if (vel.x <= -10) vel.x = -10;
            if (vel.y <= -6) vel.y = -6;
        }
        else
        {
            if(vel.x >= 2) vel.x = 2;
            if(vel.y >= 4) vel.y = 4;
            if(vel.x <= -2) vel.x = -2;
            if(vel.y <= -2.5f) vel.y = -2.5f;
        }

        SetTrajectoryPoints(player.GetPukePoint(), vel / 1);
    }

    public Vector2 GetForceFrom(Vector3 fromPos, Vector3 toPos)
    {
        Vector2 vectorToReturn;

        vectorToReturn = (new Vector2(toPos.x, toPos.y) - new Vector2(fromPos.x, fromPos.y)) * power;

        /*if(player.powered)
        {
            vectorToReturn = (new Vector2(toPos.x, toPos.y) - new Vector2(fromPos.x, fromPos.y)) * 5;
        }*/

        return vectorToReturn;
    }

    void SetTrajectoryPoints(Vector3 pStartPosition, Vector3 pVelocity)
    {
        float velocity = Mathf.Sqrt((pVelocity.x * pVelocity.x) + (pVelocity.y * pVelocity.y));
        float angle = Mathf.Rad2Deg * (Mathf.Atan2(pVelocity.y, pVelocity.x));
        float fTime = 0;

        fTime += 0.05f;
        for (int i = 0; i < numOfTrajectoryPoints; i++)
        {
            float dx = velocity * fTime * Mathf.Cos(angle * Mathf.Deg2Rad);
            float dy = velocity * fTime * Mathf.Sin(angle * Mathf.Deg2Rad) - (Physics2D.gravity.magnitude/2 * fTime * fTime / 2.0f);
            Vector3 pos = new Vector3(pStartPosition.x + dx, pStartPosition.y + dy, 2);
            trajectoryPoints[i].transform.position = pos;
            trajectoryPoints[i].GetComponentInChildren<SpriteRenderer>().enabled = true;
            trajectoryPoints[i].transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(pVelocity.y - (Physics.gravity.magnitude) * fTime, pVelocity.x) * Mathf.Rad2Deg);
            fTime += 0.05f;
        }
    }

    public void DesactivateTrajectoryPoints()
    {
        for(int i = 0; i < numOfTrajectoryPoints; i++)
        {
            trajectoryPoints[i].GetComponentInChildren<SpriteRenderer>().enabled = false;
        }
    }
}
