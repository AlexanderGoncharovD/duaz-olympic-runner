using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationGeneration : MonoBehaviour
{
    [Header("Settings location")]
    public GameObject LocationObject; // Объект локации для клонирования
    public Transform NextPoint; // Точка спавна следующией копии этой локации
    public int LengthLocation = 200;

    [Header("Other")]
    public int CountLocationGenerated; // Сколько должно быть сгенерировано локаций
    public float DistanceToNextSpawn = 18; // Дистанция до следующей точки спавна локации

    // Use this for initialization
    void Start () {

        CountLocationGenerated = Mathf.CeilToInt(LengthLocation / DistanceToNextSpawn);

        for (int i = 1; i <= CountLocationGenerated; i++)
        {
            Instantiate(LocationObject, NextPoint.localPosition, Quaternion.identity);
            NextPoint.localPosition += new Vector3(18, 0, 0);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
