using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventsButtons : MonoBehaviour {

	public void Play()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<Animator>().enabled = true;
        player.GetComponent<CameraLookAtPlayer>().enabled = true;
    }
}
