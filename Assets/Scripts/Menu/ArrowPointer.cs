using UnityEngine;
using System.Collections;

//TOO MUCH WORK SCREW IT
public class ArrowPointer : MonoBehaviour
{
    [SerializeField]
    private float m_xOffset = 0.0f;

    [SerializeField]
    private float m_Speed = 0.0f;

    private Vector3 m_StartPosition;
    private int m_Direction = 1;

    private float m_PreviousFrameTime;
	void Start ()
    {
        m_StartPosition = gameObject.transform.localPosition;
	}
	
	// Update is called once per frame
	void Update ()
    {
        float deltaMovement = m_Direction * m_Speed * Time.deltaTime;
        float newX = transform.localPosition.x + deltaMovement;

        if (newX >= (m_StartPosition.x + m_xOffset))
        {
            newX = m_StartPosition.x + m_xOffset;
            m_Direction *= -1;
        }

        if (newX <= m_StartPosition.x)               
        {
            newX = m_StartPosition.x;
            m_Direction *= -1;
        }

        transform.localPosition = new Vector3(newX, transform.localPosition.y, transform.localPosition.z);
	}
}
