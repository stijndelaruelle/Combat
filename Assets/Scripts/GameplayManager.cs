using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameplayManager : MonoBehaviour
{
    public enum GameMode
    {
        InvisibleMode,
        SingleBulletMode,
    }

    [SerializeField]
    private int m_MaxScore;

    private Tank[] m_Players;
    private Scoreboard m_Scoreboard = null;

    [SerializeField]
    private GameMode m_GameMode = GameMode.InvisibleMode;
    public GameMode CurrentGameMode
    {
        get { return m_GameMode; }
    }

    [SerializeField]
    private Text m_GameModeLabel;

    //Singleton
    private static GameplayManager m_Instance;
    public static GameplayManager Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = GameObject.FindObjectOfType<GameplayManager>();
            }

            return m_Instance;
        }
    }

    void Awake()
    {
        if (m_Instance == null)
        {
            //If I am the first instance, make me the Singleton
            m_Instance = this;
        }
        else
        {
            //If a Singleton already exists and you find
            //another reference in scene, destroy it!
            if (this != m_Instance)
                Destroy(this.gameObject);
        }
    }

	// Use this for initialization
	private void Start ()
    {
        m_Players = GameObject.FindObjectsOfType<Tank>();
        m_Scoreboard = GameObject.Find("Scoreboard").GetComponent<Scoreboard>();

        SetGameMode(m_GameMode);
	}

    private void Update()
    {
        //Reset the game
        if (Input.GetButtonDown("Start")) { ResetGame(); }

        //Loop gamemode
        if (Input.GetButtonDown("Select"))
        {
            if (m_GameMode == GameMode.InvisibleMode) SetGameMode(GameMode.SingleBulletMode);
            else                                      SetGameMode(GameMode.InvisibleMode);

            ResetGame();
        }

        //Switch level
        if (Input.GetButtonDown("SwapLevel"))
        {
            if (Application.loadedLevelName == "level1")
            {
                Application.LoadLevel("level2");
            }
            else
            {
                Application.LoadLevel("level1");
            }
        }
    }

    private void ResetGame()
    {
        StopAllCoroutines();

        //Reset the level
        ResetLevel();

        //Reset all the scores
        m_Scoreboard.ResetScores();
    }

    private void SetGameMode(GameMode gameMode)
    {
        m_GameMode = gameMode;

        switch (m_GameMode)
        {
            case GameMode.InvisibleMode:
            {
                m_GameModeLabel.text = "Invisible Mode";
                break;
            }

            case GameMode.SingleBulletMode:
            {
                m_GameModeLabel.text = "Single Bullet Mode";
                break;
            }

            default:
                break;
        }
    }

	private void ResetLevel()
    {
        //SUPER LAME SERIOUSLY

        //Respawn all the players
        foreach (Tank player in m_Players)
        {
            player.Respawn();
            player.InputEnabled = true;
        }

        //Remove all the bullets
        Bullet[] bullets = GameObject.FindObjectsOfType<Bullet>();

        for (int i = bullets.Length - 1; i >= 0; --i)
        {
            GameObject.Destroy(bullets[i].gameObject);
        } 
    }

    public void UpdateScore(int playerID, int deltaScore)
    {
        int newScore = m_Scoreboard.UpdateScore(playerID, deltaScore);

        //Disable player input for a bit
        foreach (Tank player in m_Players)
        {
            player.InputEnabled = false;
        }

        if (newScore >= m_MaxScore)
        {
            Debug.Log(playerID + " wins!");
            m_Scoreboard.SetWinner("Player " + (playerID + 1));
        }
        else
        {
            StartCoroutine(ResetLevelRoutine());
        }
    }

    private IEnumerator ResetLevelRoutine()
    {
        yield return new WaitForSeconds(m_Players[0].InvisibleSpeed);
        ResetLevel();
    }
}
