using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RivalOnTriggerBarrier : MonoBehaviour {

    public Rival Character;

    private BoxCollider[] thisColliser;

	private void OnTriggerEnter(Collider collider)
    {
        // Получение списка всех коллайдеров на объекте
        thisColliser = GetComponents<BoxCollider>();
        // Получение ссылки на управляющий персонажем скрипт
        Character = collider.gameObject.GetComponent<Rival>();
        // Определение диапазона в пределах которой персонаж может выполнить действие
        var acrionRange = new Vector2((transform.position.x + thisColliser[1].center.x) - thisColliser[1].size.x / 2,
            (transform.position.x + thisColliser[1].center.x) + thisColliser[1].size.x / 2);
        // Передача персонажу тега препятствия и диапазон выполнения действия
        Character.MakeDecision(tag, acrionRange);
    }
}
