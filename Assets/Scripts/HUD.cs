using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class HUD : MonoBehaviour
{
    //Stage based modes
    [SerializeField]
    private GameObject m_SubStageIconPrefab;
    private Dictionary<SubStage, SubStageIcon> m_Map;
    private SubStage m_LastActiveSage;

    //Score based modes
    [SerializeField]
    private List<Text> m_Textfields;
    private List<int> m_Scores;

    private void Awake()
    {
        GameplayManager gameplayManager = GameplayManager.Instance;

        gameplayManager.OnStartCountDown += OnStartCountDown;
        gameplayManager.OnResetGame += OnResetGame;
        gameplayManager.OnUpdateScore += OnUpdateScore;
        gameplayManager.OnGenerateMap += OnGenerateMap;
        gameplayManager.OnUpdateMap += OnUpdateMap;

        //Init scores
        m_Scores = new List<int>();
        for (int i = 0; i < m_Textfields.Count; ++i) { m_Scores.Add(0); }
    }

    //Map
    private void GenerateMap(SubStage startStage, bool show)
    {
        //Clear our current map
        if (m_Map == null) { m_Map = new Dictionary<SubStage, SubStageIcon>(); }

        foreach(KeyValuePair<SubStage, SubStageIcon> keyValue in m_Map)
        {
            Destroy(keyValue.Value.gameObject);
        }
        m_Map.Clear();

        AddSubStageIcon(startStage, m_SubStageIconPrefab.GetComponent<RectTransform>().anchoredPosition);

        m_Map[startStage].SetContested(true);

        ShowMap(show);
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
        newObject.transform.GetComponent<RectTransform>().anchoredPosition = pos;
        newObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        SubStageIcon subStageIcon = newObject.GetComponent<SubStageIcon>();
        subStageIcon.SetContested(false, subStage.HomeColor);

        m_Map.Add(subStage, subStageIcon);

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
                    case 0: offset.x = -155.0f; break;
                    case 1: offset.x = 155.0f;  break;
                    case 2: offset.y = 92.0f;  break;
                    case 3: offset.y = -92.0f;   break;

                    default:
                        break;
                }

                //NOTE: RECURSIVE CALL!
                AddSubStageIcon(neighbour, pos + offset);
            }
        }
    }

    public void OnGenerateMap(SubStage stage, bool show)
    {
        m_LastActiveSage = stage;
        GenerateMap(stage, show);
        return;
    }

    public void OnUpdateMap(SubStage stage, Color color)
    {
        color.a = 1.0f;

        if (m_LastActiveSage != null) m_Map[m_LastActiveSage].SetContested(false, color);
        m_Map[stage].SetContested(true);

        m_LastActiveSage = stage;
    }

    public void ResetMap()
    {
        m_LastActiveSage = null;

        foreach (KeyValuePair<SubStage, SubStageIcon> data in m_Map)
        {
            data.Value.SetContested(false, data.Key.HomeColor);
        }
    }

    private void ShowMap(bool value)
    {
        foreach (KeyValuePair<SubStage, SubStageIcon> kv in m_Map)
        {
            kv.Value.gameObject.SetActive(value);
        }
    }

    //Winner & score
    public void OnStartCountDown(int playerCount)
    {
        //Reset the scores
        for (int i = 0; i < m_Textfields.Count; ++i)
        {
            m_Scores[i] = 0;
            m_Textfields[i].text = "" + 0; //lame

            //Enable of disable some score labels
            bool active = false;
            if (i < playerCount) { active = true; }
            m_Textfields[i].gameObject.SetActive(active);
        }
    }

    public void OnResetGame()
    {
        ResetMap();
    }

    public void OnUpdateScore(int playerID)
    {
        if (playerID >= m_Scores.Count) return;

        m_Scores[playerID] += 1;

        m_Textfields[playerID].text = m_Scores[playerID].ToString();
    }
}
