using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathMenu : MonoBehaviour
{
    public void RestartLevel(string level)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Player player = new Player();
        player.IsDead = false;
        Time.timeScale = 0f; // pauses the scene 
        SceneManager.LoadScene(level);
    }
}
