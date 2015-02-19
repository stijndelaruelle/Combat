using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour
{
    [SerializeField]
    private float m_Angle;

	void Update ()
    {
        transform.Rotate(new Vector3(0.0f, 0.0f, 1.0f), m_Angle);
	}
}
