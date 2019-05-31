using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Скрипт отывечает за приследование игрока камерой*/
public class CameraLookAtPlayer : MonoBehaviour {

    public Transform Player;
    public Transform Camera;
    public float smooth = 1.0f; // скорость сглаживания приследования 
    public float DistanceFromStart = 4.0f; // Дистаниция, которую игрок пробегает до начала приследования камерой
    public float SpeedMoveCamera; // Скорость перемещения камера за кадр
    Vector3 xMoveCamera, xLateMoveCamera; // х камеры до и после кадра

    bool isLookAt = true; // нужно ли в анный момент следить за игроком
    Vector3 playerStartPoint; // точка, из которой старует игрок

	// Use this for initialization
	void Start () {

        playerStartPoint = Player.position;
    }

    // Update is called once per frame
	void LateUpdate() {
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
                Mathf.Lerp(Camera.position.y, Player.position.y, Time.deltaTime * smooth), -10);
        }
        xLateMoveCamera = Camera.position;
        SpeedMoveCamera = (xMoveCamera - xLateMoveCamera).magnitude / Time.deltaTime;
    }
}
