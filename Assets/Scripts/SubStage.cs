using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SubStage : MonoBehaviour
{
    //-------------
    // Datamembers
    //-------------

    [SerializeField]
    private Color m_HomeColor;
    public Color HomeColor
    {
        get { return m_HomeColor; }
    }

    [SerializeField]
    private List<GameObject> m_DynamicObjects;

    [SerializeField]
    private List<Transform> m_SpawnTransforms;

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

    //-------------
    // Functions
    //-------------
    private void Start()
    {
        Deactivate();
    }

    public void Activate()
    {
        //Enable & reset all our dynamic objects
        foreach (GameObject obj in m_DynamicObjects)
        {
            obj.transform.localRotation = Quaternion.identity;
            obj.SetActive(true);
        }
    }

    public void Deactivate()
    {
        //Disable all our dynamic objects
        foreach (GameObject obj in m_DynamicObjects)
        {
            obj.SetActive(false);
        }
    }

	public Transform GetSpawnTransform(int id)
    {
        if (id < m_SpawnTransforms.Count)
        {
            return m_SpawnTransforms[id];
        }

        return null;
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
