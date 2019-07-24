using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace SplitPrice { };

public class SettingsElementCharacterInStore : MonoBehaviour {

    public int Id, // id элемента
        Price, // Цена персонажа или его улучшение
        Lvl = 1; // Уровень улучшения персонажа
    public float Markup = 0.15f; // Процентная ставка увеличения цены улучшения
    public string Name, Energy, Boost, Respawn; // gjkexftvst лучаемы данные
    public Text NameT, EnergyT, BoostT, RespawnT, PriceT; // текстовые элементы для отображения характеристик в магазине
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

        PriceT.text = SlidePrice(Price);
    }

    // Разделение числа по три знака
    private string SlidePrice(float price)
    {
        string text = price + ""; // Получение из int string
        // Создание массива по 3 разряда числа
        string[] symbols = new string[Mathf.CeilToInt(text.Length/3.0f)];
        string endText = ""; // Конечный полученный текст

        /* Цикл разбивает число по 3 разряда и записывает в массив. Нулевой индекс массива равен последним 3 цифрам числа
         * Последний индекс массива равен самому старшему разряду в числе*/
        for(int i = 0; i < symbols.Length; i++)
        {
            // Отделение 3-х младших разрядов от числа. Деление на 1000 и преобразовывание остатка
            symbols[i] = ((price / 1000.0f) - Mathf.FloorToInt(price / 1000.0f)) * 1000+ "";
            // Уменьшение первоначального числа на 3 разряда. Деление на 1000 и отнимание остатка
            price = (price / 1000.0f) - ((price / 1000.0f) - Mathf.FloorToInt(price / 1000.0f));
        }

        // Цикл записывает разряды в строку и между 3-мя разрядавми ставит запятую
        for(int i = symbols.Length-1; i >= 0; i--)
        {
            // Дополнение младших разрядов нулями
            // число не должно находится в конце массива, так как это число имеет старший разряд
            if (i != symbols.Length-1)
            {
                // Если в числе меньше 3-х разрядов, то нужно перед ним дописать нули
                if(symbols[i].Length < 3)
                {
                    /* Определение количества дописываемых нулей
                     * из 3 вычитаем количество уже существующих разрядов*/                
                    for(int k = 0; k <= (3 - symbols[i].Length); k++)
                    {
                        symbols[i] = "0" + symbols[i];
                    }
                }
            }

            // Запись в конечный текст разряды
            endText += symbols[i];

            /* Разделение разрядов запятой
             * Только если это не самый младший разряд, после него запятая не ставится*/
            if (i != 0)
            {
                endText += ",";
            }
        }
        return endText;
    }

}
