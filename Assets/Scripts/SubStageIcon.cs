using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SubStageIcon : MonoBehaviour
{
    [SerializeField]
    Image m_ContestedSprite = null;

    [SerializeField]
    Image m_ClaimedSprite = null;

    public void SetContested(bool value)
    {
        m_ContestedSprite.enabled = value;
        m_ClaimedSprite.enabled = !value;
    }

    public void SetContested(bool value, Color color)
    {
        SetContested(value);

        m_ContestedSprite.color = color;
        m_ClaimedSprite.color = color;
    }
}
