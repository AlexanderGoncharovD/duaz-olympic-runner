using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsElementCharacterInStore : MonoBehaviour {

    public int Id; // id элемента
    public string Name, Energy, Boost, Respawn; // gjkexftvst лучаемы данные
    public Text NameT, EnergyT, BoostT, RespawnT; // текстовые элементы для отображения характеристик в магазине
    public Vector3 EnergyScale, BoostScale, RespawnScale; // Отображение размеров шкалы характеристик от 0 до 1
    public Transform EnergyLine, BoostLine, RespawnLine; // UI шкала характеристик от 0 до 1
    public Transform CharacterPointSpawn; // Точка спауна миниатуюр персонажей в магазине
    public GameObject SelectText; // Текст уведомляющий, что этот персонаж уже выбран
    public bool isBuy, // Куплен ли этот персонаж
        isSelected; // Выбран ли это персонаж


    public void Apply()
    {
        NameT.text = Name;
        EnergyT.text = Energy;
        BoostT.text = Boost;
        RespawnT.text = Respawn;

        EnergyLine.localScale = EnergyScale;
        BoostLine.localScale = BoostScale;
        RespawnLine.localScale = RespawnScale;
    }
}
