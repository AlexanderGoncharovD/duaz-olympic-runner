using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInterface : MonoBehaviour
{
    public GameObject Interface; // Родительский объект всего интерфейса в меню

    public void Show()
    {
        Interface.SetActive(true);
    }

    public void Hide()
    {
        Interface.SetActive(false);
    }
}
