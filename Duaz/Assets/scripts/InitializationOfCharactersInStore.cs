using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct DateCharacter
{
    public string Name, Energy, Boost, Respawn; //имя персонажа
    public GameObject UIPrefab; // анимированный префаб персонажа из UI элементов
    public GameObject Prefab; // Игровой префаб персонажа
}

public class InitializationOfCharactersInStore : MonoBehaviour
{
    public int Active; // Номер активной карточки с персонажем
    public GameObject SelectedCharacter; // Выбранный персонаж
    public int IdSelectedCharacter; // ID выбранного персонажа
    public DateCharacter[] Character; // список всех персонажей в магазине
    public GameObject Element; // ссылка на балванку элемента магазина
    public GameObject[] Elements; // Массив со всеми элементами магазина
    public float Index = 185.0f; // индекс смещения элемента в магазине

    [Space]
    public float SpeedSwipe, // Скорость перелистывания родительского объекта элементов магазина
           CurSpeedSwipe; // Изменяемая скорость свайпа
    public GameObject ButtonBuy, ButtonUpgrade, ButtonSelect; // Ссылки на объекты кнопок для отображения

    public Text debug;
    private Vector3 elementPosition; // позиция сгенерированного эелемнта магазина
    private GameObject newElement; // ссылка на только что сгенерированный элемент магазина
    private SettingsElementCharacterInStore parameter; // ссылка на настройки сгенерированного элемента магазина
    private Transform parent; // Родительский объект элементов магазина
    private Vector2 Range; // Диапазон прокрутки элементов магазин по X от min до max
    private float CurPosition; // Текущая позиция родительского объекта элементов магазина
    private bool swipe, // Совершо свайп - перелистывание элементов магазина
        isRase, // True - скорость свайпа возрастает; False -  скорость свайпа убывает
        moveEndElement; // Перемещение элемента в конечную точку (центр окна) после свайпа
    private float delta, // Знак свайпа определяющий сторону свайпа
        newOffset; // Новое смещение для элементов магазина
    private int idSelectedElement = -1; // активный элемент магазина после сайпа
    private bool resizeCards; // Изменение размеров карточек активных и неактивных

    void Start ()
    {
        // Определение минимума и максимума прокрутки элементов по Х
        Range = new Vector2(Element.transform.localPosition.x, -Index * (Character.Length-1));
        // Присвоение стартовой позиции родительскому объекту
        CurPosition = Range.x;
        // Определение положения первого элемента магазина
        elementPosition = new Vector3(Element.transform.localPosition.x, 19.0f, .0f);
        // Определение количества элементво в магазине
        Elements = new GameObject[Character.Length];
        parent = Element.transform.parent;
        // Инициализация всех элементво магазина
        GenerationElementsInStore();
        LoadingCharacterParameters();

    }

    // Инициализация всех персонажей и их карточек в магазине
    private void GenerationElementsInStore ()
    {
        for(int i = 0; i < Character.Length; i++)
        {
            newElement = Instantiate(Element, Element.transform.position, Quaternion.identity);

            parameter = newElement.GetComponent<SettingsElementCharacterInStore>();
            newElement.transform.parent = parent;
            newElement.transform.localPosition = elementPosition;
            newElement.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
#if UNITY_EDITOR
            newElement.name = Character[i].Name;
#endif
            elementPosition += new Vector3(Index, .0f, .0f);

            Elements[i] = newElement;
            // Спаун миниатюры персонажа для карточки в магазине
            if (Character[i].UIPrefab != null)
            {
                newElement = Instantiate(Character[i].UIPrefab, parameter.CharacterPointSpawn.position, Quaternion.identity);
                newElement.transform.parent = parameter.CharacterPointSpawn;
            }
            // Присвоение характеристик для отображения под карточкой в магазине
            parameter.Id = i;
            parameter.Name = Character[i].Name;
            parameter.Energy = Character[i].Energy;
            parameter.Boost = Character[i].Boost;
            parameter.Respawn = Character[i].Respawn;
            parameter.Apply();
        }
        Destroy(Element);
    }

