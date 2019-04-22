using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.Multiplayer;
using UnityEngine.SceneManagement;

public class Authorization : MonoBehaviour, RealTimeMultiplayerListener
{
    public GameObject textLoadAuthorization, textErrorLoadAuthorization, textHostIdentificator;
    public GameObject buttonSignIn, buttonSignOut, buttonStartGame, buttonLeaveGame;
    public GameObject boxSelectCharacter, boxCharacterIcon;
    public Image iconBoxCharacter;
    public bool isPlayerLoggedInGPS = false, // если игрок авторизаван в GPS
                isCharacterSelected = false, // если персонаж выбран
                isHost = false; // если этот игрок является хостом
    public Sprite[] allSpritesCharacters = new Sprite[16];
    public int idSelectedCharacter;
    public Transform[] pointSpawnPlayers;
    public GameObject patternPlayer;

    public GameObject player, opponent;

    private const int minOpponents = 1 , maxOpponents = 3;
    private string[] idPlayersGPS; // id игроков комнаты
    private string myId;
    private GenerationLocation generationLocation;

    [Space][Header("Debugging")]
    public Text debugText;

    void Awake ()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();
    }

    void Start()
    {
        SignInGPS(); // попытка авторизавть игрока в GPS
        generationLocation = GetComponent<GenerationLocation>();
    }

	public void SignInGPS () // Авторизация игрока в GPS
    {
        textLoadAuthorization.SetActive(true);
        Social.localUser.Authenticate((bool success) =>
        {
            if (success) // игрок успешно авторизавлся
            {
                textLoadAuthorization.SetActive(false);
                buttonSignOut.SetActive(true);
                isPlayerLoggedInGPS = true;
                OpenBoxSelectCharacter();
;
                debugText.text += "Игрок " + Social.localUser.userName + " авторизован\n"; //  ВЫВОД ЛОГОВ НА ЭКРАН
            }
            else // ошибка авторизации
            {
                textLoadAuthorization.SetActive(false);
                textErrorLoadAuthorization.SetActive(true);
                buttonSignIn.SetActive(true);
                isPlayerLoggedInGPS = false;

                debugText.text += "ошибка авторизации игрока\n"; //  ВЫВОД ЛОГОВ НА ЭКРАН
            }
        });
	}

    public void OpenBoxSelectCharacter ()
    {
        boxSelectCharacter.SetActive(true);
        buttonStartGame.SetActive(false);
    }

    public void SelectIdCharacter (int id)
    {
        idSelectedCharacter = id;
        iconBoxCharacter.sprite = allSpritesCharacters[id];
        boxSelectCharacter.SetActive(false);
        boxCharacterIcon.SetActive(true);
        buttonStartGame.SetActive(true);

        debugText.text += "выбран персонаж id: " + id + "\n"; //  ВЫВОД ЛОГОВ НА ЭКРАН
    }

    public void CreateMultiplayerRoom()
    {
        PlayGamesPlatform.Instance.RealTime.CreateQuickGame(minOpponents, maxOpponents, 0, this);
    }

    public void LeaveMultiplayerRoom()
    {
        PlayGamesPlatform.Instance.RealTime.LeaveRoom();
        SceneManager.LoadScene(0);
    }

	void Update ()
    {
		
	}

    public void OnRoomSetupProgress(float percent)
    {
        debugText.text += "подключение...\n"; //  ВЫВОД ЛОГОВ НА ЭКРАН
    }

    public void OnRoomConnected(bool success)
    {
        if (success)
        {
            buttonStartGame.SetActive(false);
            buttonLeaveGame.SetActive(true);

            debugText.text += "подключение УСПЕШНО\n"; //  ВЫВОД ЛОГОВ НА ЭКРАН

            ReqestIdPlayers();
        }
        else
        {
            debugText.text += "подключение НЕ УДАЛОСЬ\n"; //  ВЫВОД ЛОГОВ НА ЭКРАН
        }
    }

    void ReqestIdPlayers()
    {
        List<Participant> idPlayers = PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants();
        Participant myIdP = PlayGamesPlatform.Instance.RealTime.GetSelf();
        myId = myIdP.ParticipantId;
        debugText.text += myId + " - Я\n"; //  ВЫВОД ЛОГОВ НА ЭКРАН

        idPlayersGPS = new string[idPlayers.Count];
        for (int i = 0; i < idPlayers.Count; i++)
        {
            idPlayersGPS[i] = idPlayers[i].ParticipantId; // запись списка id каждого игрока в комнате

            debugText.text += idPlayers[i].ParticipantId + "\n"; //  ВЫВОД ЛОГОВ НА ЭКРАН
        }

        debugText.text += "определение хоста\n"; //  ВЫВОД ЛОГОВ НА ЭКРАН

        PlayGamesPlatform.Instance.RealTime.SendMessage(
            true, idPlayers[0].ParticipantId, Encoding.UTF8.GetBytes("host"));
        SendIdCharacter();
    }

    public void OnLeftRoom()
    {
        buttonLeaveGame.SetActive(false);
        buttonStartGame.SetActive(true);
        textHostIdentificator.SetActive(false);
        isHost = false;

        debugText.text += "покинул комнату\n"; //  ВЫВОД ЛОГОВ НА ЭКРАН
    }

    public void OnParticipantLeft(Participant participant)
    {
        throw new System.NotImplementedException();
    }

    public void OnPeersConnected(string[] participantIds)
    {
        throw new System.NotImplementedException();
    }

    public void OnPeersDisconnected(string[] participantIds)
    {
        throw new System.NotImplementedException();
    }

    public void OnRealTimeMessageReceived(bool isReliable, string senderId, byte[] data)
    {

        string recivedText = Encoding.UTF8.GetString(data);

        debugText.text += "сообщение получено\n"; //  ВЫВОД ЛОГОВ НА ЭКРАН

        if (recivedText  == "host")
        {
            isHost = true;
            textHostIdentificator.SetActive(true);
            SendPositionAndIdObjects (generationLocation.Generation());
        }
        else
        {
            string[] splitText = recivedText.Split('#');

            if (splitText[0] == "ob")
            {
                for (int i = 1; i < splitText.Length; i++)
                {
                    string[] idAndPos = splitText[i].Split('&');
                    SpawnObjectOnLocation(int.Parse(idAndPos[0]), int.Parse(idAndPos[1]));
                }
                debugText.text += "объекты получены\n"; //  ВЫВОД ЛОГОВ НА ЭКРАН
            }
            else if (splitText[0] == "p")
            {
                ReadMessageIdCharacter(splitText[1]);
                debugText.text += "оппонент получен\n"; //  ВЫВОД ЛОГОВ НА ЭКРАН
            }
        }
    }

    void SendPositionAndIdObjects (string text)
    {
        
        byte[] b = Encoding.UTF8.GetBytes(text);
        debugText.text += "отправка сгенерированных объектов (" + b.Length +" байта)\n"; //  ВЫВОД ЛОГОВ НА ЭКРАН
        PlayGamesPlatform.Instance.RealTime.SendMessageToAll(true, b);
    }

    void SpawnObjectOnLocation(int id, int pos)
    {
        GameObject newObjectOnLocation = (GameObject)Instantiate(generationLocation.patternObject,
                                                                    new Vector3(pos, 1.0f, 0),
                                                                    Quaternion.Euler(0, 0, 0));
        newObjectOnLocation.GetComponent<SpriteRenderer>().sprite = generationLocation.spritesObjects[id];
    }

    public void SendIdCharacter()
    {
        byte[] b = Encoding.UTF8.GetBytes("p#" + idSelectedCharacter);
        PlayGamesPlatform.Instance.RealTime.SendMessageToAll(true, b);

        debugText.text += "Я создан\n"; //  ВЫВОД ЛОГОВ НА ЭКРАН
        player = (GameObject)Instantiate(patternPlayer, pointSpawnPlayers[0].position, Quaternion.Euler(0, 0, 0));
        player.GetComponent<SpriteRenderer>().sprite = allSpritesCharacters[idSelectedCharacter];
    }

    void ReadMessageIdCharacter (string text)
    {
        debugText.text += "получен текст: " + text + "\n"; //  ВЫВОД ЛОГОВ НА ЭКРАН
        SpawnParticipant(int.Parse(text));

    }

    void SpawnParticipant (int idCharacterParticipant)
    {
        debugText.text += "оппонент создан\n"; //  ВЫВОД ЛОГОВ НА ЭКРАН
        opponent = (GameObject)Instantiate(patternPlayer, pointSpawnPlayers[1].position, Quaternion.Euler(0, 0, 0));
        opponent.GetComponent<SpriteRenderer>().sprite = allSpritesCharacters[idCharacterParticipant];
    }

}
