using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Скрипт отвечает за свойства препятствия. Вешается на префаб препятствия*/
public class BarrierProperties : MonoBehaviour
{
    public float Mass; // Масса препятствия
    public float ReducedSpeed; // Индекс потери скорости при взаимодействии с препятствием
    public float PreviousBarrierDistance; // Дистанция необходимая от предыдущего препятствия
    public float NextBarrierDistance; // Дистанция необходимая до следущего  перпятствия
    public float RemovealRadiusDecoretion; // Радиус в котором удалаются декорации окружения

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
