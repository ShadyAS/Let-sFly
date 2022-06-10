using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool gameIsOn = true;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PauseGame()
    {
        gameIsOn = !gameIsOn;

        Cursor.visible = !gameIsOn;
        
        Time.timeScale = gameIsOn ? 1.0f : 0.0f;
    }
}
