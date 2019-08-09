using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum action
{
    NoAction,
    Jump,
    RolledUp
}

public class RivalOnTriggerBarrier : MonoBehaviour {

    public action expectedAction; // Какое действие ожидается от соперника для преодоления препятствия


    private Rival Character; // Ссылка на соперника, которые вошел в триггер препятствия
    private BoxCollider[] thisColliser;

    private void OnTriggerEnter(Collider collider)
    {
        if(Character == null)
        {
            // Получение ссылки на управляющий персонажем скрипт
            Character = collider.gameObject.GetComponent<Rival>();
        }
        // Получение списка всех коллайдеров на объекте
        thisColliser = GetComponents<BoxCollider>();
        // Определение диапазона в пределах которой персонаж может выполнить действие
        var acrionRange = new Vector2((transform.position.x + thisColliser[1].center.x) - thisColliser[1].size.x / 2,
            (transform.position.x + thisColliser[1].center.x) + thisColliser[1].size.x / 2);
        // Передача персонажу тега препятствия и диапазон выполнения действия
        Character.MakeDecision(tag, acrionRange, expectedAction+"");
        thisColliser[1].enabled = false;
    }
}
