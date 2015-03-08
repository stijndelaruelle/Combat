using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WinMenu : MonoBehaviour
{
    [SerializeField]
    private Text m_NameField;

    [SerializeField]
    private GameObject m_MainButton;

    private void OnEnable()
    {
        if (m_MainButton != null)
        {
            EventSystem.current.SetSelectedGameObject(m_MainButton);
        }
    }

    public void SetName(string name)
    {
        m_NameField.text = name + " wins!";
    }

    public void RestartGame()
    {
        GameplayManager.Instance.ResetGame();
    }
}
