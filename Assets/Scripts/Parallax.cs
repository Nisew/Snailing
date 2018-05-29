using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public Transform[] backgrounds;
    float[] parallaxScale;
    public float smooth = 1f;

    Transform cam;
    Vector3 previousCamPos;

    void Awake()
    {
        cam = Camera.main.transform;
    }

	void Start ()
    {
        previousCamPos = cam.position;

        parallaxScale = new float[backgrounds.Length];

        for(int i = 0; i < backgrounds.Length; i++)
        {
            parallaxScale[i] = backgrounds[i].position.z * -1;
        }
	}
	
	void Update ()
    {

        for (int i = 0; i < backgrounds.Length; i++)
        {
            float parallax = (previousCamPos.x - cam.position.x) * parallaxScale[i];

            float backgroundTargetPosX = backgrounds[i].position.x - parallax;

            Vector3 backgroundTargetPos = new Vector3(backgroundTargetPosX, backgrounds[i].position.y, backgrounds[i].position.z);

            backgrounds[i].position = Vector3.Lerp(backgrounds[i].position, backgroundTargetPos, smooth * Time.deltaTime);
        }
        previousCamPos = cam.position;
    }
}
