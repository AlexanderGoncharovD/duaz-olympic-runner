using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventsButtons : MonoBehaviour {

    public GameObject GameInterface,
        MenuInterface;
    public InitializationRival initializationRival;

	public void Play()
    {
        GameInterface.SetActive(true);
        MenuInterface.SetActive(false);
        initializationRival.StartCoroutine("InstantiateRival");
    }
}
