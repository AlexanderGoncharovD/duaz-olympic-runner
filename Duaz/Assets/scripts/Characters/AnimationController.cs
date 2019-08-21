using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Vector2 rangeSpeed;
    private Animator animator;

    private void Start()
    {
        rangeSpeed = GetComponent<CharacterInfo>().RangeSpeed;
        animator = GetComponent<Animator>();
    }

    public void Speed(float speed)
    {
        animator.SetFloat("speed", 0.5f + ((speed * 100) / (rangeSpeed.y - rangeSpeed.x) * 0.5f) / 100.0f);
    }

    public void Run()
    {
        animator.SetBool("run", true);
    }

    public void Idle()
    {
        animator.SetBool("run", false);
    }

    // Сменить позицию персонажа на следующиую (на линии старта)
    public void NextStartumPosition(int numberPosition)
    {
        animator.SetInteger("start", numberPosition);
    }
}
