using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLookAtPlayer : MonoBehaviour {

    public Transform Player;
    public Transform Camera;
    public float smooth = 2.0f;
    public bool isLookAt = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (isLookAt)
        {
            Camera.position = new Vector3(Mathf.Lerp(Camera.position.x, Player.position.x, Time.deltaTime * smooth),
                Mathf.Lerp(Camera.position.y, Player.position.y, Time.deltaTime * smooth), -10);
        }
    }
}
