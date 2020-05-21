using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private HealthManager healthManager;
    
    
    void Start()
    {
        healthManager = GetComponent<HealthManager>();
    }
    
    void Update()
    {
        
    }
}
