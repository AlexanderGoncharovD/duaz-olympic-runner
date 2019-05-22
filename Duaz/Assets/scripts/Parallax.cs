using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Скрипт отвечате за перемещение слоев векораций на сцене. Создание паралакс эффекта*/
public class Parallax : MonoBehaviour
{
    public GameObject[] Layers = new GameObject[5]; // Список всех слоев на сцене с тегом "layer"
    public float SpeedIndex = 0.1f; //индекс разности смещения слоев для паралакса
    float SpeedMoveCamera;

	// Use this for initialization
	void Start () {
        Layers[0] = GameObject.FindGameObjectWithTag("Layer1");
        Layers[1] = GameObject.FindGameObjectWithTag("Layer2");
        Layers[2] = GameObject.FindGameObjectWithTag("Layer3");
        Layers[3] = GameObject.FindGameObjectWithTag("Layer4");
        Layers[4] = GameObject.FindGameObjectWithTag("Layer5");
    }

    // Update is called once per frame
    void Update()
    {
        SpeedMoveCamera = GetComponent<CameraLookAtPlayer>().SpeedMoveCamera;
        Layers[0].transform.position += Vector3.left * SpeedMoveCamera / SpeedIndex * 1.8f * Time.deltaTime;
        Layers[2].transform.position -= Vector3.left * SpeedMoveCamera / SpeedIndex * 0.75f * Time.deltaTime;
        Layers[3].transform.position -= Vector3.left * SpeedMoveCamera / SpeedIndex * 1.75f * Time.deltaTime;
        Layers[4].transform.position -= Vector3.left * SpeedMoveCamera / SpeedIndex * 0.5f * Time.deltaTime;
    }
}
