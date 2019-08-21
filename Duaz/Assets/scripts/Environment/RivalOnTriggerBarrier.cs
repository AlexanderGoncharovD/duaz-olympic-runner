using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum action
{
    NoAction,
    Jump,
    RolledUp
}

public class RivalOnTriggerBarrier : MonoBehaviour {

    public action expectedAction; // Какое действие ожидается от соперника для преодоления препятствия

    
}
