using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/*Код делает различные расчеты по интерфейсу */
public class Interface : MonoBehaviour {

    public GameObject Player; // Ссылка на го игрока
    public Transform FinishPoint; // точка финиша

    [Header("Метка дистанции до финиша")]
    public Text DistanceText; // текст дистанции доя финиша
    public int DistanceToFinish; // итог рассчета дистанции до финиша
    public GameObject UIFinishMarker; // графическая метка до финиша
    int DistanceDeactivedMarker = 30; // дистанция для отключения маркера 
    public bool isShowFinishMarker; // нужно ли показывать метку до финиша

    [Header("Запас энергии")]
    public float EnergyPercent; // Запас энергии в процентах
    public RectTransform EnergySprite; // ссылка на спрайт полосы энергии
    public GameObject EnergyText; // ссылка на текст шкалы энергии
    public bool isResizeEnergy; // для присвоения нового размера шкале
    Vector2 CurTargetSize; // текущий и к какому размеру стремится шкала энергии
    float EnergyMax; // один процент от всей энергии
    public float EnergySmooth = 1.0f; // скорость сглаживания изменения размера шкалы

    public Player PlayerScript; // ссылка на экзмеплер скрипта игрока
    Transform playerTr; // трансформ игрока

	// Use this for initialization
	void Start () {

        isShowFinishMarker = true;
        EnergyMax = PlayerScript.MaxEnergy;
        EnergyPercent = (EnergyMax * 100.0f) / EnergyMax;
        CalculationSizeEnergyBar();
    }
	
	// Update is called once per frame
	void Update () {

        playerTr = Player.transform;

        // рассчет и показ дистанции до финиша
        if (isShowFinishMarker)
        {
            CalculateDistanceToFinish();
        }

        // присвоение нового размера шкале энргии
        if (isResizeEnergy)
        {
            EnergySprite.localScale = Vector3.Lerp(new Vector3(EnergySprite.localScale.x, 1, 1), new Vector3(CurTargetSize.y, 1, 1), Time.deltaTime * EnergySmooth);
            if (EnergySprite.localScale.x < 0)
            {
                EnergySprite.localScale = new Vector3(0, 1, 1);
            }
            if (EnergyText.activeSelf)
            {
                if (Math.Round(EnergySprite.localScale.x, 2)  <= 0.13f)
                {
                    EnergyText.SetActive(false);
                }
            }
            else
            {
                if (Math.Round(EnergySprite.localScale.x, 2) > 0.13f)
                {
                    EnergyText.SetActive(true);
                }
            }
            if (EnergySprite.localScale.x == CurTargetSize.y)
            {
                isResizeEnergy = false;
            }
        }

    }

    // рассчет и показ дистанции до финиша
    void CalculateDistanceToFinish()
    {
        DistanceToFinish = Mathf.CeilToInt((FinishPoint.position - playerTr.position).magnitude);
        if (DistanceToFinish < DistanceDeactivedMarker)
        {
            UIFinishMarker.SetActive(false);
            isShowFinishMarker = false;
        }
        else
        {
            UIFinishMarker.SetActive(true);
        }
        DistanceText.text = DistanceToFinish + " m";
    }

    public void CalculationSizeEnergyBar()
    {
        isResizeEnergy = true;
        EnergyPercent = (PlayerScript.Energy * 100.0f) / EnergyMax;
        CurTargetSize.x = EnergySprite.localScale.x;
        CurTargetSize.y = (1.0f * EnergyPercent) / 100.0f;
    }
}
