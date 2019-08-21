using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Скрипт управления экземпляром одного соперника
 Управление основывается на положении/действиях игрока*/
public class ArtificialIntelligence : MonoBehaviour
{
    public GameObject NextBarrier; // Ссылка на следущее препятсвие

    private Vector3 EdgeBarrier; // Крайняя точка препятствия
    [HideInInspector]
    public GlobalGameControl globalGameControl;
    private MoveController moveController;
    private AnimationController animation;
    private CapsuleCollider thisCollider;

    void Start()
    {
        moveController = GetComponent<MoveController>();
        animation = GetComponent<AnimationController>();
        thisCollider = GetComponent<CapsuleCollider>();
    }

    private void Update()
    {
        if (NextBarrier == null)
        {
            if (moveController.isStartGame)
            {
                GetNearBarrier();
            }
        }
        else
        {
            Debug.DrawRay(transform.position, NextBarrier.transform.position, Color.yellow);
        }
    }

    private void GetNearBarrier()
    {
        RaycastHit hit;
        // Пустить луч, чтобы найти следующее препятствия
        if (Physics.Raycast(transform.position - new Vector3(0, 0.5f, 0), transform.TransformDirection(Vector3.right), out hit, Mathf.Infinity, 1 << 8 /*Битовая маска 6 слоя коллайдеров*/))
        {
            Debug.Log("Searched next barrier");
            NextBarrier = hit.transform.gameObject;
            EdgeBarrier = new Vector3(NextBarrier.transform.position.x- NextBarrier.GetComponent<BoxCollider>().size.x / 2 - thisCollider.radius, 0, transform.position.z);
        }
    }

    // Бежать к стартовой линии
    public void RunToTheStartingLine()
    {
        moveController.Speed = 3.5f;
        animation.Run();
    }

    // Обрабатывать столкновения
    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "OpponentStartPoint")
        {
            moveController.Speed = 0.0f;
            collision.collider.GetComponent<BoxCollider>().enabled = false;
            animation.Idle();
            globalGameControl.ReadyForAnotherPlayer();
        }
    }
}
