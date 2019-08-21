using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveController : MonoBehaviour
{
    public float Speed, // Текущая скорость персонажа
           Enegry; // Текущая энерги персонажа

    public Vector2 RangeSpeed, // Минимальная и максимальная скорость
        RangeEnergy; // Минимальный и максимальный запас энергии

    public bool isStartGame, // Игра началась. Персонаж побежал со стартовой линии
        isFinish; // Если персонаж пересёк финишную линию


    private Rigidbody rigidbody;
    private AnimationController animation;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        animation = GetComponent<AnimationController>();
    }

    private void FixedUpdate()
    {
        rigidbody.MovePosition(transform.position + transform.right * Speed * Time.deltaTime);
    }

    void Update()
    {
        // Если персонаж бежит со стартовой линии
        if (isStartGame)
        {
            // Если персонаж пересёк финишную линию
            if (isFinish)
            {

            }
            else
            {

            }
        }

        // Рассчёт скорости анимации на основе текущей скорости персонажа
        animation.Speed(Speed);
    }

    // Начать забег
    public void StartRun()
    {
        isStartGame = true;
        Speed = RangeSpeed.x;
        animation.Run();
    }
}
