using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public float RolledUpTime; // Время, которое персонаж проезжает в лежачем положении (подкат)
    public bool isPitDown; // Если персонаж упал в яму
    public float TimeForRespawnPit = 3.0f; // Время для возрождения из ямы 

    public Interface Interface; // ссылка на скрипт
    public Animator InterfaceAnimator; //Ссылка на аниматор графического интерфейса
    public GameObject EnergyBorder; // для анимации, когда заканчивается энергия

    public Text debug;

    [Header("Other objects")]
    public GameObject UILineRespawn; // UI элемент шкала возрождения персонажа после падения в яму
    public GameObject UILineScale; // Изменение размера шкалы времени для возрождения

    Animator animator;
    Rigidbody rigidbody;
    Vector3 xMovePlayer, xLateMovePlayer; // х Персонажа до и после кадра
    bool isOneTouch, isTouchHold,
        isCollisionWithBarrier, // Столкновение с препятствием
        isCollisionStandingSlipPuddle, // Скольжение по луже стоя
        isCollisionDownSlipPuddle,
        isCollisionPitDown, // Падени в яму
        isRolledUp, // Подкат
        isEnableDivingAnimation, // Плавное включение анимации с погружение под воду в озере
        isDisableDivingAnimation; // плавное отключение анимации с погружение под воду в озере
    float timerPitDown; // Таймер, после которого начинается возрождения персонажа
    float timeRespawnPit = 3.0f; // Время возрождения после попадания в яму
    Transform PositionPitRespawn; // Точка респавна после падения в яму
    Vector3 StartPositionCharacter; // Стартовая позиция персонажа нужна для респавна из ямы (используется только Y)

    // Use this for initialization
    void Start() {

        Energy = MaxEnergy;
        animator = this.GetComponent<Animator>();
        Speed = RangeSpeed.x;
        SpeedUpMax = SpeedUp;
        rigidbody = GetComponent<Rigidbody>();
        DefaultSpeedUp = SpeedUp;
        RecoveryEnergy = Energy * 0.15f;
        StartPositionCharacter = transform.position; // Стартовая позиция персонажа нужна для респавна из ямы (используется только Y)

        Input.simulateMouseWithTouches = true;
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
        if (isPitDown) // Если персонаж упал в яму
        {
            if (timerPitDown > 0)
            {
                timerPitDown -= Time.deltaTime;
            }
            else
            {
                if (!UILineRespawn.activeSelf && timeRespawnPit == TimeForRespawnPit)
                {
                    UILineRespawn.SetActive(true);
                }
                if (timeRespawnPit > 0)
                {
                    timeRespawnPit -= Time.deltaTime;
                    UILineScale.transform.localScale = new Vector3((timeRespawnPit * 1.0f) / TimeForRespawnPit, 1f, 1f);
                }
                else
                {
                    StartCoroutine(CharacterRespawnFromThePit()); // Респавн из ямы
                }
            }
        }
        else
        {
            // рассчёт ускорения
            if (!isOneTouch)
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

                if (!isTouchHold)
                {
                    CalculationSpeed();
                }

            }
        }

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(1))
        {
            SwipeUp();
        }
        if (Input.GetMouseButtonDown(0))
        {
            isOneTouch = true;
            OneTouch();
        }
        if (Input.GetMouseButton(0))
        {
            isTouchHold = true;
            HoldTouch();
        }
        if (Input.GetMouseButtonUp(0))
        {
            isTouchHold = false;
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SwipeDown();
        }
