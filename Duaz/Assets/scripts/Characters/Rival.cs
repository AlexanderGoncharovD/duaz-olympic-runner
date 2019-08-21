using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rival : MonoBehaviour
{
    public Transform StartPoint; // Стартовая точка, до которой идёт персонаж
    public int NumTeadmills; // Порядковый номер беговой дорожки
    public Vector2 RangeSpeed = new Vector2(5, 10); // Минимальная и максимальная скорость персонажа
    public float Speed, // Текущая скорость перемещения персонажа
        SpeedLoss = 1.0f, // Потеря скорости при бездействии
        SpeedUp = 2.0f, // Разовое ускорение
        RolledUpTime = 2.0f, // Время, которое персонаж проезжает в лежачем положении (подкат)
        MaxEnergy = 100.0f,  // Максимальный запас энергии
        Energy = 100.0f,  // запас энергии
        TimeForRespawnOak = 3.0f, // Время для возрождения из ямы 
        TimeForRespawnPit = 3.0f, // Время для возрождения из ямы
        JumpForce = 350.0f, // сила прыжка
        SpeedUpDecrease = 0.15f, // Значение в процентах, на которое уменьшается SpeedUp
        EnergyTimer; // Таймер, после которого начинается восстановления энрегии
    public bool isRun, // Бежит ли персонаж
        isRolledUp, // Если персонаж находится в подкате
        isCollisionWithBarrier, // Если персонаж столкнулся с барьером
        isCollisionDownSlipPuddle, // Если персножа проскользил по луже лёжа
        isCollisionStandingSlipPuddle, // Если персонаж подскользнулся на луже стоя
        isEnableDivingAnimation, // Плавное включение анимации с погружение под воду в озере
        isDisableDivingAnimation, // плавное отключение анимации с погружение под воду в озере
        isClashWithsOak, // столкноение персонажа с веткой дуба
        isCollisionPitDown, // Падени в яму
        isPitDown, // Если персонаж упал в яму
        PerformAction, // Выполнить действие по преодалению препятствия
        isJump, // Прыжок персонажа начался
        isJumpOver, // Прыжок персонажа закончился
        SmoothRunFastLayer2Anim, // Параметер второго анимациинного слоя на персонаже, параметер отвчает за прозрачность анимационного слоя
        isFinish;
    [Range(0.0f, 1.0f)] public float ChancePerformAction = 0.95f, // Вероятность выполнения действия
        ActionExecution = 0.8f; // Вероятность прыжка в процентном соотношении
    public int IdSituation; // Порядковый номер ситуации (Массив ситуаций в скрипте AccelerationCalculation)
    public SituationStruct CurrentSituation; // Активная на данный момент ситуация

    private bool isNotOnStartPosition, // Если персонаж ещё не на стартовой линии
        isGame,
        isOneTouch, // Имитиация разового нажатия на экран активно
        isTouchHold;
    private float timerOakDown, // Таймер, после которого начинается возрождения персонажа
        timeRespawnOak = 3.0f, // Время возрождения после попадания в яму
        SpeedClashWithsOak, // Скорость переключения на слой с анимацией столкновения
        timerPitDown, // Таймер, после которого начинается возрождения персонажа
        timeRespawnPit = 3.0f, // Время возрождения после попадания в яму
        pointAction, // Точка выполнения действия для преодаления препятствия
        timerBetweenActions = 0.0f, // Таймер между исполнением действий в ситуации
        TimerRunFastLayer2Anim, // Время поистечению которого анимацинной слой ускорения становится прозрачным
        DefaultSpeedUp, // Изначальное значение SpeedUp
        SpeedUpMax, // Ускорение при разовом нажатии
        RecoveryEnergy; // Значение восстановления энергии
    private Transform PositionRespawn; // Точка респавна после столкновения/падения
    Vector3 StartPositionCharacter; // Стартовая позиция персонажа нужна для респавна из ямы (используется только Y)
    private Rigidbody rigidbody;
    private Animator animator;
    private AccelerationCalculation accelerationCalculation; // Ссылка на скрипт со всеми ситуациями для разгона

    void Start()
    {
        gameObject.tag = "Rival";
        Destroy(GetComponent<CameraLookAtPlayer>());
        Destroy(GetComponent<Parallax>());
        isNotOnStartPosition = true; // Соперник не на ходится на стартовой линии
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        animator.enabled = true;
        animator.SetBool("run", true);
        Speed = 3.5f; // Стартовая скорость для перемещения до стартовой линии
        StartPositionCharacter = transform.position; // Стартовая позиция персонажа нужна для респавна из ямы (используется только Y)
        accelerationCalculation = GetComponent<AccelerationCalculation>();
        SpeedUpMax = SpeedUp;
        DefaultSpeedUp = SpeedUp;
    }

    private void FixedUpdate()
    {
        // Перемещение пероснажа
        if (isRun)
        {
            rigidbody.MovePosition(transform.position + transform.right * Speed * Time.deltaTime);
        }
    }

    void Update()
    {
        // Если игра уже идёт
        if (isGame)
        {
            if (isPitDown) // Если персонаж упал в яму
            {
                CollisionPitDown();
            }
            else if (isClashWithsOak)// Если персонаж столкнулся с дубом
            {
                CollisionWithOak();
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
                        }
                    }
                    if (!isTouchHold)
                    {
                        if (!isFinish)
                        {
                            // Расчёт скорости бега персонажа
                            CalculationSpeed();
                        }
                    }
                }
                

                // Если оставшейся энергии больше, чем планировалось потратить на разгон
                if (Energy > CurrentSituation.EnergyCosts && isRun)
                {
                    // Исполнение активной ситуации
                    ExecutionOfSituation();
                }

                // процедура настройки прозрачности анимационного слоя при столкновении с барьером
                CollisionWithBarrier();// Если персонаж подскользулся на луже стоя

                if (isCollisionStandingSlipPuddle)
                {
                    CollisionStandingSlipPuddle();
                }

                // Если персонаж столкнулся с озером
                CollisionWithLake();
;

                if (PerformAction)
                {
                    if (transform.position.x > pointAction)
                    {
                        // Процедура выполнения прыжка
                        Jump();

                        // процедура настройки прозрачности анимационного слоя при подкате
                        RolledUp();
                    }
                }
            }
        }
        else
        {
            // Если соперник находится не на стартовой линии, то переместить его до старта
            if (isNotOnStartPosition)
            {
                if (StartPoint != null)
                {
                    if (transform.position.x < StartPoint.position.x)
                    {
                        rigidbody.MovePosition(transform.position + transform.right * Speed * Time.deltaTime);
                    }
                    else
                    {
                        isNotOnStartPosition = false;
                        animator.SetBool("run", false);
                    }
                }
            }
            else
            {

            }
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
    }

    // Исполнение активной ситуации
    int CurActionInSituation; // Index текущего действия в ситуации
    private void ExecutionOfSituation()
    {
        // Если временной интервал между действиями прошёл, то выполнить действие
        if (timerBetweenActions <= 0.0f)
        {
            // Если действие развое нажатие
            if (CurrentSituation.ActionDesignation[CurActionInSituation] == 1)
            {
                isOneTouch = true;
                OneTouch();
            }
            else if (CurrentSituation.ActionDesignation[CurActionInSituation] == 2) // Если действие удержание скорости
            {
                isTouchHold = true;
                HoldTouch();
            }
            else // Если бездействие
            {

            }
            // Если ещё остались действия в этой ситуации
            if (CurActionInSituation < CurrentSituation.ActionDesignation.Length - 1)
            {
                CurActionInSituation++;
            }
            else // Если массив с действиями закончился
            {
                CurActionInSituation = 0;
            }
            timerBetweenActions = CurrentSituation.TimeInterval; // Восстановить таймер между действиями
        }
        else // Если таймер между действиями ещё не истёк
        {
            timerBetweenActions -= Time.deltaTime;
        }
    }

    // рассчет скорости бега персонажа
    private void CalculationSpeed()
    {
        if (Speed > RangeSpeed.x)
        {
            Speed -= Time.deltaTime * SpeedLoss;
        }
        else
        {
            Speed += Time.deltaTime * 0.2f;
        }
    }

    /*рассчет действия на разовое нажатие на экран (ускорение)*/
    void OneTouch()
    {
        if (isPitDown) // Если персонаж упал в яму
        {
            /*if (UILineRespawn.activeSelf)
            {
                timeRespawnPit -= TimeForRespawnPit / 20.0f;
            }*/
        }
        else if (isClashWithsOak)
        {
            /*if (UILineRespawn.activeSelf)
            {
                timeRespawnOak -= TimeForRespawnOak / 20.0f;
            }*/
        }
        else
        {
            // Если персонаж не в подкате и не финишировал
            if (!isRolledUp && !isFinish)
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
                        //EnergyTimer = DelyRecoveryEnergy;
                    }
                    else
                    {
                        Energy -= 0.5f;
                        //EnergyTimer = DelyRecoveryEnergy;
                    }
                }
                else
                {
                    Energy = 0;
                }
                SmoothRunFastLayer2Anim = true;
                TimerRunFastLayer2Anim = 0.5f;
            }
            isOneTouch = false;
        }
    }

    /*Имитация удержания пальца на экарне*/
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
    }

    // Принять решение при приближении к препятствияю
    public void MakeDecision (string tag, Vector2 actionRange, string expectedAction)
    {
        // Выбираем случайную точку из диапазона для выполнения действия
        pointAction = UnityEngine.Random.Range(actionRange.x, actionRange.y);
        switch (ChoiceOfAction(expectedAction))
        {
            case "Jump":
                isJumpOver = false;
                isJump = true;
                break;
            case "RolledUp":
                if (Speed >= RangeSpeed.x)
                {
                    RolledUpTime = 2.0f;
                    isRolledUp = true;
                }
                break;
        }
        PerformAction = true;
    }

    // Выбор, какое действие будет делать соперник (string ожидаемое действие)
    private string ChoiceOfAction(string expectedAction)
    {
        var action = "NoAction"; // Задётся значение на случай, если switch ничего не вернёт
        // Шанс, что персонаж Выполнит какое-либо действие
        if (UnityEngine.Random.Range(0.0f, 1.0f) < ChancePerformAction)
        {
            // Какое действие ожидается от персонажа, то он и выполняется с вероятностью ActionRival
            if (UnityEngine.Random.Range(0.0f, 1.0f) < ActionExecution)
            {
                switch (expectedAction)
                {
                    case "Jump":
                        action = "Jump";
                        break;
                    case "RolledUp":
                        action = "RolledUp";
                        break;
                }
            }
            else
            {
                /* Если ожидался прыжок и шанс выполнение предполагаемого действия не сработал,
                 * то выолняется подкат*/
                if(expectedAction == "Jump")
                {
                    action = "RolledUp";
                }
                else // Соответственно, если предполагался подкат, то выполняется прыжок
                {
                    action = "Jump";
                }
            }
        }
        return action;
    }

    // Действия после старта, когда все начинают бежать
    public void AnimationStartAnimSpeedRun()
    {
        animator.SetFloat("speed", 1.0f);
        Speed = RangeSpeed.x;
        isRun = true;

        // Запуск ситуации из массива с индексм 0 (Разгон на старта)
        IdSituation = 0;
        accelerationCalculation.SituationGeneration(IdSituation);
        CurrentSituation = accelerationCalculation.Situation;
    }

    void JumpAddForce()
    {
        rigidbody.AddForce(transform.up * JumpForce, ForceMode.Impulse);
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
            animator.SetBool("jump", true);
            animator.SetLayerWeight(2, Mathf.Lerp(animator.GetLayerWeight(2), 1.0f, Time.deltaTime * 2.0f * Speed));
        }
        else if (isJumpOver)
        {
            animator.SetLayerWeight(2, Mathf.Lerp(animator.GetLayerWeight(2), 0.0f, Time.deltaTime * 1.5f * Speed));
            if (animator.GetLayerWeight(2) <= 0.08f)
            {
                animator.SetLayerWeight(2, 0.0f);
                isJumpOver = false;
                PerformAction = false;
            }
        }
    }

    // Проверка столкновений
    private void OnCollisionEnter(Collision collision)
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
        else if (collision.collider.tag == "Lake")
        {
            collision.collider.enabled = false;
            Speed = 2.0f;
            animator.SetBool("diving", true);
            var boxCollider = collision.collider.GetComponent<BoxCollider>();
            /* Расчёт с какого места воспроизводить анимацию погружения под воду
             * рассчитываются начальная и конечная точки коллайдера озера:
             * Берётся центральная точка коллайдера min = центр - пол ширины коллайдера, max = центр + пол ширины коллайдера*/
            Vector2 positionMinMax = new Vector2(collision.collider.transform.position.x - boxCollider.size.x / 2,
                collision.collider.transform.position.x + boxCollider.size.x / 2);
            /* рассчитывается позиция игрока относительно точки столкновения с коллайдером озера
             * вычитается min коллайдера озера и прибавляется радиус коллайдера игрока*/
            float curPosition = (float)Math.Round(transform.position.x - positionMinMax.x + GetComponent<CapsuleCollider>().radius, 1);
            /* нормализуется анимация погружения и с какого места её запускать, через процентное соотношение относительно точек min и max коллайдера озера*/
            animator.SetFloat("divingNormalized", ((curPosition * 100) / (positionMinMax.y - positionMinMax.x)) / 100);
            isDisableDivingAnimation = false;
            isEnableDivingAnimation = true;
        }
        else if (collision.collider.tag == "Oak") // Столкновение с дубом
        {
            collision.collider.enabled = false;
            animator.SetBool("clash_oak", true);
            SpeedClashWithsOak = Speed;
            Speed = .0f;
            animator.SetLayerWeight(2, .0f); // отключение анимационного слоя прыжка
            timerOakDown = 1.5f; //Назначение таймера, после которого начинается возрождения персонажа
            timeRespawnOak = TimeForRespawnOak; // Рестарт таймера возрождения
            PositionRespawn = collision.transform;
            isClashWithsOak = true;
        }
        else if (collision.collider.tag == "Pit") // Падение в яму
        {
            collision.collider.enabled = false;
            GetComponent<CapsuleCollider>().enabled = false;
            isCollisionPitDown = true;
            rigidbody.velocity = Vector3.zero;
            rigidbody.velocity = new Vector3(-Speed * .65f, -10.0f, 0);
            Speed -= .0f;
            timerPitDown = 1.5f; //Назначение таймера, после которого начинается возрождения персонажа
            timeRespawnPit = TimeForRespawnPit; // Рестарт таймера возрождения
            isPitDown = true;
            PositionRespawn = collision.transform;
            //GetComponent<CameraLookAtPlayer>().isLookAt = false;
        }
    }

    /* процедура настройки прозрачности анимационного слоя при столкновении с барьером*/
    private void CollisionWithBarrier()
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
        // Если время скольжения ещё не вышло
        if (timerSlipPuddle > 0)
        {
            // То плавно повышаем прозрачность слоя с анимацией подскальзывания до единицы
            animator.SetLayerWeight(5, Mathf.Lerp(animator.GetLayerWeight(5), 1.0f, Time.deltaTime * Speed));
            timerSlipPuddle -= Time.deltaTime;
        }
        else
        {
            // Если время скольжения вышло
            // то уменьшаем прозрачность слоя с анимацией скольжения до нуля
            animator.SetLayerWeight(5, Mathf.Lerp(animator.GetLayerWeight(5), .0f, Time.deltaTime * Speed));

            /* Когда прозрачность меньше 5%, то полное отключение слоя с анимацией скольжения
             * восставновления времени скольжения и отключение переменноё отвечающей за скольжение по луже*/
            if (animator.GetLayerWeight(5) <= 0.05f)
            {
                animator.SetLayerWeight(5, .0f);
                timerSlipPuddle = .5f;
                isCollisionStandingSlipPuddle = false;
            }
        }
    }

    // Если персонаж столкнулся с озером
    private void CollisionWithLake()
    {
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

    // Отключение анимации при выходе из озера
    void DisableDivingAnimation()
    {
        Speed = 5.0f;
        isEnableDivingAnimation = false;
        isDisableDivingAnimation = true;
        Energy = MaxEnergy;
    }

    // Если персонаж столкнулся с деревом
    private void CollisionWithOak()
    {
        animator.SetLayerWeight(9, Mathf.Lerp(animator.GetLayerWeight(9), 1.0f, Time.deltaTime * SpeedClashWithsOak * 1.5f));
        if (timerOakDown > 0)
        {
            timerOakDown -= Time.deltaTime;
        }
        else
        {
            if (timeRespawnOak > 0)
            {
                timeRespawnOak -= Time.deltaTime;
            }
            else
            {
                StartCoroutine(CharacterRespawnOak()); // Респавн после столкновения с деревом
            }
        }
    }

    //Респаун после столкновения с деревом
    IEnumerator CharacterRespawnOak()
    {
        isClashWithsOak = false; // отключение всех элементов после столкновения с деревом
        Speed = 5.0f;
        animator.SetBool("clash_oak", false); // отключение переменной в анимации
        animator.SetLayerWeight(9, .0f); // и отключение анимационного слоя с падением
        transform.position = new Vector3(PositionRespawn.position.x, StartPositionCharacter.y, PositionRespawn.position.z);
        animator.SetLayerWeight(7, 1.0f); // включение анимации мигания при возрождениии
        Energy = MaxEnergy; // восстановление энергии
        yield return new WaitForSeconds(1.5f); // задержка анимации возрождения
        animator.SetLayerWeight(7, .0f); // отключение слоя с анимацией мигания при возрождении
    }

    // Если персонаж упал в яму
    private void CollisionPitDown()
    {
        animator.SetLayerWeight(6, Mathf.Lerp(animator.GetLayerWeight(6), 1.0f, Time.deltaTime * 10));
        if (timerPitDown > 0)
        {
            timerPitDown -= Time.deltaTime;
        }
        else
        {
            if (timeRespawnPit > 0)
            {
                timeRespawnPit -= Time.deltaTime;
            }
            else
            {
                StartCoroutine(CharacterRespawnFromThePit()); // Респавн из ямы
            }
        }
    }

    // Респавн из ямы
    IEnumerator CharacterRespawnFromThePit()
    {
        GetComponent<CapsuleCollider>().enabled = true;
        isCollisionPitDown = false;
        Speed = 5.0f;
        isPitDown = false;
        animator.SetLayerWeight(6, .0f);
        transform.position = new Vector3(PositionRespawn.position.x, StartPositionCharacter.y, PositionRespawn.position.z);
        animator.SetLayerWeight(7, 1.0f);
        Energy = MaxEnergy;
        yield return new WaitForSeconds(1.5f);
        animator.SetLayerWeight(7, .0f);
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
                    PerformAction = false;
                }
            }
        }
    }
}