    // Загрузка всех параметров для каждого персонажа
    private void LoadingCharacterParameters()
    {
        Elements[0].GetComponent<SettingsElementCharacterInStore>().isBuy = true;
        Elements[0].GetComponent<SettingsElementCharacterInStore>().isSelected = true;
        Elements[0].GetComponent<SettingsElementCharacterInStore>().SelectText.SetActive(true);
        swipe = true;
    }
    void Update()
    {
        parent.localPosition = new Vector3(CurPosition, .0f, .0f);
        // Провекрка свайпов
        TouchScreen();

        if (swipe)
        {
            if (isRase)
            {
                if (CurSpeedSwipe < SpeedSwipe * 0.9f)
                {
                    CurSpeedSwipe = Mathf.Lerp(CurSpeedSwipe, SpeedSwipe, Time.deltaTime * 5);

                }
                else
                {
                    isRase = false;
                }
            }
            else
            {
                if (CurSpeedSwipe > SpeedSwipe * 0.30f)
                {
                    CurSpeedSwipe = Mathf.Lerp(CurSpeedSwipe, 0, Time.deltaTime * 5);
                }
                else
                {
                    if (idSelectedElement == -1)
                    {
                        idSelectedElement = SearchActiveCard();
                    }
                    else
                    {
                        moveEndElement = true;
                        CurSpeedSwipe = 0;
                        swipe = false;
                    }
                }
            }
            newOffset = delta * CurSpeedSwipe * Time.deltaTime;
            if (CurPosition + newOffset > Range.y && CurPosition + newOffset < Range.x)
            {
                CurPosition += delta * CurSpeedSwipe * Time.deltaTime;
            }
            // Выводит Id активной карточки
            Active = SearchActiveCard();
        }
        ReSizeCards(Active);
        // Центрирование ближайшего к центру элемента, когда скорость перемещения всех элементов придельна мала
        if (moveEndElement)
        {
            CurPosition = Mathf.Lerp(CurPosition, -Elements[idSelectedElement].transform.localPosition.x, Time.deltaTime * 5f);
            // Если персонаж куплен, то выводить кнопку улучшения, иначе выводить кнопку покупки
            if (Elements[Active].GetComponent<SettingsElementCharacterInStore>().isBuy)
            {
                // Если персонаж выбран и активрован, то выводится кнопка улучшить, иначе выводится кнопка выбрать
                if (Elements[Active].GetComponent<SettingsElementCharacterInStore>().isSelected)
                {
                    ShowUpgrade();
                }
                else
                {
                    ShowSelect();
                }
            }
            else
            {
                ShowBuy();
            }
        }
    }

