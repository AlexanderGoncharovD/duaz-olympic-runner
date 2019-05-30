using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float Speed; // текущая скорость персонажа
    public Vector2 RangeSpeed = new Vector2(2, 5); // Минимальная и максимальная скорость игрока
    public float SpeedUp = 2.0f; // Ускорение при разовом нажатии
    public float SpeedUpDecrease = 0.15f; // Значение в процентах, на которое уменьшается SpeedUp
    float DefaultSpeedUp; // Изначальное значение SpeedUp
    float SpeedUpMax; // Ускорение при разовом нажатии
    public float SpeedLoss = 1.0f; // Потеря скорости при бездействии
    public float MaxEnergy = 100.0f;  // Максимальный запас энергии
    public float Energy = 100.0f;  // запас энергии
    public float DelyRecoveryEnergy; // временная здаержка восстновления шкалы енергииж
    float RecoveryEnergy; // Значение восстановления энергии
    public float EnergyTimer;
    public bool run;
    public bool RunFast;
    bool SmoothRunFastLayer2Anim; // Параметер второго анимациинного слоя на персонаже, параметер отвчает за прозрачность анимационного слоя
    float TimerRunFastLayer2Anim; // Время поистечению которого анимацинной слой ускорения становится прозрачным
    public bool isJump = false, isJumpOver = false; // используется в анимации при прыжке

    public Interface Interface; // ссылка на скрипт
    public Animator InterfaceAnimator; //Ссылка на аниматор графического интерфейса
    public GameObject EnergyBorder; // для анимации, когда заканчивается энергия


    public Vector2 startPos;
    public Vector2 direction;

    Animator animator;
    Rigidbody rigidbody;
    Vector3 xMovePlayer, xLateMovePlayer; // х Персонажа до и после кадра

    // Use this for initialization
    void Start() {

        Energy = MaxEnergy;
        animator = this.GetComponent<Animator>();
        Speed = RangeSpeed.x;
        SpeedUpMax = SpeedUp;
        rigidbody = GetComponent<Rigidbody>();
        DefaultSpeedUp = SpeedUp;
        RecoveryEnergy = Energy * 0.15f;
    }

    void FixedUpdate()
    {
        if (run)
        {
            rigidbody.MovePosition(transform.position + transform.right * Speed * Time.deltaTime);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            OneTouch();
            
        }
        else
        {
            
            if (EnergyTimer > 0)
            {
                EnergyTimer -= Time.deltaTime;
            }
            else
            {
                if (Energy < MaxEnergy)
                {
                    Energy += RecoveryEnergy * Time.deltaTime;
                    SpeedUp = Mathf.Lerp(SpeedUp, SpeedUpMax, Time.deltaTime);
                    Interface.CalculationSizeEnergyBar();
                }
            }

            if (Speed > RangeSpeed.x)
            {
                Speed -= Time.deltaTime * SpeedLoss;
            }
            else
            {
                Speed += Time.deltaTime * 0.2f;
            }
            if (SpeedUp < DefaultSpeedUp)
            {
                //SpeedUpRecovery();
            }

        }

        if (Input.GetMouseButtonDown(1))
        {
            SwipeUp();
        }

        // Рассчёт скорости анимации бега
        animator.SetFloat("speed", 0.5f + ((Speed * 100) / (RangeSpeed.y - RangeSpeed.x) * 0.5f) / 100.0f);
        if (SmoothRunFastLayer2Anim)
        {
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0.75f, Time.deltaTime * 5));
            if (TimerRunFastLayer2Anim > 0)
            {
                TimerRunFastLayer2Anim -= Time.deltaTime;
            }
            else
            {
                SmoothRunFastLayer2Anim = false;
            }
        }
        else
        {
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0.0f, Time.deltaTime * 2));
        }

        if (isJump)
        {
            animator.SetLayerWeight(2, Mathf.Lerp(animator.GetLayerWeight(2), 1.0f, Time.deltaTime * 2.0f * Speed));
        }
        else if (isJumpOver)
        {
            animator.SetLayerWeight(2, Mathf.Lerp(animator.GetLayerWeight(2), 0.0f, Time.deltaTime * 1.5f * Speed));
            if (animator.GetLayerWeight(2) <= 0.08f)
            {
                animator.SetLayerWeight(2, 0.0f);
                isJumpOver = false;
            }
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Handle finger movements based on TouchPhase
            switch (touch.phase)
            {
                //When a touch has first been detected, change the message and record the starting position
                case TouchPhase.Began:
                    // Record initial touch position.
                    startPos = touch.position;
                    break;

                //Determine if the touch is a moving touch
                case TouchPhase.Moved:
                    // Determine direction by comparing the current touch position with the initial one
                    direction = touch.position - startPos;
                    if (touch.position.y > startPos.y && touch.position.y - startPos.y > Screen.height*0.2)
                    {
                        SwipeUp();
                    }
                    break;

                case TouchPhase.Ended:
                    // Report that the touch has ended when it ends
                    break;
            }
        }

    }

    void OneTouch()
    {
        if (Energy > 0)
        {
            if (Speed < RangeSpeed.y)
            {
                Energy -= 1.0f;
                Speed += SpeedUp;
                SpeedUp -= SpeedUp * SpeedUpDecrease;
                EnergyTimer = DelyRecoveryEnergy;
            }
            else
            {
                Energy -= 0.5f;
                EnergyTimer = DelyRecoveryEnergy;
            }
            Interface.CalculationSizeEnergyBar();
        }
        else
        {
            Energy = 0;
            Interface.CalculationSizeEnergyBar();
            if (!EnergyBorder.activeSelf)
            {
                InterfaceAnimator.Play("energy border");
            }
        }
        SmoothRunFastLayer2Anim = true;
        TimerRunFastLayer2Anim = 0.5f;
    }

    void SwipeUp()
    {
        if (animator.GetLayerWeight(2) == 0.0f)
        {
            animator.SetBool("jump", true);
            isJumpOver = false;
            isJump = true;
        }
    }

    void JumpAddForce()
    {
        rigidbody.AddForce(transform.up * 350, ForceMode.Impulse);
    }

    void JumpOver()
    {
        animator.SetBool("jump", false);
        isJump = false;
        isJumpOver = true;
    }

    void AnimationStartAnimSpeedZero()
    {
        animator.SetFloat("speed", 0.0f);
    }

    void AnimationStartAnimSpeedRun()
    {
        animator.SetFloat("speed", 1.0f);
        animator.SetBool("run", true);
        run = true;
        this.GetComponent<Parallax>().enabled = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Barrier")
        {
            Destroy(collision.gameObject);
            Speed = Speed * 0.75f;
        }
    }
    
    
}
