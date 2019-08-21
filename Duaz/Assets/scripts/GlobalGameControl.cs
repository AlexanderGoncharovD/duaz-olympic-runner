using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalGameControl : MonoBehaviour {

    public GameObject[] Characters; // Персонажи доступные для игры
    public Vector2Int MinMaxNumberOfOpponents; // Минимальное и максимальное количестов противников
    public bool isSoloGame; // Режим одиночной игры
    public GameObject[] Players = new GameObject[4]; // Все игроки
    public string idCharacters; // Id персонажей доступных для игры
    public float PlayersReadiness; // Готовность игроков начинать игру (когда все прибежали на старт равно 1.0f)

    private int NumberOfOpponents; // Количество создоваемых противников
    private bool isInternet; // Доступен ли интернет
    private Opponent opponent; // Ссылка на общий класс противнка
    private Parallax parallax; // ССылка на класс параллакса
    private Lobby lobby; // Ссылка на класс лобби
    private float SinglePlayerReadiness; // Готовность одного игрока (рассчитывается исходя из общего количество игроков)
    private MenuInterface menuInterface;
    private GameInterface gameInterface;
    private CameraLookAtPlayer cameraLook;

    private void Awake()
    {
        var idSelectedCharacter = PlayerPrefs.GetInt("SelectedCharacter");
        // Перебор всего массива и сравнение сохранённого id с id персонажа из массива
        for (int index = 0; index < Characters.Length; index++)
        {
            if (Characters[index].GetComponent<CharacterInfo>().Id == idSelectedCharacter)
            {
                // Инициализация сохранённого персонажа по его id
                GetComponent<InitializationCharacterPlayer>().InitializationPlayer(Characters[index]);
                break;
            }
        }
    }

    private void Start()
    {
        opponent = GetComponent<Opponent>();
        parallax = GetComponent<Parallax>();
        lobby = GetComponent<Lobby>();
        menuInterface = GetComponent<MenuInterface>();
        gameInterface = GetComponent<GameInterface>();
        cameraLook = GetComponent<CameraLookAtPlayer>();
    }

    // Действия после нажатия кнопки "Играть/Play"
    public void Play()
    {
        if (isSoloGame)
        {

        }
        else
        {
            isInternet = IsInternetConnection(); // Обновить данные о подключении к интернету

            /* Если есть интернет, то доступно максимальное количество противников
             * Если интернета нет, то доступно минимальное количество соперников*/
            if (isInternet)
                NumberOfOpponents = MinMaxNumberOfOpponents.y;
            else
                NumberOfOpponents = MinMaxNumberOfOpponents.x;

            /*Рассчёт готовности одного игрока в процентах. Количество противников + игрок.
             * Используется для обозначения готовности всех игроков (Все оин прибежали на стартувую линию)*/
            SinglePlayerReadiness = 1.0f / (NumberOfOpponents + 1);
            ReadyForAnotherPlayer();// Прибавляется одна часть сразу, так как игрок уже готов и находится на стартовой линии

            // Получить строку всех id персонажей, доступных для игры
            idCharacters = GetIdCharacters(Characters);

            // Для каждого слота в лобби выбор персонажа (Кроме игрока, он загружается в Awake или выбирается в ручную игроком)
            for (int numberOpponent = 1; numberOpponent <= NumberOfOpponents; numberOpponent++)
            {
                // Выбрать персонажа, с учётом доступных id для создания, из массива всех персонажей 
                Players[numberOpponent] = opponent.Create(numberOpponent, opponent.SelectCharacter(idCharacters, Characters));
            }
            lobby.Create(NumberOfOpponents, isInternet, Players); // Открыть лобби на нужное количество соперников
            parallax.Activate(); // Активация эффекта парллакаса
        }
    }

    // Проверка подключения к интернету
    private bool IsInternetConnection()
    {
        return true;
    }

    // Получить строку всех id персонажей, доступных для игры
    private string GetIdCharacters(GameObject[] characters)
    {
        var ids = "";
        var idSelectedCharacterPlayer = PlayerPrefs.GetInt("SelectedCharacter");
        // Если массив не путой
        if (characters.Length > 0)
        {
            // Получение Id персонажей из их номера индекса в массиве
            for (int index = 0; index < characters.Length; index++)
            {
                // Получение всех id персонажей, кроме id персонажа игрока
                if (characters[index].GetComponent<CharacterInfo>().Id != idSelectedCharacterPlayer)
                {
                    ids += characters[index].GetComponent<CharacterInfo>().Id + ":";
                }
            }
        }
        else
        {
            Debug.LogError("GlobalGameControl.cs: WARNING GetIdCharacters(), Array 'Characters[]' = null");
        }
        return ids;
    }

    public void CancelTheGame()
    {
        lobby.Close();
    }

    // Таймер отсчитвающий смену позиции персонажа на старте
    private IEnumerator CountdownToStart()
    {
        yield return new WaitForSeconds(1.0f);
        PlayersReadiness += 1;
        NextStartumPosition(Mathf.RoundToInt(PlayersReadiness));
    }

    // Сменить позицию персонажей на следующиую (на линии старта)
    private void NextStartumPosition(int numberPosition)
    {
        for (int index = 0; index < Players.Length; index++)
        {
            if (Players[index] != null)
            {
                Players[index].GetComponent<AnimationController>().NextStartumPosition(numberPosition);

                // Если это последняя поза перед стартом
                if (numberPosition == 3)
                {
                    // Процедура начала забега для каждого персонажа
                    Players[index].GetComponent<MoveController>().StartRun();

                    // Если это Игрок
                    if (index == 0)
                    {
                        // Назначить трансформ игрока и Активировать слеженеи камеры за игроком
                        cameraLook.Player = Players[index].transform;
                        cameraLook.Activate();
                    }
                }
            }
        }
        // Если это не последняя поза перед стартом
        if (numberPosition != 3)
        {
            StartCoroutine(CountdownToStart());
        }
    }

    // Когда на линию старта прибежал ещё один игрок
    public void ReadyForAnotherPlayer()
    {
        // Дополнить общую готовность к игре
        PlayersReadiness += SinglePlayerReadiness;

        //Если все готовы, то начать игру
        if (PlayersReadiness == 1.0f)
        {
            //lobby.Hide();
            menuInterface.Hide();
            gameInterface.Show();
            // Встать в стартовую позу
            NextStartumPosition(Mathf.RoundToInt(PlayersReadiness));
        }
    }
}