#endif

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

        // Проверка прикосновения к экрану
        TouchScreen();

        // процедра прозрачности анимационного слоя прыжка
        Jump();

        // процедура настройки прозрачности анимационного слоя при подкате
        RolledUp();

        // процедура настройки прозрачности анимационного слоя при столкновении
        CollisionWithBarrier();

        // Если персонаж подскользулся на луже стоя
        if (isCollisionStandingSlipPuddle)
        {
            CollisionStandingSlipPuddle();
        }
        // Если персонаж упал в яму
        if (isCollisionPitDown)
        {
            CollisionPitDown();
        }

        // Плавное ВКЛЮЧЕНИЕ анимации погружения под воду в озере
        if (isEnableDivingAnimation)
        {
            animator.SetLayerWeight(8, Mathf.Lerp(animator.GetLayerWeight(8), 1.0f, Time.deltaTime * 10));
        }
        // Плавное ВЫКЛЮЧЕНИЕ анимации погружения под воду в озере
        if (isDisableDivingAnimation)
        {
            animator.SetLayerWeight(8, Mathf.Lerp(animator.GetLayerWeight(8), .0f, Time.deltaTime * 10));
            if (animator.GetLayerWeight(8) <= 0.05f)
            {
                isDisableDivingAnimation = false;
                animator.SetBool("diving", false);
            }
        }
    }
    // рассчет скорости бега персонажа
    void CalculationSpeed()
    {
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

    Vector2 startPos;
    /*Определение касаний экрана*/
    void TouchScreen()
    {
        debug.text = "Debug:\nКоличество касаний: " + Input.touchCount + "\n";                                          /// DEBUG   DEBUG   DEBUG   DEBUG
        if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                // Отслеживание действия касания
                switch (touch.phase)
                {
                    // Если только что докаснулся до экрана
                    case TouchPhase.Began:
                        startPos = touch.position;
                        break;

                    // Если было совершено перемещение пальцем
                    case TouchPhase.Moved:
                        // Если это был быстрый свайп
                        if (touch.deltaPosition.magnitude / touch.deltaTime >= 600)
                        {
                            // Если свайп вверх и длина свайпа больше 15% высоты экрана
                            if (touch.position.y > startPos.y && touch.position.y - startPos.y > Screen.height * 0.15f)
                            {
                                SwipeUp();
                            }
                            if (touch.position.y < startPos.y && startPos.y - touch.position.y > Screen.height * 0.15f)
                            {
                                SwipeDown();
                            }
                        }
                        break;

                    case TouchPhase.Stationary:
                        debug.text += "Touch hold";                                                                    /// DEBUG   DEBUG   DEBUG   DEBUG
                        isTouchHold = true;
                        HoldTouch();
                        break;

                    // Если палец был убран с экрана
                    case TouchPhase.Ended:
                        // Если отпускание пальца лежит в доступном радиусе (1% от высоты экрана) от прикосновения пальца до экрана
                        if ((touch.position-startPos).magnitude <= Screen.height * 0.01f)
                        {
                            isOneTouch = true;
                            OneTouch();
                        }
                        isTouchHold = false;
                        break;
                }
            }
        }
    }

    /*рассчет действия на разовое нажатие на экран (ускорение)*/
    void OneTouch()
    {
        if (isPitDown) // Если персонаж упал в яму
        {
            if (UILineRespawn.activeSelf)
            {
                timeRespawnPit -= TimeForRespawnPit / 20.0f;
            }
        }
        else
        {
            // Если персонаж не в подкате
            if (!isRolledUp)
            {
                if (Energy > 0)
                {
                    // Если скорость меньше максимальной
                    if (Speed < RangeSpeed.y)
                    {
                        // уменьшение запаса выносливости и увеличение текущей скорости
                        Energy -= 1.0f;
                        Speed += SpeedUp;
                        // уменьшение индекса ускорения и запуск таймер восстновления выносливости
                        SpeedUp -= SpeedUp * SpeedUpDecrease;
                        EnergyTimer = DelyRecoveryEnergy;
                    }
                    else
                    {
                        Energy -= 0.5f;
                        //EnergyTimer = DelyRecoveryEnergy;
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
            isOneTouch = false;
        }
    }

    /*Если удержание пальца на экарне*/
    void HoldTouch()
    {
        Energy -= 0.5f * Time.deltaTime;
        TimerRunFastLayer2Anim = 0.5f;
        if (Energy <= 0)
        {
            isTouchHold = false;
            TimerRunFastLayer2Anim = 0.0f;
            CalculationSpeed();
        }
        else
        {
            SmoothRunFastLayer2Anim = true;
        }
        Interface.CalculationSizeEnergyBar();
    }

    void SwipeUp()
    {
        // Если персонаж ещё не прыгает и не находится в лежачем положении (подкат)
        if (animator.GetLayerWeight(2) == 0.0f && !isRolledUp)
        {
            animator.SetBool("jump", true);
            isJumpOver = false;
            isJump = true;
        }
        else if (isRolledUp) // Если на ходится в лежачем положении, то встает
        {
            RolledUpTime = 0.0f;
        }
    }

    void SwipeDown()
    {
        if (Speed >= RangeSpeed.x && RolledUpTime <= 0)
        {
            RolledUpTime = 2.0f;
            isRolledUp = true;
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

    /* процедра прозрачности анимационного слоя прыжка*/
    void Jump()
    {
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
    }

    /* процедура настройки прозрачности анимационного слоя при столкновении*/
    void CollisionWithBarrier()
    {
        // Плавное переключение на слой с анимацией столкновения с препятствием
        if (isCollisionWithBarrier)
        {
            // плавно устанаваливается прозрачность анимацинного слоя до 56%
            animator.SetLayerWeight(3, Mathf.Lerp(animator.GetLayerWeight(3), 0.56f, Time.deltaTime * 2.5f * Speed));
            // если прозрачность слоя больше 55%, то нужно выключать этот слой с анимацией столкновения
            if (animator.GetLayerWeight(3) >= 0.55f)
            {
                isCollisionWithBarrier = false;
            }
        }
        else
        {
            // если персонаж не врезался в перпятствие и прозрачность анимационного слоя больше 2,5%, то нужно уменьшать прозрачность
            if (animator.GetLayerWeight(3) >= 0.025f)
            {
                animator.SetLayerWeight(3, Mathf.Lerp(animator.GetLayerWeight(3), 0.0f, Time.deltaTime * 0.5f * Speed));
            }
            else
            {
                // если прозрачность меньше 2,5%, то приравниваем её к нулю и выходим из конструкции уменьшения прозрачности
                animator.SetLayerWeight(3, 0.0f);
            }
        }
    }

    // Если персонаж подскользнулся на луже стоя
    float timerSlipPuddle = .5f; // время проигрывания анимации скольжения по луже 
    void CollisionStandingSlipPuddle()
    {
        if (timerSlipPuddle > 0)
        {
            animator.SetLayerWeight(5, Mathf.Lerp(animator.GetLayerWeight(5), 1.0f, Time.deltaTime * Speed));
            timerSlipPuddle -= Time.deltaTime;
        }
        else
        {
            animator.SetLayerWeight(5, Mathf.Lerp(animator.GetLayerWeight(5), .0f, Time.deltaTime * Speed));
            if (animator.GetLayerWeight(5) <= 0.05f)
            {
                animator.SetLayerWeight(5, .0f);
                timerSlipPuddle = .5f;
                isCollisionStandingSlipPuddle = false;
            }
        }
    }

    // Если пресонаж упал в яму
    void CollisionPitDown()
    {
        animator.SetLayerWeight(6, Mathf.Lerp(animator.GetLayerWeight(6), 1.0f, Time.deltaTime * 10));
    }

    /*процедура настройки прозрачности анимационного слоя при подкате*/
    void RolledUp()
    {
        if (isRolledUp)
        {
            if (RolledUpTime > 0 && Speed >= RangeSpeed.x)
            {
                Speed -= SpeedLoss / 4 * Time.deltaTime;
                animator.SetLayerWeight(4, Mathf.Lerp(animator.GetLayerWeight(4), 0.95f, Time.deltaTime * 1.5f * Speed));
                RolledUpTime -= Time.deltaTime;
            }
            else
            {
                RolledUpTime = 0.0f;
                animator.SetLayerWeight(4, Mathf.Lerp(animator.GetLayerWeight(4), 0.0f, Time.deltaTime * 1.5f * Speed));
                if (animator.GetLayerWeight(4) <= 0.025f)
                {
                    animator.SetLayerWeight(4, 0.0f);
                    isRolledUp = false;
                }
            }
        }
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

    // Респавн из ямы
    IEnumerator CharacterRespawnFromThePit()
    {
        UILineRespawn.SetActive(false);
        GetComponent<CapsuleCollider>().enabled = true;
        GetComponent<CameraLookAtPlayer>().isLookAt = true;
        isCollisionPitDown = false;
        Speed = 5.0f;
        isPitDown = false;
        animator.SetLayerWeight(6,.0f);
        transform.position = new Vector3(PositionPitRespawn.position.x, StartPositionCharacter.y, PositionPitRespawn.position.z);
        GetComponent<CameraLookAtPlayer>().isReset = true;
        animator.SetLayerWeight(7, 1.0f);
        Energy = MaxEnergy;
        yield return new WaitForSeconds(1.5f);
        animator.SetLayerWeight(7, .0f);
    }

    // Проверка столкновений
    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Barrier")
        {
            collision.gameObject.GetComponent<Animator>().Play("crash down");
            Speed = Speed * 0.75f;
            // Если персонаж врезается в положении лежа (подкате)
            if (isRolledUp)
            {
                RolledUpTime = 0.0f;
            }
            isCollisionWithBarrier = true;
        }
        else if (collision.collider.tag == "Puddle") // Скольжение по луже
        {
            collision.collider.enabled = false;
            collision.collider.GetComponent<ParticleSystem>().Play();
            Speed -= .75f;

            if (isRolledUp)
            {
                isCollisionDownSlipPuddle = true;
            }
            else
            {
                isCollisionStandingSlipPuddle = true;
            }
        }
        else if (collision.collider.tag == "Pit") // Падение в яму
        {
            collision.collider.enabled = false;
            GetComponent<CapsuleCollider>().enabled = false;
            isCollisionPitDown = true;
            rigidbody.velocity = Vector3.zero;
            rigidbody.velocity = new Vector3(-Speed*.65f, -10.0f, 0);
            Speed -= .0f;
            timerPitDown = 1.5f; //Назначение таймера, после которого начинается возрождения персонажа
            timeRespawnPit = TimeForRespawnPit;
            isPitDown = true;
            PositionPitRespawn = collision.transform;
            //GetComponent<CameraLookAtPlayer>().isLookAt = false;
        }
        else if (collision.collider.tag == "Lake")
        {
            collision.collider.enabled = false;
            Speed = 2.0f;
            animator.SetBool("diving", true);
            isDisableDivingAnimation = false;
            isEnableDivingAnimation = true;
        }
    }
    
    void DisableDivingAnimation()
    {
        Speed = 5.0f;
        isEnableDivingAnimation = false;
        isDisableDivingAnimation = true;
        Energy = MaxEnergy;
    }
    
}
