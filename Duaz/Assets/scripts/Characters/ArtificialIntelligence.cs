using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Скрипт управления экземпляром одного соперника
 Управление основывается на положении/действиях игрока*/
public class ArtificialIntelligence : MonoBehaviour
{
    public bool isStartGame, // Игра началась. Персонаж побежал со стартовой линии
        isFinish; // Если персонаж пересёк финишную линию

    void Start()
    {
        
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
    }
}
