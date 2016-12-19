using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class ButtonSelect : MonoBehaviour, ISelectHandler
{
    [SerializeField]
    private RectTransform m_ArrowTransform = null;

    public void OnSelect(BaseEventData eventData)
    {
        if (m_ArrowTransform == null) return;

        Vector3 currentPos = m_ArrowTransform.localPosition;
        currentPos = new Vector3(transform.localPosition.x - 75.0f, transform.localPosition.y, transform.localPosition.z);
        m_ArrowTransform.localPosition = currentPos;
    }
}
