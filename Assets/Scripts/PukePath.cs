using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PukePath : MonoBehaviour
{
    public GameObject TrajectoryPointPrefab;
    Player player;

    float power = 1.5f;

    int numOfTrajectoryPoints = 30;
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
        setTrajectoryPoints(player.GetPukePoint(), vel / 1);
    }

    public Vector2 GetForceFrom(Vector3 fromPos, Vector3 toPos)
    {
        return (new Vector2(toPos.x, toPos.y) - new Vector2(fromPos.x, fromPos.y)) * power;
    }

    void setTrajectoryPoints(Vector3 pStartPosition, Vector3 pVelocity)
    {
        float velocity = Mathf.Sqrt((pVelocity.x * pVelocity.x) + (pVelocity.y * pVelocity.y));
        float angle = Mathf.Rad2Deg * (Mathf.Atan2(pVelocity.y, pVelocity.x));
        float fTime = 0;

        fTime += 0.03f;
        for (int i = 0; i < numOfTrajectoryPoints; i++)
        {
            float dx = velocity * fTime * Mathf.Cos(angle * Mathf.Deg2Rad);
            float dy = velocity * fTime * Mathf.Sin(angle * Mathf.Deg2Rad) - (Physics2D.gravity.magnitude/2 * fTime * fTime / 2.0f);
            Vector3 pos = new Vector3(pStartPosition.x + dx, pStartPosition.y + dy, 2);
            trajectoryPoints[i].transform.position = pos;
            trajectoryPoints[i].GetComponentInChildren<SpriteRenderer>().enabled = true;
            trajectoryPoints[i].transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(pVelocity.y - (Physics.gravity.magnitude) * fTime, pVelocity.x) * Mathf.Rad2Deg);
            fTime += 0.03f;
        }
    }
}
