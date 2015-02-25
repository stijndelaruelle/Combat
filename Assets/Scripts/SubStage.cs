using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SubStage : MonoBehaviour
{
    [SerializeField]
    private List<Transform> m_SpawnTransforms;

    [SerializeField]
    private List<Color> m_PlayerColors;

    [SerializeField]
    private SubStage m_LeftStage = null;
    public SubStage LeftStage
    {
        get { return m_LeftStage; }
    }

    [SerializeField]
    private SubStage m_RightStage = null;
    public SubStage RightStage
    {
        get { return m_RightStage; }
    }

    [SerializeField]
    private SubStage m_TopStage = null;
    public SubStage TopStage
    {
        get { return m_TopStage; }
    }

    [SerializeField]
    private SubStage m_BottomStage = null;
    public SubStage BottomStage
    {
        get { return m_BottomStage; }
    }

	public Transform GetSpawnTransform(int id)
    {
        if (id < m_SpawnTransforms.Count)
        {
            return m_SpawnTransforms[id];
        }

        return null;
    }

    public Color GetPlayerColor(int id)
    {
        if (id < m_SpawnTransforms.Count)
        {
            return m_PlayerColors[id];
        }

        return Color.white;
    }

    public SubStage GetNeighbouringStage(int dir, bool opposite)
    {
        //Give the opposite direction
        if (opposite)
        {
            switch (dir)
            {
                case 0: return m_RightStage;
                case 1: return m_LeftStage;
                case 2: return m_BottomStage;
                case 3: return m_TopStage;

                default: break;
            }
        }

        switch (dir)
        {
            case 0: return m_LeftStage;
            case 1: return m_RightStage;
            case 2: return m_TopStage;
            case 3: return m_BottomStage;

            default: break;
        }

        return null;
    }
}
