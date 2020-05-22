using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    private Animator animator;

    [SerializeField] private float health = 100.0f;
    [SerializeField] private bool isPlayer;
    [SerializeField] private float damageMultiplier = 1.0f;

    private void Start()
    { 
        animator = GetComponent<Animator>();
    }
    
    public void ApplyDamage(float damage)
    {
        if (IsDead)
        {
            return;
        }

        damage *= damageMultiplier;

        health -= damage;
        if (animator)
        { 
            animator.SetBool("Run", false);
            animator.SetTrigger("GotHit"); 
        }
        
        if (health <= 0)
        {
            health = 0;
            if (animator)
            { 
                animator.SetTrigger("Dead");
                this.GetComponent<ZombieAi>().enabled = false;
            }
        }
    }

    public void SetHealth(float newHP)
    {
        health = newHP;
    }

    public bool IsDead
    {
        get
        { 
            return health <= 0;
        }
    }
}
