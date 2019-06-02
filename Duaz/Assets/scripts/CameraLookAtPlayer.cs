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
    public Interface Interface;
    Vector3 xMoveCamera, xLateMoveCamera; // х камеры до и после кадра

    bool isLookAt = true; // нужно ли в анный момент следить за игроком
    Vector3 playerStartPoint; // точка, из которой старует игрок
    Camera camera;
    Transform MainCamera;

	// Use this for initialization
	void Start ()
    {
        playerStartPoint = Player.position;
        MainCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;
        camera = MainCamera.gameObject.GetComponent<Camera>();
    }

    // Update is called once per frame
	void LateUpdate()
    {
        xMoveCamera = Camera.position;

        // Слежение камеры за игрком
        if (isLookAt)
        {
            // Если игрок стартанул и пробежал больше указаной дистанции, то скорость прислеживания камеры постепенно увеличивается
            if ((playerStartPoint - Player.position).magnitude > DistanceFromStart)
            {
                smooth = Mathf.Lerp(smooth, GetComponent<Player>().Speed / 4.5f, Time.deltaTime);
            }

            // Плавное ускорение приследования камеры за игроком
            Camera.position = new Vector3(Mathf.Lerp(Camera.position.x, Player.position.x, Time.deltaTime * smooth),
                Mathf.Lerp(Camera.position.y, Player.position.y, Time.deltaTime * smooth*3), -10);

            // отдаление камеры
            // Если скорость персонажа больше 80% от максимальной 
            if(GetComponent<Player>().Speed >= GetComponent<Player>().RangeSpeed.y*0.8f)
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