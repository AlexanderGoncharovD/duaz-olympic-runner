using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudGeneration : MonoBehaviour
{
    public Sprite[] Clouds; // Спрайты облаков

    [Header("Settings")]
    public Transform Chunk; // отправная точка спавна
    public Vector2 LocationSize = new Vector2(30, 5); // длина локации
    public Vector2 DenyMinMaxX = new Vector2(2.0f, 5.0f); // плотность спавна облаков
    public Vector2 DenyMinMaxY = new Vector2(2.0f, 5.0f); // плотность спавна облаков

    [Header("Other")]
    public GameObject prefabEmptySprite; // пустая болванка для спрайта
    public Transform ParentDecoration; // Родительский объект для декораций
    GameObject Layer5;  // группировка декорация по номеру слоя

    void Start () {
        // Нахождение/создание слоя для параллакса
        Layer5 = GameObject.FindGameObjectWithTag("Layer5");
        if (Layer5 == null)
        {
            Layer5 = new GameObject();
            Layer5.tag = Layer5.name = "Layer5";
        }

        CloudGenerationGO();
    }

    void CloudGenerationGO ()
    {
        GameObject newCloud;

        for(float i = 0; i < LocationSize.x; i += Random.Range(DenyMinMaxX.x, DenyMinMaxX.y))
        {
            newCloud = Instantiate(prefabEmptySprite, new Vector3(Chunk.position.x + i, Chunk.position.y
                + Random.Range(DenyMinMaxY.x, DenyMinMaxY.y)), Quaternion.identity);
            newCloud.transform.parent = Layer5.transform;
            newCloud.GetComponent<SpriteRenderer>().sprite = Clouds[Random.Range(0, Clouds.Length)];
            newCloud.GetComponent<SpriteRenderer>().sortingOrder = -132;

        }
    }
}