    private Vector2 startPos; // Стартовая позиция прикосновения
    void TouchScreen()
    {
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
                        idSelectedElement = -1;
                        moveEndElement = false;
                        break;

                    // Если было совершено перемещение пальцем
                    case TouchPhase.Moved:
                        // Если свайп и длина свайпа больше 5% ширины экрана
                        if (Mathf.Abs(startPos.x - touch.position.x) > Screen.width * 0.05f)
                        {
                            isRase = true;
                            swipe = true;
                            if (touch.deltaPosition.x > 55.0f)
                            {
                                delta = 55.0f;
                            }
                            else if (touch.deltaPosition.x < -55.0f)
                            {
                                delta = -55.0f;
                            }
                            else
                            {
                                delta = touch.deltaPosition.x;
                            }
                            HiddenButtons();
                        }
                        break;

                    // Если палец был убран с экрана
                    case TouchPhase.Ended:
                       
                        break;
                }
            }
        }
    }

    // Изменение размеров активной и неактивной карточкиперсонажа
    private float sizeCardActive, sizeCardOldActive, minSizeCard = 1.0f, maxSizeCard = 1.1f;
    private void ReSizeCards(int active)
    {
        if (Elements[active].transform.localScale.x < maxSizeCard * 0.99f)
        {
            sizeCardActive = Mathf.Lerp(Elements[active].transform.localScale.x, maxSizeCard, 6 * Time.deltaTime);
        }
        else
        {
            sizeCardActive = 1.1f;
        }
        Elements[active].transform.localScale = new Vector3(sizeCardActive, sizeCardActive, sizeCardActive);

        for (int i = 0; i < Elements.Length; i++)
        {
            if (i != active)
            {
                if (Elements[i].transform.localScale.x > minSizeCard * 1.01f)
                {
                    sizeCardOldActive = Mathf.Lerp(Elements[i].transform.localScale.x, minSizeCard, 6 * Time.deltaTime);
                }
                else
                {
                    sizeCardOldActive = 1.0f;
                }
                Elements[i].transform.localScale = new Vector3(sizeCardOldActive, sizeCardOldActive, sizeCardOldActive);
            }
        }
    }

    /* Поиск активной акрточки с персонажем
     * Выполняется поиск для дальнейшего масштабирования карточки
     * Или для дальнейшей активации персонажа*/
    private int number;
    private int SearchActiveCard()
    {
        for (int i = 0; i < Elements.Length; i++)
        {
            if (Mathf.Abs(CurPosition) >= Elements[i].transform.localPosition.x - Index / 2
                && Mathf.Abs(CurPosition) <= Elements[i].transform.localPosition.x + Index / 2)
            {
                number = i;
            }
        }
        return number;
    }

    // Активация кнопки Покупка персонажа "Купить"
    public void BuyCharacter()
    {
        // Активируется покупка, показывается текст "Выбран" и ставится галочка "Выбран"
        Elements[Active].GetComponent<SettingsElementCharacterInStore>().isBuy = true;
        // Выбрать текущего персонажа
        SelectCharacter();
    }
    // Улучшить параметры текущего персонажа5
    public void UpgradeCharacter()
    {

    }
    // Активация кнопки выбрать текущего персонажа
    public void SelectCharacter()
    {
        // У предыдущего активного персонажа отключается текст "Выбран" и снимается галочка "Выбран"
        Elements[IdSelectedCharacter].GetComponent<SettingsElementCharacterInStore>().SelectText.SetActive(false);
        Elements[IdSelectedCharacter].GetComponent<SettingsElementCharacterInStore>().isSelected = false;
        // Определеятся новый выбранный персонаж
        IdSelectedCharacter = Active;
        SelectedCharacter = Character[Active].Prefab;
        Elements[Active].GetComponent<SettingsElementCharacterInStore>().SelectText.SetActive(true);
        Elements[Active].GetComponent<SettingsElementCharacterInStore>().isSelected = true;
        // Смена кнопки с "купить на "улучшить"
        ShowUpgrade();
    }

    // Показать кнопку "купить"
    private void ShowBuy()
    {
        ButtonBuy.SetActive(true);
        ButtonUpgrade.SetActive(false);
        ButtonSelect.SetActive(false);
    }
    // Показать кнопку "улучшить"
    private void ShowUpgrade()
    {
        ButtonUpgrade.SetActive(true);
        ButtonBuy.SetActive(false);
        ButtonSelect.SetActive(false);
    }
    //Показать кнопку "выбрать"
    private void ShowSelect()
    {
        ButtonSelect.SetActive(true);
        ButtonBuy.SetActive(false);
        ButtonUpgrade.SetActive(false);
    }
    // Скрыть все кнопки
    private void HiddenButtons()
    {
        ButtonSelect.SetActive(false);
        ButtonBuy.SetActive(false);
        ButtonUpgrade.SetActive(false);
    }

    //Закрытие окна магазина выбора персонажа
    public void CloseStore()
    {
        gameObject.SetActive(false);
    }
}
