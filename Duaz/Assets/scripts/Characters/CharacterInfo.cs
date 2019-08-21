using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInfo : MonoBehaviour
{
    public int Id;
    public string Name;
    public Sprite icon;
    public Vector2 RangeSpeed, // Минимальная и максимальная скорость персонажа
        RangeEnegry; // Минимальная и максимальная энергия персонажа
}
