using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent (typeof(Text))]
public class CountDownTimer : MonoBehaviour
{
    private RectTransform m_RectTransform;
    private Text m_Text;

    private float m_PartTime = 0.0f; //How long does 1 part take? "3, 2, 1, GO!" -> 4 parts
    private float m_Timer = 0.0f;

    private void Awake()
    {
        m_RectTransform = GetComponent<RectTransform>();
        m_Text = GetComponent<Text>();
    }

    private void Start()
    {
        GameplayManager.Instance.OnStartCountDown += OnStartCountDown;
        GameplayManager.Instance.OnStartGame += OnStartGame;

        Hide();
    }

    private void OnDestroy()
    {
        GameplayManager.Instance.OnStartCountDown -= OnStartCountDown;
        GameplayManager.Instance.OnStartGame -= OnStartGame;
    }

    private void Update()
    {
        m_Timer -= Time.deltaTime;

        //Dermine the text
        float part = (m_Timer / m_PartTime);
        int intPart = (int)part;

        string text = intPart.ToString();
        if (intPart == 0) text = "GO!";

        m_Text.text = text;

        //Determine the scale
        float rest = part - intPart;

        float scale = Mathf.Lerp(1.0f, 0.2f, rest);
        m_RectTransform.localScale = new Vector3(scale, scale, 1.0f);

        //Determine the color (alhpa)
        //float alpha = 1.0f - (int)(rest * 3.0) / 3.0f;
        //float alpha = 1.0f - (int)(1.0f / intPart);

        //Color color = m_Text.color;
        //color.a = alpha;

        //m_Text.color = color;
    }

    private void OnStartCountDown(int seconds)
    {
        m_Timer = seconds;
        m_PartTime = m_Timer / 4.0f;

        Show();
    }

    private void OnStartGame(int playerCount)
    {
        Hide();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
}
