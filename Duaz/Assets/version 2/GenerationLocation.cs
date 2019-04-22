using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationLocation : MonoBehaviour {

	
    public GameObject patternObject;
    public Sprite[] spritesObjects;

    private GameObject newObject;

    [ContextMenu("Spawn")]
    public string Generation()
    {
        string positionAndIdObjects = "ob#";
        int xPosition, idObject;
        for (int i = 0; i < spritesObjects.Length; i++)
        {
            xPosition = Random.Range(-8, 8);
            idObject = Random.Range(0, spritesObjects.Length);
            newObject = Instantiate(patternObject, new Vector3(xPosition, 1.0f, 0), Quaternion.Euler(0, 0, 0));
            newObject.GetComponent<SpriteRenderer>().sprite = spritesObjects[idObject];
            positionAndIdObjects += idObject + "&" + xPosition + "#";
        }
        return positionAndIdObjects;
    }
}
