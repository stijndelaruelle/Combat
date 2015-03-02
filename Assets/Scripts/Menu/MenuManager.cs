using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    private void Awake()
    {
        OpenMenu(true);
    }

    private void Update()
    {
        if (Input.GetButtonDown("MenuOpen"))
        {
            OpenMenu(true);
        }

        if (Input.GetButtonDown("MenuCancel"))
        {
            //If the game already had an initial setup
            if (GameplayManager.Instance.IsInitialized)
            {
                OpenMenu(false);
            }
        }
    }

    private void OpenMenu(bool value)
    {
        if (value == true)
        {
            EnableChildren(true);
            Time.timeScale = 0.0f;
        }
        else
        {
            EnableChildren(false);
            Time.timeScale = 1.0f;
        }
    }

    private void EnableChildren(bool value)
    {
        for (int i = 0; i < transform.childCount; ++i)
        {
            transform.GetChild(i).gameObject.SetActive(value);
        }
    }

    public void StartGame()
    {
        GameplayManager.Instance.StartGame();
        OpenMenu(false);
    }

    public string IncreasePlayerCount()
    {
        GameplayManager gamePlayManager = GameplayManager.Instance;

        gamePlayManager.IncreasePlayerCount();
        return gamePlayManager.CurrentPlayersCount + " Players";
    }

    public string IncreaseGameMode()
    {
        GameplayManager gamePlayManager = GameplayManager.Instance;
        gamePlayManager.IncreaseGameMode();

        if (gamePlayManager.CurrentGameMode == GameplayManager.GameMode.SingleBulletMode)
        {
            return "Single bullet";
        }

        if (gamePlayManager.CurrentGameMode == GameplayManager.GameMode.InvisibleMode)
        {
            return "Invisible";
        }

        return "";
    }
}
