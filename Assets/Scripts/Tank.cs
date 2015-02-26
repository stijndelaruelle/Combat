﻿using UnityEngine;
using System.Collections;

//Delegates
public delegate void VoidDelegate();
public delegate void BoolDelegate(bool b);
public delegate void IntDelegate(int i);
public delegate int  IntReturnIntDelegate(int i);
public delegate void FloatDelegate(float f);
public delegate void StringDelegate(string s);
public delegate void ColorDelegate(Color color);
public delegate void SubStageBoolDelegate(SubStage subStage, bool b);

public class Tank : MonoBehaviour
{
    //Events
    public event VoidDelegate OnRespawn;
    public event BoolDelegate OnEnableRenderer;
    public event IntDelegate OnHit;
    public event FloatDelegate OnAlphaChange;
    public event ColorDelegate OnColorChange;

    //Datamembers
    [SerializeField]
    private int m_PlayerID = 1;
    public int PlayerID
    {
        get { return m_PlayerID; }
    }

    [SerializeField]
    private float m_MoveSpeed = 10.0f;

    [SerializeField]
    private float m_TurnSpeed = 10.0f;

    [SerializeField]
    private float m_InvinsibleSpeed = 2.0f;
    private float m_InvisibleTimer = 0.0f;

    public bool IsVisible
    {
        get { return gameObject.GetComponent<SpriteRenderer>().enabled; }
    }
    public float InvisibleSpeed
    {
        get { return m_InvinsibleSpeed; }
    }

    [SerializeField]
    private int m_MaxHealth = 1;
    private int m_Health = 1;
    public bool IsDead
    {
        get { return (m_Health <= 0); }
    }

    private bool m_IsInputEnabled = true;
    public bool InputEnabled
    {
        get { return m_IsInputEnabled; }
        set { m_IsInputEnabled = value; }
    }

    //Player name (for fun)
    private string m_PlayerName;
    public string Name
    {
        get { return m_PlayerName; }
    }

    //Functions
    private void Start()
    {
        m_PlayerName = "Player " + (m_PlayerID + 1);
    }

    private void Update()
    {
        //Set names (for fun)
        if (Input.GetButtonDown("SetName1_Joystick" + (m_PlayerID + 1))) { m_PlayerName = "Stijn"; }
        if (Input.GetButtonDown("SetName2_Joystick" + (m_PlayerID + 1))) { m_PlayerName = "Daniel"; }
        if (Input.GetButtonDown("SetName3_Joystick" + (m_PlayerID + 1))) { m_PlayerName = "Freek"; }
        if (Input.GetButtonDown("SetName4_Joystick" + (m_PlayerID + 1))) { m_PlayerName = "Marijke"; }

        if (IsDead || !InputEnabled) return;

        //Rotating
        float xAxis = Input.GetAxis("RotateHorizontal_Joystick" + (m_PlayerID + 1));
        float yAxis = Input.GetAxis("RotateVertical_Joystick" + (m_PlayerID + 1));

        float currentAngle = transform.rotation.eulerAngles.z;
        float desired = Mathf.Atan2(yAxis, xAxis) * Mathf.Rad2Deg;

        if (xAxis == 0.0f && yAxis == 0.0f && desired == 0.0f) return;

        float tweenAngle = Mathf.LerpAngle(currentAngle, desired, Time.deltaTime * m_TurnSpeed);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, tweenAngle));

        //Moving
        float speedMultiplier = Mathf.Clamp01(Mathf.Abs(xAxis) + Mathf.Abs(yAxis));
        Vector2 velocity = transform.right * (speedMultiplier * m_MoveSpeed) * Time.deltaTime;
        transform.Translate(velocity, Space.World);
	}

    #region Damage & Death
    public void Damage(int amount, int otherPlayerID)
    {
        if (IsDead || !InputEnabled) return;

        if (OnHit != null) OnHit(otherPlayerID);

        //You can't die from your own bullets right now
        if (m_PlayerID == otherPlayerID) // && GameplayManager.Instance.CurrentGameMode == global::GameplayManager.GameMode.SingleBulletMode
        {
            return;
        }

        m_Health -= amount;

        if (m_Health <= 0)
        {
            Die(otherPlayerID);
        }

        SetVisible();
    }

    private void Die(int killerID)
    {
        //Did we kill ourselves?
        if (GameplayManager.Instance != null)
        {
            int deltaScore = 1;
            if (m_PlayerID == killerID) deltaScore = -1; //Remove a point if we killed ourselves

            GameplayManager.Instance.UpdateGame(killerID, deltaScore);
        }

        StartCoroutine(DeathRoutine());
    }

    public void Respawn(Transform newTransform)
    {
        m_Health = m_MaxHealth;

        //Reset position
        transform.position = newTransform.position;
        transform.rotation = newTransform.rotation;

        StopAllCoroutines();
        m_InvisibleTimer = 0.0f; //Set to zero so the coroutine gets started
        SetVisible();
        m_InvisibleTimer = 2.0f; //Set to 2 so we go invisible quicker right after spawn

        if (OnRespawn != null) OnRespawn();
    }

    private IEnumerator DeathRoutine()
    {
        float deathTimer = 1.5f;

        while (deathTimer > 0.0f)
        {
            deathTimer -= Time.deltaTime;
            transform.Rotate(new Vector3(0, 0, 1), 20.0f);

            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }
    #endregion

    #region visibility
    public void SetVisible()
    {
        //Set alpha to full
        SetAlpha(1.0f);

        if (GameplayManager.Instance.CurrentGameMode != GameplayManager.GameMode.InvisibleMode) return;

        if (m_InvisibleTimer == 0.0f) { StartCoroutine(VisibilityRoutine()); }
        else                          { m_InvisibleTimer = m_InvinsibleSpeed; }
    }

    public void EnableRenderer(bool value)
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = value;
        if (OnEnableRenderer != null) OnEnableRenderer(value);
    }

    public void SetColor(Color color)
    {
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = true;
        spriteRenderer.color = color;

        if (OnColorChange != null) OnColorChange(color);
    }

    public void SetAlpha(float alpha)
    {
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = true;
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);

        if (OnAlphaChange != null) OnAlphaChange(alpha);
    }

    private IEnumerator VisibilityRoutine()
    {
        EnableRenderer(true);
        m_InvisibleTimer = m_InvinsibleSpeed;

        while (m_InvisibleTimer > 0.0f)
        {
            m_InvisibleTimer -= Time.deltaTime;

            //The last second our alpha goes fading
            if (m_InvisibleTimer <= 1.0f)
            {
                //Grade down in 3 steps
                float newAlpha = Mathf.Ceil(m_InvisibleTimer * 3.0f) / 3.0f;
                SetAlpha(newAlpha);
            }

            yield return new WaitForEndOfFrame();
        }

        m_InvisibleTimer = 0.0f;
        EnableRenderer(false);
    }
    #endregion
}
