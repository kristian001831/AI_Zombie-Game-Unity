using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : MonoBehaviour
{
    private Transform origin;
    private Navigation navigation = new Navigation();
    public float Speed;
    
    void Start()
    {
        origin = GetComponent<Transform>();
        Navigation.Origins.Add(origin);
    }
}
