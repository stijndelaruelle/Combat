using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SetGameProperty : MonoBehaviour
{
    private enum GameProperty
    {
        PlayerCount,
        GameMode
    }

    [SerializeField]
    private GameProperty m_GameProperty;

    [SerializeField]
    private Text m_Text = null;

    public void ChangeGameProperty()
    {
        if (m_Text == null) return;

        GameplayManager gamePlayManager = GameplayManager.Instance;

        switch (m_GameProperty)
        {
            case GameProperty.PlayerCount:
            {
                gamePlayManager.IncreasePlayerCount();
                m_Text.text = gamePlayManager.CurrentPlayersCount + " Players";
                
                break;
            }
                
            case GameProperty.GameMode:
            {
                //Swap the game mode
                gamePlayManager.IncreaseGameMode();

                if (gamePlayManager.CurrentGameMode == GameplayManager.GameMode.SingleBulletMode)
                {
                    m_Text.text = "Single bullet";
                }

                else if (gamePlayManager.CurrentGameMode == GameplayManager.GameMode.InvisibleMode)
                {
                    m_Text.text = "Invisible";
                }

                break;
            }
                
            default:
                break;
        }
    }
}
