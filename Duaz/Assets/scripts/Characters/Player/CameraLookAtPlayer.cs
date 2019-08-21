using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Скрипт отывечает за приследование игрока камерой*/
public class CameraLookAtPlayer : MonoBehaviour
{
    public Transform Player;
    public Transform Camera;
    public float smooth = 1.0f; // скорость сглаживания приследования 
    public float DistanceFromStart = 4.0f; // Дистаниция, которую игрок пробегает до начала приследования камерой
    public float SpeedMoveCamera; // Скорость перемещения камера за кадр
    public bool isReset; // Перезапуск слежения камеры за персонажем
    public Interface Interface;
    public bool isFinish; // если игрок финишировал
    Vector3 xMoveCamera, xLateMoveCamera; // х камеры до и после кадра

    public bool isLookAt = true; // нужно ли в анный момент следить за игроком
    Vector3 playerStartPoint; // точка, из которой старует игрок
    Camera camera;
    Transform MainCamera;
    private MoveController moveController;

    private void Start()
    {
        MainCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;
        camera = MainCamera.gameObject.GetComponent<Camera>();
        Camera = MainCamera.parent;
        Interface = camera.GetComponent<Interface>();
    }

    public void Activate ()
    {
        playerStartPoint = Player.position;
        moveController = Player.gameObject.GetComponent<MoveController>();
        isLookAt = true;
    }

    // Update is called once per frame
	void LateUpdate()
    {
        xMoveCamera = Camera.position;

        // Слежение камеры за игрком
        if (isLookAt)
        {
            if (!isReset && !isFinish)
            {
                /* формула позиции у камеры size min 5 и max 7 - берется процент умножается на процент от min -1,5 max -2.4
                    * и все это вычитается из max 2.4 и прибаляется индекс 0.9 потому что камера плавно перемещается и нужен запас расстояния*/
                if (Camera.position.y > -2.3f - ((-0.9f / 100) * ((100 * (camera.orthographicSize - 5)) / 2)) + 0.9f)
                {
                    // Если игрок стартанул и пробежал больше указаной дистанции, то скорость прислеживания камеры постепенно увеличивается
                    if ((playerStartPoint - Player.position).magnitude > DistanceFromStart)
                    {
                        smooth = Mathf.Lerp(smooth, moveController.Speed / 4.5f, Time.deltaTime);
                    }
                }
                else
                {
                    if (smooth >= 0.01f)
                    {
                        smooth = Mathf.Lerp(smooth, 0, Time.deltaTime * 10);
                    }
                    else
                    {
                        smooth = 0.0f;
                        isLookAt = false;
                    }
                }
            }
            else
            {
                if (isFinish)
                {
                    smooth = Mathf.Lerp(smooth, 0.0f, Time.deltaTime*2);
                }
                else
                {
                    smooth = Mathf.Lerp(smooth, moveController.Speed / 4.5f, Time.deltaTime);
                    if (smooth >= 1.0f)
                    {
                        isReset = false;
                    }
                }
            }

            
            // Плавное ускорение приследования камеры за игроком
            Camera.position = new Vector3(Mathf.Lerp(Camera.position.x, Player.position.x, Time.deltaTime * smooth),
                Mathf.Lerp(Camera.position.y, Player.position.y, Time.deltaTime * smooth*3), -10);

            // отдаление камеры
            // Если скорость персонажа больше 80% от максимальной 
            if(moveController.Speed >= moveController.RangeSpeed.y*0.8f)
            {
                // Плавное отдаление размера ортоганали камеры через Lerp
                if (camera.orthographicSize < 7)
                {
                    camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, 7.0f, Time.deltaTime/2);
                }
                else if (camera.orthographicSize > 7)
                {
                    camera.orthographicSize = 7;
                }

                // Плавное отдаление камеры по х (чтобы персонаж всегда оставлся слевой стороны)
                if (MainCamera.localPosition.x < 13.0f)
                {
                    MainCamera.localPosition = new Vector3(Mathf.Lerp(MainCamera.localPosition.x, 13.0f, Time.deltaTime/2), 0, 0);
                }
                else if (MainCamera.localPosition.x > 13.0f)
                {
                    MainCamera.localPosition = new Vector3(13.0f, 0, 0);
                }
            }
            else
            {
                // Плавное уменьшение размера ортоганали камеры
                if (camera.orthographicSize > 5)
                {
                    camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, 5.0f, Time.deltaTime);
                }
                else if (camera.orthographicSize < 5)
                {
                    camera.orthographicSize = 5;
                }

                // Плавное смещение камеры по х
                if (MainCamera.localPosition.x > 9.0f)
                {
                    MainCamera.localPosition = new Vector3(Mathf.Lerp(MainCamera.localPosition.x, 9.0f, Time.deltaTime), 0, 0);
                }
                else if (MainCamera.localPosition.x < 9.0f)
                {
                    MainCamera.localPosition = new Vector3(9.0f, 0, 0);
                }
            }

            // Рассчёт дистанции текста (указывает расстояние до финиша) от размера ортоганали камеры камеры
            Interface.DistanceTextToCamera = camera.orthographicSize * 1.7f;
        }
        xLateMoveCamera = Camera.position;
        SpeedMoveCamera = (xMoveCamera - xLateMoveCamera).magnitude / Time.deltaTime;
    }
}