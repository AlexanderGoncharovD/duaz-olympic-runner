using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

struct PlayerInfo
{
    public string name;
    public Sprite icon;
}
[System.Serializable]
public struct ElementExample
{
    public GameObject Element,
        Loading,
        Waiting,
        Ready;
    public Image IconSprite;
    public Text Name;
}

public class Lobby : MonoBehaviour
{
    public GameObject LobbyWindow; // Ссылка на объект интерфейса лобби
    public ElementExample[] Players; // Пример слота для одного игрока
    public float DistanceToNextElement = 65.0f; // Рассточние до следующей ячейки игрока
    public GameObject WarningNoInternetConnectoin; // Текст - предупреждение, что нет подключение к интернету

    private string[] names; // Имена игроков из GP
    private GameObject[] elements; // Элементы игроков
    private Opponent opponent; // Ссылка на скрипт
    private GlobalGameControl globalGameControl;

    private void Start()
    {
        opponent = GetComponent<Opponent>();
        globalGameControl = GetComponent<GlobalGameControl>();
    }

    // Открыть лобби и создать ячейки для игроков
    public void Create(int numberOfOpponents, bool isInternet, GameObject[] players)
    {
        var numberPlayers = numberOfOpponents + 1;
        names = new string[numberPlayers]; // Создать массив с именами игроков
        elements = new GameObject[numberPlayers];

        // Есть или нет интернета скрыть или показать предупреждение
        WarningNoInternetConnectoin.SetActive(!isInternet);

        if (names.Length > 1) // Если количество слотов для имён больше 1
        {
            if (isInternet)
            {
                for (int numberElement = 0; numberElement < elements.Length; numberElement++)
                {
                    // Добавление контента элементам лобби (икноки, имена игроков)
                    if (numberElement == 0)
                    {
                        Players[numberElement].IconSprite.sprite = players[numberElement].GetComponent<CharacterInfo>().icon;
                    }
                    else if (numberElement == elements.Length-1)
                    {
                        StartCoroutine(ActivateOpponent(numberElement, Random.Range(1.0f, 4.0f), players, true));
                    }
                    else
                    {
                        StartCoroutine(ActivateOpponent(numberElement, Random.Range(1.0f, 4.0f), players, false));
                    }
                }
            }
            else
            {
                names[0] = "You";
                names[1] = "Bot";
                Players[2].Element.SetActive(false);
                Players[3].Element.SetActive(false);
            }
        }
        else
        {
            Debug.LogError("Lobby.cs Open(): Number players < 1");
        }

        LobbyWindow.SetActive(true); // Открытие интерфеса "Окно лобби"
    }

    // Процедура ожидания создания противника
    private IEnumerator ActivateOpponent(int index, float timer, GameObject[] players, bool hideLobby)
    {
        Players[index].Waiting.SetActive(true); // Включить обозначение ожидания
        Players[index].Ready.SetActive(false);// Включить обозначение готовности
        Players[index].IconSprite.gameObject.SetActive(false); // Отключение иконки персонажа
        Players[index].Loading.SetActive(true); // Анимация ожидания

        yield return new WaitForSeconds(timer);
        Players[index].IconSprite.sprite = players[index].GetComponent<CharacterInfo>().icon;
        Players[index].IconSprite.gameObject.SetActive(true); // Включение иконки персонажа

        Players[index].Waiting.SetActive(false); // Выключить обозначние ожидания
        Players[index].Ready.SetActive(true);// Включить обозначение готовности
        Players[index].Loading.SetActive(false); // Отключение анимации ожидания

        // Активировать противника, чтобы он бежал к стартовой линии
        players[index].GetComponent<ArtificialIntelligence>().RunToTheStartingLine();

        // Если заспаунен последний персонаж, то скрываем лобби
        if (hideLobby)
        {
            yield return new WaitForSeconds(0.5f);
            Hide();
        }
    }

    // Закрыть лобби и удалить ячейки для игроков
    public void Close()
    {
        for (int index = 1; index < globalGameControl.Players.Length; index++)
        {
            if (globalGameControl.Players[index] != null)
            {
                Destroy(globalGameControl.Players[index]);
            }
        }
        // Включение ограничительных коллайдеров на стартовой линии
        foreach (GameObject startpoint in GameObject.FindGameObjectsWithTag("OpponentStartPoint"))
        {
            startpoint.GetComponent<BoxCollider>().enabled = true;
        }

        //Выключение окна лобби
        LobbyWindow.SetActive(false);
    }

    // Скрыть окно лобби, когда началсь игра
    public void Hide()
    {
        LobbyWindow.SetActive(false);
    }
}
