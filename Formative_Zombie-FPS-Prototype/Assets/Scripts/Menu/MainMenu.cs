using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string levelName = "Level1";

    public void StartButton()
    {
        SceneManager.LoadScene(levelName);
    }
}
