using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opponent : MonoBehaviour
{
    // Позиции точек для создание персонажей
    public Transform[] PointInstantiate = new Transform[4];

    private GlobalGameControl globalGameControl;

    private void Start()
    {
        globalGameControl = GetComponent<GlobalGameControl>();
    }
    
    // Создание персонажа на его линии и за экраном
    public GameObject Create(int numberOpponent, GameObject character)
    {
        var newCharacter = Instantiate(character, PointInstantiate[numberOpponent].position, Quaternion.identity);

        // Каждому спрайту применяется соответсвующая группа слоёв для отображения на нужной линии
        foreach (SpriteRenderer child in newCharacter.GetComponentsInChildren<SpriteRenderer>())
        {
            if (numberOpponent == 1)
            {
                child.sortingLayerName = "Line1";
            }
            else if (numberOpponent == 2)
            {
                child.sortingLayerName = "Line2";
            }
            else if(numberOpponent == 3)
            {
                child.sortingLayerName = "Line4";
            }
        }
        newCharacter.GetComponent<ArtificialIntelligence>().globalGameControl = globalGameControl;
        return newCharacter;      
    }

    // Выбрать персонажа, с учётом доступных id для создания, из массива всех персонажей 
    public GameObject SelectCharacter(string idCharacters, GameObject[] characters)
    {
        var ids = idCharacters.Split(':'); // Массив всех достуных для генерации id
        var selectId = ids[Random.Range(0, ids.Length-1)]; // Выбор случайного id
        var character = characters[0];

        // перезапись строки с доступными id (исключается выбранный id)
        idCharacters = "";
        for (int i = 0; i < ids.Length-1; i++)
        {
            if (ids[i] != selectId)
            {
                idCharacters += ids[i] + ":";
            }
        }
        globalGameControl.idCharacters = idCharacters; // передача доступных id

        // Передаётся выбранный персонаж по id
        for (int i = 0; i < characters.Length; i++)
        {
            if (characters[i].GetComponent<CharacterInfo>().Id == int.Parse(selectId))
            {
                character = characters[i];
            }
        }
        
        return character;
    }
}
