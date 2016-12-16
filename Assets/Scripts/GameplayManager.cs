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

    //Events
    public event VoidDelegate OnStartGame;
    public event IntDelegate OnStartCountDown;
    public event VoidDelegate OnResetGame;
    public event IntDelegate OnUpdateScore;

    public event IntDelegate OnPlayerCountChange;

    public event SubStageBoolDelegate OnGenerateMap;
    public event SubStageColorDelegate OnUpdateMap;

    //Datamembers
    private int m_LastPlayerCount = 2;
    private int m_CurrentPlayerCount = 2;
    public int CurrentPlayersCount
    {
        get { return m_CurrentPlayerCount; }
    }

    [SerializeField]
    private Tank[] m_Players;

    [SerializeField]
    private GameMode m_GameMode = GameMode.InvisibleMode;
    public GameMode CurrentGameMode
    {
        get { return m_GameMode; }
    }

    [SerializeField]
    private SubStage m_2PlayerStage;

    [SerializeField]
    private SubStage m_4PlayerStage;

    private SubStage m_StartStage; //The stage where it all begins
    private SubStage m_CurrentStage;
    private SubStage m_LastStage = null;

    private int m_CurrentStageOwner = -1;
    public bool IsInitialized
    {
        get { return (m_StartStage != null); }
    }

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

    private void Awake()
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

        //Hide the mouse
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Start()
    {
        //Disable all player input
        foreach (Tank player in m_Players)
        {
            player.InputEnabled = false;
        }

        //Set the first 2 player map as active
        if (m_2PlayerStage != null) m_2PlayerStage.Activate();
    }

    public void StartCountDown()
    {
        //Select the right stage & reset the game
        if (m_CurrentPlayerCount == 2) { m_StartStage = m_2PlayerStage; }
        else { m_StartStage = m_4PlayerStage; }

        m_CurrentStage = m_StartStage;
        if (OnGenerateMap != null) OnGenerateMap(m_CurrentStage, (m_CurrentPlayerCount == 2));

        ResetGame();

        //Disable all player input
        foreach (Tank player in m_Players)
        {
            player.InputEnabled = false;
        }

        StopAllCoroutines();
        StartCoroutine(CountDownRoutine(3));


        if (OnPlayerCountChange != null &&
            m_LastPlayerCount != m_CurrentPlayerCount)
        {
            OnPlayerCountChange(m_CurrentPlayerCount);
            m_LastPlayerCount = m_CurrentPlayerCount;
        }
    }

    private IEnumerator CountDownRoutine(int secondsLeft)
    {
        OnStartCountDown(secondsLeft);
        yield return new WaitForSeconds(secondsLeft);

        StartGame();
    }

    private void StartGame()
    {
        //We only restart if we have the same amount of players
        if (OnStartGame != null) OnStartGame();

        //Disable all player input
        foreach (Tank player in m_Players)
        {
            player.InputEnabled = true;
        }
    }

    private void ResetGame()
    {
        StopAllCoroutines();

        if (OnResetGame != null) OnResetGame();

        //Reset the level
        m_CurrentStage.Deactivate();
        m_StartStage.Activate();

        m_CurrentStage = m_StartStage;
        if (OnUpdateMap != null) OnUpdateMap(m_CurrentStage, m_CurrentStage.HomeColor);

        m_CurrentStageOwner = -1;

        PrepareNextStage(true);

        //Hide the win menu
        MenuManager.Instance.OpenWinMenu(false);
    }

    private void PrepareNextStage(bool instant = false)
    {
        //Set the camera
        Vector3 newPosition = m_CurrentStage.transform.position;
        newPosition.z = Camera.main.transform.position.z;

        float time = 0.5f;
        if (instant) time = 0.0f;
        Camera.main.GetComponent<CameraPanner>().PanCamera(newPosition, time);

        //Respawn all the active
        for (int i = 0; i < m_CurrentPlayerCount; ++i)
        {
            m_Players[i].gameObject.SetActive(true);
            m_Players[i].Respawn(m_CurrentStage.GetSpawnTransform(i));
            m_Players[i].InputEnabled = true;
        }

        //Decativate the rest
        for (int i = m_CurrentPlayerCount; i < m_Players.Length; ++i)
        {
            m_Players[i].gameObject.SetActive(false);
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
        //Check if a level was actually won
        int playersAlive = 0;
        for (int i = 0; i < m_CurrentPlayerCount; ++i)
        {
            if (!m_Players[i].IsDead) ++playersAlive;
        }

        if (playersAlive > 1)
        {
            return;
        }

        //Disable input for a bit
        foreach (Tank player in m_Players)
        {
            player.InputEnabled = false;
        }

        //Update the game
        if (GotoNextStage(playerID))
        {
            //A player won!
            if (OnUpdateScore != null) { OnUpdateScore(playerID); }

            MenuManager.Instance.OpenWinMenu(true, m_Players[playerID].Name);
        }
        else
        {
            StartCoroutine(PrepareStageRoutine(playerID));
        }
    }

    private bool GotoNextStage(int winnerID)
    {
        //Currently player id's are linked to directions
        //Player 0 = left, 1 = right, 2 = up, 3 = bottom

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

    private IEnumerator PrepareStageRoutine(int winnerID)
    {
        yield return new WaitForSeconds(m_Players[0].InvisibleSpeed);

        m_LastStage.Deactivate();
        m_CurrentStage.Activate();
        if (OnUpdateMap != null) OnUpdateMap(m_CurrentStage, m_Players[winnerID].CurrentColor);

        PrepareNextStage();
    }


    //Called from main menu
    public void IncreasePlayerCount()
    {
        ++m_CurrentPlayerCount;

        //Never go over 4 players
        if (m_CurrentPlayerCount > m_Players.Length)
        {
            m_CurrentPlayerCount = 2;
        }
    }

    public void IncreaseGameMode()
    {
        if (m_GameMode == GameplayManager.GameMode.SingleBulletMode)
        {
            m_GameMode = GameplayManager.GameMode.InvisibleMode;
        }

        else if (m_GameMode == GameplayManager.GameMode.InvisibleMode)
        {
            m_GameMode = GameplayManager.GameMode.SingleBulletMode;
        }
    }
}
