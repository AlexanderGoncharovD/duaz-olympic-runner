using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubbishGeneration : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite[] SmallRubbish; // мусор ближе к поверхности
    public Sprite[] MediumRubbish; // мусор среднего размера
    public Sprite[] BigRsubbish; // большой мусор

    [Header("Settings")]
    public Transform ChunkSmallRubbish; // отправная точка спавна для мелкого мусора
    public Transform ChunkMediumAndBigRubbish; // отправная точка спавна для среднего и крупного мусора
    public Vector2 ChunkSize = new Vector2(12.0f, 0.3f); //  размеры чанка
    public Vector2Int SmallDenyMinMax = new Vector2Int(2, 5); // Количество спавна мелкого мусора
    public Vector2Int MediumAndBigDenyMinMaxY = new Vector2Int(1, 3); // Количество спавна мелкого мусора
    int NumberOfChanks; // Количество чанков на всю длину

    [Header("Other")]
    public float LengthLocation = 18; // Длина всей локации
    public int ZeroLayer = 20; // слой для муосра
    public GameObject prefabEmptySprite; // пустая болванка для спрайта
    public Transform ParentDecoration; // Родительский объект для декораций
    Transform Layer1;

    // Use this for initialization
    void Start () {
        GameObject[] grounds = GameObject.FindGameObjectsWithTag("Ground");
        Layer1 = GameObject.FindGameObjectWithTag("Layer1").transform;
        NumberOfChanks = Mathf.CeilToInt(LengthLocation / ChunkSize.x); // Количество чанков на всю длину
        RubbishGenerationGO();
	}

    void RubbishGenerationGO()
    {
        int NumberSmallRuddish, NumberMediumAndBigRuddish; // Количество мелкого мусора в одном чанке
        GameObject newRuddish; // новый сгенерированный мусор
        // генерация мелкого мусора
        for (int i = 0; i < NumberOfChanks; i++)
        {
            NumberSmallRuddish = Random.Range(SmallDenyMinMax.x, SmallDenyMinMax.y);
            for (int k = 0; k < NumberSmallRuddish; k ++)
            {
                newRuddish = CreateRubbish(SmallRubbish[Random.Range(0, SmallRubbish.Length)], ChunkSmallRubbish,
                    new Vector3(0, 0, Random.Range(-180, 180)));
                newRuddish.name = "SmallRubbish_" + i + "_" + k;
            }
            ChunkSmallRubbish.position = new Vector3(ChunkSmallRubbish.position.x + ChunkSize.x, ChunkSmallRubbish.position.y, 0);
        }
        // Генерация больших объектов
        for (int i = 0; i < NumberOfChanks; i++)
        {
            NumberMediumAndBigRuddish = Random.Range(MediumAndBigDenyMinMaxY.x, MediumAndBigDenyMinMaxY.y);
            for (int k = 0; k < NumberMediumAndBigRuddish; k++)
            {
                if (Random.Range(0.0f, 1.0f) > 0.3f)
                {
                    newRuddish = CreateRubbish(MediumRubbish[Random.Range(0, MediumRubbish.Length)], ChunkMediumAndBigRubbish,
                         new Vector3(0, 0, 0));
                    newRuddish.name = "MediumRubbish_" + i + "_" + k;
                }
                else
                {

                }
            }
            ChunkMediumAndBigRubbish.position = new Vector3(ChunkMediumAndBigRubbish.position.x + ChunkSize.x, ChunkMediumAndBigRubbish.position.y, 0);
        }
    }

    /*Создание нового объекта с указанным спрайтом, позицией чанка в пространстве и углом поворота*/
    GameObject CreateRubbish(Sprite sprite, Transform chunk, Vector3 rotation)
    {
        Vector2 position = new Vector2(chunk.position.x + Random.Range(0.0f, ChunkSize.x),
            chunk.position.y + Random.Range(0.0f, ChunkSize.y));
        GameObject newObject = Instantiate(prefabEmptySprite, position, Quaternion.Euler(rotation));

        newObject.GetComponent<SpriteRenderer>().sprite = sprite;
        newObject.GetComponent<SpriteRenderer>().sortingOrder = ZeroLayer;
        newObject.transform.parent = Layer1;

        return newObject;
    }
}
