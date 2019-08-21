using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializationCharacterPlayer : MonoBehaviour
{

    public void InitializationPlayer(GameObject selectedCharacter)
    {
        GameObject oldPlayer = GameObject.FindGameObjectWithTag("Player");
        Transform startPoint = GameObject.FindGameObjectWithTag("StartPoint").transform;
        Interface Interface = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Interface>();
        GlobalGameControl globalGameControl = GetComponent<GlobalGameControl>();
        if (oldPlayer != null)
        {
            Destroy(oldPlayer);
        }
        GameObject newPlayer = Instantiate(selectedCharacter, startPoint.position, Quaternion.identity);
        Interface.Player = newPlayer;
        Interface.PlayerScript = newPlayer.GetComponent<Player>();
        Interface.Start();
        globalGameControl.Players[0] = newPlayer; // Запись в массив игрока
    }
}
