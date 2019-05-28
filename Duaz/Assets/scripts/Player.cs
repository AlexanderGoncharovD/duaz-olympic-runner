using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Vector2 Speed = new Vector2(2, 5); // Минимальная и максимальная скорость игрока
    public float Energy = 100.0f;  // запас энергии
    public bool run;
    public bool RunFast;

    public Interface Interface; // ссылка на скрипт

    Animator animator;

    // Use this for initialization
    void Start() {
        animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() {
        if (run)
        {
            this.transform.position += Vector3.right * Mathf.Lerp(0, Speed.x, Time.time / 2) * Time.deltaTime;
        }
       if(Input.GetKeyDown(KeyCode.Escape))
        {
            RunFast = !RunFast;
            animator.SetBool("run_fast", RunFast);
        }

        if (RunFast)
        {
            if (Speed.x < 10)
            {
                Speed.x += Time.deltaTime * 6;
            }
        }
        else
        {
            if (Speed.x > 4)
            {
                Speed.x -= Time.deltaTime * 3;
            }
        }

        if(Input.GetMouseButtonDown(0))
        {
            if (Energy > 0)
            {
                Energy -= 5.0f;
                Interface.CalculationSizeEnergyBar();
            }
            else
            {
                Energy = 0;
            }
        }
    }

    void AnimationStartAnimSpeedZero()
    {
        animator.SetFloat("speed", 0.0f);
    }

    void AnimationStartAnimSpeedRun()
    {
        animator.SetFloat("speed", 1.0f);
        animator.SetBool("run", true);
        run = true;
        this.GetComponent<Parallax>().enabled = true;
    }
    
    
}
