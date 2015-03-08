using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private GameObject m_MainMenu;

    [SerializeField]
    private GameObject m_WinMenu;

    private bool m_IsWinMenuOpen = false;

    //Singleton
    private static MenuManager m_Instance;
    public static MenuManager Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = GameObject.FindObjectOfType<MenuManager>();
            }

            return m_Instance;
        }
    }

    private void Awake()
    {
        if (m_Instance == null)
        {
            //If I am the first instance, make me the Singleton
            m_Instance = this;
        }
        else
        {
            //If a Singleton already exists and you find
            //another reference in scene, destroy it!
            if (this != m_Instance)
                Destroy(this.gameObject);
        }
    }

	// Use this for initialization
    private void Start()
    {
        m_MainMenu.SetActive(true);
        m_WinMenu.SetActive(false);
	}
	
	// Update is called once per frame
	private void Update()
    {
        //If the game already had an initial setup
        if (GameplayManager.Instance.IsInitialized)
        {
            if (Input.GetButtonDown("MenuOpen"))
            {
                OpenMainMenu(!m_MainMenu.activeInHierarchy);
            }

            if (Input.GetButtonDown("MenuCancel"))
            {
                OpenMainMenu(false);
            }
        }
	}

    public void OpenMainMenu(bool value)
    {
        m_MainMenu.SetActive(value);

        if (value == false && m_IsWinMenuOpen)
        {
            m_WinMenu.SetActive(true);
        }

        if (value == true)
        {
            m_IsWinMenuOpen = m_WinMenu.activeInHierarchy;
            m_WinMenu.SetActive(false);
        }
    }

    public void OpenWinMenu(bool value, string name = "")
    {
        if (value) m_WinMenu.GetComponent<WinMenu>().SetName(name);

        if (m_MainMenu.activeInHierarchy)
        {
            m_IsWinMenuOpen = value;
        }
        else
        {
            m_WinMenu.SetActive(value);
        }
    }

    public bool IsMenuOpen()
    {
        if (m_MainMenu.activeInHierarchy) return true;
        if (m_WinMenu.activeInHierarchy)  return true;

        return false;
    }
}
