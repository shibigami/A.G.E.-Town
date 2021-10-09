using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager Instance = null;

    public PlayerController Player;

    public GameObject GameOverHUD;

    public GameObject GameWonHUD;

    public float ShowGameOverTime = 1.5f;

    public float ShowGameWonTime = 1.0f;

    public event EventHandler GameOver;

    public event EventHandler GameWon;

    public GameWinCondition WinCondition;

    void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
        }

        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        // Dont destroy on reloading the scene
        // DontDestroyOnLoad(gameObject);

        if (Player == null)
        {
            Debug.LogError("You need to assign a Player to the GameManager");
        }

        if (Player != null)
        {
            Player.PlayerDied += Player_PlayerDied;
        }
    }

    private void Player_PlayerDied(object sender, System.EventArgs e)
    {
        SetGameOver();
    }

    public void SetGameOver()
    {
        if (Player.Hud == null)
        {
            Debug.LogError("You need to assign a HUD to the PlayerController");
        }
        else
        {
            Player.Hud.gameObject.SetActive(false);

            if (GameOverHUD == null)
            {
                Debug.LogError("You need to assign a GameOverHUD to the GameManager");
            }
            else
            {
                GameOverNotify();

                Invoke("ShowGameOver", ShowGameOverTime);
            }
        }
    }

    public void SetGameWon()
    {
        if (Player.Hud == null)
        {
            Debug.LogError("You need to assign a HUD to the PlayerController");
        }
        else
        {
            Player.Hud.gameObject.SetActive(false);

            if (GameWonHUD == null)
            {
                Debug.LogError("You need to assign a GameWonHUD to the GameManager");
            }
            else
            {
                GameWonNotify();

                Invoke("ShowGameWon", ShowGameWonTime);
            }
        }
    }

    private void GameOverNotify()
    {
        if (GameOver != null)
            GameOver(this, EventArgs.Empty);
    }

    private void GameWonNotify()
    {
        if (GameWon != null)
            GameWon(this, EventArgs.Empty);
    }

    private void ShowGameOver()
    {
        GameOverHUD.SetActive(true);
    }

    private void ShowGameWon()
    {
        GameWonHUD.SetActive(true);
    }

    public void RetryGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
   
}
