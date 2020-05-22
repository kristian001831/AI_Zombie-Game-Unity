using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private HealthManager healthManager;
    [SerializeField] private GameObject deadScreen;
    SafeZone safeZone = new SafeZone();

    private bool isDead;
    public bool IsDead
    {
        get => isDead;
        set => isDead = value;
    }


    private void Start()
    {
        healthManager = GetComponent<HealthManager>();
    }
 
    private void Update()
    {
        if(healthManager.IsDead) 
        {
            ShowDeadScreen();
        }

        if (!isDead && !safeZone.IsWon)
        {
            Time.timeScale = 1f;
            this.GetComponent<PlayerController>().enabled = true;
        }
        else
        {
            this.GetComponent<PlayerController>().enabled = false;
        }
        
    }
    
    private void ShowDeadScreen()
    {
        deadScreen.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isDead = true;
        Time.timeScale = 0f; // pauses the scene 
    }
}
