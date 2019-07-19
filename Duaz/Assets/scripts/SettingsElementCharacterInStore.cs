using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsElementCharacterInStore : MonoBehaviour {

    public int Id; // id элемента
    public string Name, Energy, Boost, Respawn; // gjkexftvst лучаемы данные
    public Text NameT, EnergyT, BoostT, RespawnT; // текстовые элементы для отображения характеристик в магазине
    public Vector3 EnergyScale, BoostScale, RespawnScale; // Отображение размеров шкалы характеристик от 0 до 1
    public Transform CharacterPointSpawn; // Точка спауна миниатуюр персонажей в магазине


    public void Apply()
    {
        NameT.text = Name;
        EnergyT.text = Energy;
        BoostT.text = Boost;
        RespawnT.text = Respawn;
    }
}
