using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class Scoreboard : MonoBehaviour
{
    [SerializeField]
    private Text m_VictoryLabel = null;

    [SerializeField]
    private List<Text> m_Textfields;
    private List<int> m_Scores;

    private void Start()
    {
        //Init scores
        m_Scores = new List<int>();
        for (int i = 0; i < m_Textfields.Count; ++i) { m_Scores.Add(0); }
    }

    public int UpdateScore(int playerID, int deltaScore)
    {
        if (playerID >= m_Scores.Count) return 0;
        m_Scores[playerID] += deltaScore;

        m_Textfields[playerID].text = m_Scores[playerID].ToString();

        return m_Scores[playerID];
    }

    public void ResetScores()
    {
        for (int i = 0; i < m_Textfields.Count; ++i)
        {
            m_Scores[i] = 0;
            m_Textfields[i].text = "" + 0; //lame
        }

        m_VictoryLabel.gameObject.SetActive(false);
    }

    public void SetWinner(string name)
    {
        m_VictoryLabel.text = name + " wins!";
        m_VictoryLabel.gameObject.SetActive(true);
    }
}
