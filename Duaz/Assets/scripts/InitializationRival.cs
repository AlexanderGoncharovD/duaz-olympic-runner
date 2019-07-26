using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Скрипт инициализации соперников полсе нажатия кнопки "играть"*/
public class InitializationRival : MonoBehaviour {

    public Transform[] Treadmills = new Transform[4]; // Ссылки на начальные позиции беговых дорожек (должны располагаться за кадром)
    public GameObject[] ListCharacters; // Список высех персонажей

    private Player Player; // Ссылка на персонажа игрока, который находится на старте
    private int PlayerId; // Id персонажа игрока, чтобы не генерировать таких же соперников
    private string listOfAvailableId; // Список всех доступных для инициализации Id персоажей;

    // Инициализация соперников для забега
    public void InstantiateRival()
    {
        // Поиск персонажа игрока по тэгу и получение эго ID
        Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        PlayerId = Player.Id;

        // Создание списка всех доступных Id персонажей
        CreateListAllId();
    }

    // Создание списка всех доступных Id персонажей для инициализации как соперников
    private void CreateListAllId()
    {
        for (int i = 0; i < ListCharacters.Length; i++)
        {
            // Запись всех Id за исключение Id текущего персонажа игрока
            if (i != PlayerId)
            {
                listOfAvailableId += ListCharacters[i].GetComponent<Player>().Id + ":";
            }
        }
    }
}
