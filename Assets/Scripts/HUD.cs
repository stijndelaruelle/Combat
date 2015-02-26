using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class HUD : MonoBehaviour
{
    [SerializeField]
    private Text m_VictoryLabel = null;

    //Stage based modes
    [SerializeField]
    private GameObject m_SubStageIconPrefab;

    private Dictionary<SubStage, Image> m_Map;

    //Score based modes
    [SerializeField]
    private List<Text> m_Textfields;
    private List<int> m_Scores;

    private void Awake()
    {
        GameplayManager gameplayManager = GameplayManager.Instance;

        gameplayManager.OnResetGame += OnResetGame;
        gameplayManager.OnUpdateScore += OnUpdateScore;
        gameplayManager.OnUpdateMap += OnUpdateMap;
        gameplayManager.OnSetWinner += OnSetWinner;

        //Init scores
        m_Scores = new List<int>();
        for (int i = 0; i < m_Textfields.Count; ++i) { m_Scores.Add(0); }
    }

    //Map
    private void GenerateMap(SubStage startStage)
    {
        //Clear our current map
        if (m_Map == null) { m_Map = new Dictionary<SubStage, Image>(); }
        m_Map.Clear();

        AddSubStageIcon(startStage, Vector2.zero);
    }

    private void AddSubStageIcon(SubStage subStage, Vector2 pos)
    {
        if (m_SubStageIconPrefab == null)
        {
            Debug.LogWarning("No SubStage icon prefab provided to the scoreboard!");
        }

        if (m_Map.ContainsKey(subStage)) return;

        //Add the icon
        GameObject newObject = GameObject.Instantiate(m_SubStageIconPrefab) as GameObject;
        newObject.transform.SetParent(gameObject.transform);
        newObject.transform.localPosition = pos;
        newObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        m_Map.Add(subStage, newObject.GetComponent<Image>());

        //Go over all the neightbours
        for (int dir = 0; dir < 4; ++dir)
        {
            SubStage neighbour = subStage.GetNeighbouringStage(dir, false);

            if (neighbour != null)
            {
                //Calculate the offset
                Vector2 offset = Vector2.zero;

                switch (dir)
                {
                    case 0: offset.x = -150.0f; break;
                    case 1: offset.x = 150.0f;  break;
                    case 2: offset.y = 87.0f;  break;
                    case 3: offset.y = -87.0f;   break;

                    default:
                        break;
                }

                //NOTE: RECURSIVE CALL!
                AddSubStageIcon(neighbour, pos + offset);
            }
        }
    }

    public void OnUpdateMap(SubStage stage, bool generate)
    {
        if (generate)
        {
            GenerateMap(stage);
            return;
        }


    }

    //Score
    public int OnUpdateScore(int playerID)
    {
        if (playerID >= m_Scores.Count) return 0;
        m_Scores[playerID] += 1;

        m_Textfields[playerID].text = m_Scores[playerID].ToString();

        return m_Scores[playerID];
    }

    public void OnResetGame()
    {
        for (int i = 0; i < m_Textfields.Count; ++i)
        {
            m_Scores[i] = 0;
            m_Textfields[i].text = "" + 0; //lame
        }

        m_VictoryLabel.gameObject.SetActive(false);
    }

    public void OnSetWinner(string name)
    {
        m_VictoryLabel.text = name + " wins!";
        m_VictoryLabel.gameObject.SetActive(true);

        Debug.Log(name);
    }
}
