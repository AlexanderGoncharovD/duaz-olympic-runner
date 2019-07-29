using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rival : MonoBehaviour {

    public Transform StartPoint; // Стартовая точка, до которой идёт соперник
    public int NumTeadmills; // Порядковый номер беговой дорожки
    public float Speed;
    public Vector2 RangeSpeed = new Vector2(2, 5); // Минимальная и максимальная скорость игрока
    public InitializationRival initializationRival; // Родительский скрипт

    private bool isNotOnStartPosition; // Если соперник ещё не на стартовой линии
    private Rigidbody rigidbody;
    private Animator animator;
    private bool isGame;

	void Start () {
        // Соперник не на ходится на стартовой линии
        isNotOnStartPosition = true;
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        animator.enabled = true;
        animator.SetBool("run", true);
        Speed = 3.5f; // Стартовая скорость для перемещения до стартовой линии

    }

	void Update () {
		
        // Если соперник находится не на стартовой линии, то переместить его до старта
        if (isNotOnStartPosition)
        {
            if (transform.position.x < StartPoint.position.x)
            {
                rigidbody.MovePosition(transform.position + transform.right * Speed * Time.deltaTime);
            }
            else
            {
                isNotOnStartPosition = false;
                animator.SetBool("run", false);
                /* Дополнить массив готовности соперников
                 * Найти элемент с занчением "false" и сделать его "true"*/
                for (int i = 0; i < initializationRival.isDoneRivels.Length; i++)
                {
                    if (!initializationRival.isDoneRivels[i])
                    {
                        initializationRival.isDoneRivels[i] = true;
                        if (i == initializationRival.isDoneRivels.Length - 1)
                        {
                            initializationRival.globalGameControl.startGame = true;
                            initializationRival.globalGameControl.StartGame();
                        }
                        break;
                    }
                }
            }
        }
        else
        {
            if (initializationRival.globalGameControl.startGame && !isGame)
            {
                animator.enabled = true;
                animator.SetInteger("start", 1);
                animator.SetBool("run", true);
                isGame = true;
            }
        }
        // Рассчёт скорости анимации бега
        animator.SetFloat("speed", 0.5f + ((Speed * 100) / (RangeSpeed.y - RangeSpeed.x) * 0.5f) / 100.0f);
    }
}
