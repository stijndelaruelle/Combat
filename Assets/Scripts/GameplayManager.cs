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

    public enum VictoryCondition
    {
        Territory,
        Score,
    }

    //Events
    public event VoidDelegate OnResetGame;
    public event IntReturnIntDelegate OnUpdateScore;
    public event SubStageBoolDelegate OnUpdateMap;
    public event StringDelegate OnSetWinner;

    //Datamembers
    [SerializeField]
    private int m_MaxScore;

    [SerializeField]
    private Tank[] m_Players;

    [SerializeField]
    private GameMode m_GameMode = GameMode.InvisibleMode;
    public GameMode CurrentGameMode
    {
        get { return m_GameMode; }
    }

    [SerializeField]
    private VictoryCondition m_VictoryCondition = VictoryCondition.Territory;
    public VictoryCondition CurrentVictoryCondition
    {
        get { return m_VictoryCondition; }
    }

    [SerializeField]
    private Text m_GameModeLabel;

    [SerializeField]
    private SubStage m_StartStage; //The stage where it all begins
    private SubStage m_CurrentStage;
    private SubStage m_LastStage;

    private int m_CurrentStageOwner = -1;

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
        m_CurrentStage = m_StartStage;
        m_LastStage = null;

        if (m_VictoryCondition == VictoryCondition.Territory)
        {
            if (OnUpdateMap != null) OnUpdateMap(m_CurrentStage, true);
        }

        SetGameMode(m_GameMode);
        ResetGame();
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
    }

    private void ResetGame()
    {
        StopAllCoroutines();

        //Reset the level
        m_CurrentStage.Deactivate();
        m_StartStage.Activate();

        m_CurrentStage = m_StartStage;
        
        m_CurrentStageOwner = -1;

        ResetLevel();

        if (OnResetGame != null) OnResetGame();
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
        //Set the camera
        Vector3 newPosition = m_CurrentStage.transform.position;
        newPosition.z = Camera.main.transform.position.z;

        Camera.main.transform.position = newPosition;

        //Respawn all the players
        for (int i = 0; i < m_Players.Length; ++i)
        {
            m_Players[i].Respawn(m_CurrentStage.GetSpawnTransform(i));
            m_Players[i].SetColor(m_CurrentStage.GetPlayerColor(i));
            m_Players[i].InputEnabled = true;
        }

        //Remove all the bullets
        Bullet[] bullets = GameObject.FindObjectsOfType<Bullet>();

        for (int i = bullets.Length - 1; i >= 0; --i)
        {
            GameObject.Destroy(bullets[i].gameObject);
        } 
    }

    public void UpdateGame(int playerID, int deltaScore = 0)
    {
        bool victory = false;

        //Disable input for a bit
        foreach (Tank player in m_Players)
        {
            player.InputEnabled = false;
        }

        //Update the game
        switch (m_VictoryCondition)
        {
            case VictoryCondition.Territory:
            {
                victory = UpdateStage(playerID);
                if (OnUpdateMap != null) OnUpdateMap(m_CurrentStage, false);
            }  
            break;

            case VictoryCondition.Score:
            {
                if (OnUpdateScore != null)
                {
                    int newScore = OnUpdateScore(playerID);
                    if (newScore >= m_MaxScore) { victory = true; }
                } 
            }
            break;

            default: break;
        }

        if (victory)
        {
            //A player won!
            if (OnSetWinner != null) { OnSetWinner(m_Players[playerID].Name); }
        }
        else
        {
            StartCoroutine(ResetLevelRoutine());
        }
    }

    private bool UpdateStage(int winnerID)
    {
        //Currently player id's are linked to directions
        //Player 0 = left, 1 = right, 2 = uo, 3 = bottom

        SubStage nextStage = null;

        if (m_CurrentStageOwner != -1 && m_CurrentStageOwner != winnerID)
        {
            nextStage = m_CurrentStage.GetNeighbouringStage(m_CurrentStageOwner, false);
            if (nextStage == null) { Debug.LogError("Something went wrong in connecting stages!"); }

            //If this is the starting stage, the last winner is currently no longer winning
            if (nextStage == m_StartStage)
            {
                m_CurrentStageOwner = -1;
            }
        }
        else
        {
            //Get the next stage for the winner
            nextStage = m_CurrentStage.GetNeighbouringStage(winnerID, true);

            //Update the stage owner 
            m_CurrentStageOwner = winnerID;

            if (nextStage == null)
            {
                //No stage exists? That means we reached the end and won!
                return true;
            }
        }

        m_LastStage = m_CurrentStage;
        m_CurrentStage = nextStage;

        return false;
    }

    private IEnumerator ResetLevelRoutine()
    {
        yield return new WaitForSeconds(m_Players[0].InvisibleSpeed);

        m_LastStage.Deactivate();
        m_CurrentStage.Activate();

        ResetLevel();
    }
}
