using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct DateCharacter
{
    public string Name, Energy, Boost, Respawn; //имя персонажа
    public GameObject UIprefab; // анимированный префаб персонажа из UI элементов
}
public class InitializationOfCharactersInStore : MonoBehaviour
{
    public DateCharacter[] Character; // список всех персонажей в магазине
    public GameObject Element; // ссылка на балванку элемента магазина
    public GameObject[] Elements; // Массив со всеми элементами магазина
    public Transform Parent; // Родительский объект для генерации нового элемента магазина
    public float Index = 185.0f; // индекс смещения элемента в магазине

    private Vector3 elementPosition; // позиция сгенерированного эелемнта магазина
    private GameObject newElement; // ссылка на только что сгенерированный элемент магазина
    private SettingsElementCharacterInStore parameter; // ссылка на настройки сгенерированного элемента магазина

    void Start()
    {
        Element.transform.localScale = new Vector3(1.385641f, 1.385641f, 1.385641f);
        elementPosition = new Vector3(-Index, 19.0f, .0f);
        Elements = new GameObject[Character.Length];
        GenerationElementsInStore();
    }

    void GenerationElementsInStore()
    {
        for(int i = 0; i < Character.Length; i++)
        {
            newElement = Instantiate(Element, Element.transform.position, Quaternion.identity);

            parameter = newElement.GetComponent<SettingsElementCharacterInStore>();
            newElement.transform.parent = Parent;
            newElement.transform.localPosition = elementPosition;
            elementPosition += new Vector3(Index, .0f, .0f);

            Elements[i] = newElement;
            parameter.Id = i;
            parameter.Name = Character[i].Name;
            parameter.Energy = Character[i].Energy;
            parameter.Boost = Character[i].Boost;
            parameter.Respawn = Character[i].Respawn;
            parameter.Apply();
        }
        Destroy(Element);
    }
}
