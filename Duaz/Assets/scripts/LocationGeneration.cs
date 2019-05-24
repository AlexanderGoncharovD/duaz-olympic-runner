using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationGeneration : MonoBehaviour
{
    [Header("Settings location")]
    public GameObject LocationObject; // Объект локации для клонирования
    public GameObject GroundLocationObject; // быстродвижующаяся самая ближняя к камере часть локации
    public Transform NextPoint; // Точка спавна следующией копии этой локации
    public int LengthLocation = 200;

    [Header("Other")]
    public int CountLocationGenerated; // Сколько должно быть сгенерировано локаций
    public float DistanceToNextSpawn = 18; // Дистанция до следующей точки спавна локации
    public Transform ParentDecoration;
    Transform Layer1;
    GameObject Layer3, Layer4;

    // Use this for initialization
    void Start () {

        CountLocationGenerated = Mathf.CeilToInt(LengthLocation / DistanceToNextSpawn);
        GameObject newLocation;
        for (int i = 1; i <= CountLocationGenerated; i++)
        {
            newLocation = Instantiate(LocationObject, NextPoint.localPosition, Quaternion.identity);
            newLocation.transform.parent = ParentDecoration;
            NextPoint.localPosition += new Vector3(18, 0, 0);
        }
        Destroy(LocationObject);
        Destroy(GroundLocationObject);

        Layer3 = new GameObject();
        Layer3.tag = Layer3.name = "Layer3";
        Layer4 = new GameObject();
        Layer4.tag = Layer4.name = "Layer4";

        GameObject[] bg_1 = GameObject.FindGameObjectsWithTag("Background_1");
        for (int i = 0; i < bg_1.Length; i++)
        {
            bg_1[i].transform.parent = Layer3.transform;
        }
        GameObject[] bg_2 = GameObject.FindGameObjectsWithTag("Background_2");
        for (int i = 0; i < bg_2.Length; i++)
        {
            bg_2[i].transform.parent = Layer4.transform;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
