using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalGameControl : MonoBehaviour {

    public GameObject[] Players = new GameObject[4];
    public bool startGame;
    public GameObject WaitingForRivals;
    public int AnimationStartParameter = 1;

	public void SearchPlayer()
    {
        Players[0] = GameObject.FindGameObjectWithTag("Player");
    }

    public void StartGame()
    {
        WaitingForRivals.SetActive(false);
        Players[0].GetComponent<Animator>().enabled = true;
        Players[0].GetComponent<Animator>().SetInteger("start", AnimationStartParameter);
        Players[0].GetComponent<Animator>().SetBool("run", true);
        Players[0].GetComponent<CameraLookAtPlayer>().enabled = true;
        StartCoroutine("NextNumParameter");
    }
    IEnumerator NextNumParameter()
    {
        yield return new WaitForSeconds(1.0f);
        AnimationStartParameter++;
        for (int i = 0; i < Players.Length; i++)
        {
            Players[i].GetComponent<Animator>().SetInteger("start", AnimationStartParameter);
        }
        yield return new WaitForSeconds(2.0f);
        AnimationStartParameter++;
        for (int i = 0; i < Players.Length; i++)
        {
            Players[i].GetComponent<Animator>().SetInteger("start", AnimationStartParameter);
        }
        Players[0].GetComponent<Player>().AnimationStartAnimSpeedRun();
    }
}
