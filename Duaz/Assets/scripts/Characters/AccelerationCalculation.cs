using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Структура с найстройкой ситуаций
[System.Serializable]
public struct SituationStruct
{
    public string name; // Названеи ситуации и её обозначение
    public Vector2 RangeTimeInterval; // Временные интервалы между действиями
    public float TimeInterval; // Временной интервал между действиями 
    public float Time; // Время работы ситуации
    public int[] ActionDesignation; // Массив действий 1 - разовое нажатие; 2 - удержание
    [Range(.0f, 1.0f)] public float ChancePerformAction, // Будет ли выполняться какое-либо действие
        ChanceTouch, // Шанс нажатия на экран для ускарения
        EnergyCosts; // Сколько затрачивается энергии на разгон
}

// Скрипт позволяет генрировать модели поведения соперников для разных ситуаций
public class AccelerationCalculation : MonoBehaviour
{
    public SituationStruct[] Situations; // Все возможные ситуации у персонажа от этого зависити какие действия делать
    public SituationStruct Situation; // Действующая ситуация

    // Генерация действий для ситуации
    public void SituationGeneration(int id)
    {
        Situation = Situations[id]; // Выбор по Id из массива той ситуации, которая сейчас происходит
        Situation.TimeInterval = TimeIntervalCalculation(id); // Определенеие времнного интервала между нажатиями на экран
        // Сколько будет затрачиваться энергии на разгон
        Situation.EnergyCosts = Random.Range(Situation.EnergyCosts - 0.25f, Situation.EnergyCosts + 0.25f);

        // Определение количества действий, которые будут сделаны за отвдённое время для происходящей ситуации
        var numberOfActions = Mathf.CeilToInt(Situation.Time / Situation.TimeInterval);
        Situation.ActionDesignation = new int[numberOfActions]; // Создание массива действий, где 1 - разовое нажатие; 2 - удержание

        // Генерирование значения для каждого действия в массиве
        for (int action = 0; action < Situation.ActionDesignation.Length; action++)
        {
            /*Выбор, будет ли персонаж делать какое-либо действие
             * Иначе 0 - невыполнение каких-либо действий (По умолчанию элементы массива имют значение 0)*/
            if (Random.Range(0.0f, 1.0f) <= Situation.ChancePerformAction)
            {
                // Определённое количество процентов для каждой ситуации, что выпадет 1 - разовое нажатие на экран
                if (Random.Range(0.0f, 1.0f) <= Situation.ChanceTouch)
                {
                    Situation.ActionDesignation[action] = 1;
                }
                else // Иначе 2 - долгое нажатие на экран
                {
                    Situation.ActionDesignation[action] = 2;
                }
            }
        }
    }

    // Вычисление временных интервалов между имитациями нажатий на экран
    private float TimeIntervalCalculation(int id)
    {
        return Random.Range(Situation.RangeTimeInterval.x, Situation.RangeTimeInterval.y);
    }
}
