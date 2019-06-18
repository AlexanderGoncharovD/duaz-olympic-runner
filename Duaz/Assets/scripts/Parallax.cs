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
        Layers[0] = new GameObject();
        Layers[0].name = Layers[0].tag = "Layer3";
        Layers[1] = new GameObject();
        Layers[1].name = Layers[1].tag = "Layer4";
        Layers[2] = new GameObject();
        Layers[2].name = Layers[2].tag = "Layer5";

        GameObject[] layer3 = GameObject.FindGameObjectsWithTag("Background_1");
        foreach(GameObject Object in layer3)
        {
            Object.transform.parent = Layers[0].transform;
        }

        GameObject[] layer4 = GameObject.FindGameObjectsWithTag("Background_2");
        foreach (GameObject Object in layer4)
        {
            Object.transform.parent = Layers[1].transform;
        }

        GameObject[] layer5 = GameObject.FindGameObjectsWithTag("Cloud");
        foreach (GameObject Object in layer5)
        {
            Object.transform.parent = Layers[2].transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        SpeedMoveCamera = GetComponent<CameraLookAtPlayer>().SpeedMoveCamera;
        Layers[0].transform.position -= Vector3.left * SpeedMoveCamera / SpeedIndex * 0.5f * Time.deltaTime;
        Layers[1].transform.position -= Vector3.left * SpeedMoveCamera / SpeedIndex * 1.5f * Time.deltaTime;
        Layers[2].transform.position -= Vector3.left * SpeedMoveCamera / SpeedIndex * 2.5f * Time.deltaTime;
    }
}
