using UnityEngine;
using System.Collections;

public class Barrel : MonoBehaviour
{
    //Datamembers
    [SerializeField]
    private float m_TurnSpeed = 10.0f;

    [SerializeField]
    private float m_ReloadSpeed = 1.0f;

    [SerializeField]
    private GameObject m_BulletPrefab = null;

    private int m_PlayerID = 1; //Get from parent
    private Quaternion m_LastRotation;

    private bool m_IsReloading = false;
    private Tank m_Parent = null;

    //Functions
    private void Start()
    {
        m_Parent = transform.parent.GetComponent<Tank>();

        //Subscribe to events
        m_Parent.OnHit += OnParentHit;
        m_Parent.OnRespawn += OnParentRespawn;
        m_Parent.OnEnableRenderer += OnParentEnableRenderer;
        m_Parent.OnColorChange += OnParentColorChange;
        m_Parent.OnAlphaChange += OnParentAlphaChange;

        m_PlayerID = m_Parent.PlayerID;

        m_LastRotation = m_Parent.gameObject.transform.localRotation;
    }

    private void Destroy()
    {
        //Unsubscribe from events
        m_Parent.OnHit -= OnParentHit;
        m_Parent.OnRespawn -= OnParentRespawn;
        m_Parent.OnEnableRenderer -= OnParentEnableRenderer;
        m_Parent.OnColorChange -= OnParentColorChange;
        m_Parent.OnAlphaChange -= OnParentAlphaChange;
    }

    private void Update()
    {
        if (m_Parent.IsDead || !m_Parent.InputEnabled) return;

        HandleRotating();
        HandleShooting();
    }

    private void HandleRotating()
    {
        //Ignore our parent's rotation
        transform.rotation = m_LastRotation;

        //Rotating
        float xAxis = Input.GetAxis("AimHorizontal_Joystick" + (m_PlayerID + 1));
        float yAxis = Input.GetAxis("AimVertical_Joystick" + (m_PlayerID + 1));

        float currentAngle = transform.rotation.eulerAngles.z;
        float desired = Mathf.Atan2(yAxis, xAxis) * Mathf.Rad2Deg;

        if (xAxis == 0.0f && yAxis == 0.0f && desired == 0.0f) return;

        float tweenAngle = Mathf.LerpAngle(currentAngle, desired, Time.deltaTime * m_TurnSpeed);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, tweenAngle));

        m_LastRotation = transform.rotation;
    }

    private void HandleShooting()
    {
        if (m_BulletPrefab == null)
        {
            Debug.LogWarning("Assign a bullet prefab to the barrel or it will shoot nothing at all.");
            return;
        }

        float shootAxis = Input.GetAxis("Shoot_Joystick" + (m_PlayerID + 1));

        if (shootAxis != 0.0f && m_IsReloading == false)
        {
            //Calculate spawn position
            Vector3 offset = transform.right * 90.0f * Time.deltaTime;
            Vector2 spawnPos = transform.position + offset;

            //Shoot
            GameObject obj = GameObject.Instantiate(m_BulletPrefab, spawnPos, transform.rotation) as GameObject;
            Bullet bullet = obj.GetComponent<Bullet>();

            Color color = gameObject.GetComponent<SpriteRenderer>().color;
            color.a = 1.0f;
            bullet.SetColor(color);

            bullet.OwnerID = m_Parent.PlayerID;

            //Make parent visible
            m_Parent.SetVisible();

            //Start reloading
            m_IsReloading = true;

            if (GameplayManager.Instance.CurrentGameMode != GameplayManager.GameMode.SingleBulletMode)
            {
                StartCoroutine(ReloadRoutine());
            }
        }
    }

    private IEnumerator ReloadRoutine()
    {
        yield return new WaitForSeconds(m_ReloadSpeed);
        m_IsReloading = false;

        yield return null;
    }

    private void OnParentHit(int otherID)
    {
        if (GameplayManager.Instance.CurrentGameMode == GameplayManager.GameMode.SingleBulletMode &&
            m_PlayerID == otherID)
        {
            m_IsReloading = false;
        }
    }

    private void OnParentRespawn()
    {
        m_IsReloading = false;

        //Reset rotation
        m_LastRotation = m_Parent.gameObject.transform.localRotation;
    }

    private void OnParentColorChange(Color color)
    {
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = true;
        spriteRenderer.color = color;
    }

    private void OnParentAlphaChange(float alpha)
    {
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = true;
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);
    }

    private void OnParentEnableRenderer(bool value)
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = value;
    }
}
