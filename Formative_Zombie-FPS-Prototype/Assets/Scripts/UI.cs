using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField] private Text ammoLeftText;
    [SerializeField] private Text bulletsInMagazineText;
    
    private Pistol pistolCs;

    void Start()
    {
        pistolCs = new Pistol();
    }

    void Update()
    {
        ammoLeftText.text = $"Bullets left: {pistolCs.BulletsLeft}";
        bulletsInMagazineText.text = $"Magazine: {pistolCs.LoadedBullets}";
    }
}
