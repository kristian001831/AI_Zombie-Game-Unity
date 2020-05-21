using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    private Animator animator;

    [SerializeField] private float health = 100.0f;
    [SerializeField] private bool isPlayer;
    [SerializeField] private HealthManager referrer;
    [SerializeField] private float damageMultiplier = 1.0f;
    [SerializeField] private TextMeshProUGUI healthText;


    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isPlayer)
        {
            healthText.text = $"HP: {health.ToString()}";
        }
    }

    public void ApplyDamage(float damage)
    {
        if (IsDead)
        {
            return;
        }

        damage *= damageMultiplier;

        if (referrer)
        {
            referrer.ApplyDamage(damage);
        }
        else
        {
            health -= damage;

            if (health <= 0)
            {
                health = 0;
                if (animator)
                {
                    animator.SetTrigger("Dead");
                }
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
            if (!referrer)
            {
                return health <= 0;
            }
            else
            {
                return referrer.IsDead;
            }
        }
    }
}
