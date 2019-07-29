using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Скрипт инициализации соперников полсе нажатия кнопки "играть"*/
public class InitializationRival : MonoBehaviour {

    public Transform[] Treadmills = new Transform[4]; // Ссылки на начальные позиции беговых дорожек (должны располагаться за кадром)
    public GameObject[] ListCharacters; // Список высех персонажей
    public GameObject LoadingUIObject; // Объект с анимацией загрузки
    public bool isInternetConnection;
    public bool[] isDoneRivels; // Все ли соперники готовы к забегу
    public GlobalGameControl globalGameControl; // Скрипт общего контроле над игрой

    private Player Player; // Ссылка на персонажа игрока, который находится на старте
    private int PlayerId; // Id персонажа игрока, чтобы не генерировать таких же соперников
    private string listOfAvailableIndex; // Список index'ов  из массива доступных для инициализации персоажей;
    private Rival rivalComponent; // Нужен для присвоение параметров при создании соперника

    // Инициализация соперников для забега
    public void InstantiateRival()
    {
        globalGameControl = GetComponent<GlobalGameControl>();
        globalGameControl.SearchPlayer();
        LoadingUIObject.SetActive(true);
        // Поиск персонажа игрока по тэгу и получение эго ID
        Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        PlayerId = Player.Id;

        // Создание списка всех доступных Id персонажей
        CreateListAllIndex();

        StartCoroutine(CreatingAnOpponentOnTheSecondTreadmill(2, "Line2", false));

        if (isInternetConnection)
        {
            isDoneRivels = new bool[3];
            StartCoroutine(CreatingAnOpponentOnTheSecondTreadmill(1, "Line1", true));
            StartCoroutine(CreatingAnOpponentOnTheSecondTreadmill(4, "Line4", true));
        }
        else
        {
            isDoneRivels = new bool[1];
        }
    }

    // Создание списка всех доступных Id персонажей для инициализации как соперников
    private void CreateListAllIndex()
    {
        listOfAvailableIndex = "";
        for (int i = 0; i < ListCharacters.Length; i++)
        {
            // Запись всех Id за исключение Id текущего персонажа игрока
            if (i != PlayerId && ListCharacters[i] != null)
            {
                listOfAvailableIndex += i + ":";
            }
        }
    }

    private IEnumerator CreatingAnOpponentOnTheSecondTreadmill(int idTreadmill, string sortingLayerName, bool isDelay)
    {
        if (isDelay)
        {
            yield return new WaitForSeconds(Random.Range(1.0f, 2.5f));
        }
        idTreadmill -= 1; // Приведение номера беговой дорожки к id этой же дорожки в массиве
        // Разделить строку даступных index'ов на отдельные символы
        string[] indexes = listOfAvailableIndex.Split(char.Parse(":"));
        // Случайный индекс соперника
        var rndIndex = Random.Range(0, indexes.Length-1);
        var rival = Instantiate(ListCharacters[int.Parse(indexes[rndIndex])], Treadmills[idTreadmill].position, Quaternion.identity);
        Destroy(rival.GetComponent<Player>());
        rivalComponent = rival.GetComponent<Rival>();
        // Получение позиции стартовой точки, до которой соперник должен идти
        rivalComponent.StartPoint = Treadmills[idTreadmill].GetChild(0).transform;
        rivalComponent.initializationRival = GetComponent<InitializationRival>();
        rivalComponent.NumTeadmills = idTreadmill;
        // Удаление из списка доступных для генерации персонаже и обновление списка
        ListCharacters[int.Parse(indexes[rndIndex])] = null;
        // Заполнение массива всеми игроками на беговой дороге
        for (int i = 0; i < globalGameControl.Players.Length; i++)
        {
            if (globalGameControl.Players[i] == null)
            {
                globalGameControl.Players[i] = rival;
                break;
            }
        }
        CreateListAllIndex();
        // Всем дочерним объектам персонажа, котороые содержат комнонент SriteRendere обновить слой в соответствии с номером бегвоой дорожки
        foreach(SpriteRenderer child in rival.GetComponentsInChildren<SpriteRenderer>(true))
        {
            child.sortingLayerName = sortingLayerName;
        }
    }
}
