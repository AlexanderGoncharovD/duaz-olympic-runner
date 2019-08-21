using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInterface : MonoBehaviour
{
    public GameObject Interface; // Родительский объект всего интерфейса в игре

    public void Show()
    {
        Interface.SetActive(true);
    }

    public void Hide()
    {
        Interface.SetActive(false);
    }
}
