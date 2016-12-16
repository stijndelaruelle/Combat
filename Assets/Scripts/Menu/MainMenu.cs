using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject m_MainButton;

    private void OnEnable()
    {
        OpenMenu(true);

        if (m_MainButton != null)
        {
            EventSystem.current.SetSelectedGameObject(m_MainButton);
        }
    }

    private void OnDisable()
    {
        OpenMenu(false);
    }

    public void OpenMenu(bool value)
    {
        if (value == true)
        { 
            Time.timeScale = 0.0f;
        }
        else
        {
            Time.timeScale = 1.0f;
        }

        gameObject.SetActive(value);
        EnableChildren(value);
    }

    private void EnableChildren(bool value)
    {
        for (int i = 0; i < transform.childCount; ++i)
        {
            transform.GetChild(i).gameObject.SetActive(value);
        }
    }

    public void StartCountDown()
    {
        GameplayManager.Instance.StartCountDown();
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
