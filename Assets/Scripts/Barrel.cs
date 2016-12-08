using UnityEngine;
using System.Collections;
using XBOXParty;

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
        m_Parent.OnRespawn += OnParentRespawn;
        m_Parent.OnEnableRenderer += OnParentEnableRenderer;
        m_Parent.OnColorChange += OnParentColorChange;
        m_Parent.OnAlphaChange += OnParentAlphaChange;

        m_PlayerID = m_Parent.PlayerID;

        m_LastRotation = m_Parent.gameObject.transform.localRotation;

        //Bind axis
        InputManager.Instance.BindAxis("Deadzone_AimHorizontal_" + m_PlayerID, m_PlayerID, ControllerAxisCode.RightStickX);
        InputManager.Instance.BindAxis("Deadzone_AimVertical_" + m_PlayerID, m_PlayerID, ControllerAxisCode.RightStickY);

        InputManager.Instance.BindAxis("Deadzone_ShootLeft_" + m_PlayerID, m_PlayerID, ControllerAxisCode.LeftTrigger);
        InputManager.Instance.BindAxis("Deadzone_ShootRight_" + m_PlayerID, m_PlayerID, ControllerAxisCode.RightTrigger);
    }

    private void Destroy()
    {
        //Unsubscribe from events
        m_Parent.OnRespawn -= OnParentRespawn;
        m_Parent.OnEnableRenderer -= OnParentEnableRenderer;
        m_Parent.OnColorChange -= OnParentColorChange;
        m_Parent.OnAlphaChange -= OnParentAlphaChange;

        //Unbind axis
        InputManager.Instance.UnbindAxis("Deadzone_AimHorizontal_" + m_PlayerID);
        InputManager.Instance.UnbindAxis("Deadzone_AimVertical_" + m_PlayerID);

        InputManager.Instance.UnbindAxis("Deadzone_ShootLeft_" + m_PlayerID);
        InputManager.Instance.UnbindAxis("Deadzone_ShootRight_" + m_PlayerID);
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
        float xAxis = InputManager.Instance.GetAxis("Deadzone_AimHorizontal_" + m_PlayerID);
        float yAxis = InputManager.Instance.GetAxis("Deadzone_AimVertical_" + m_PlayerID);

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

        float shootAxisLeft = InputManager.Instance.GetAxis("Deadzone_ShootLeft_" + m_PlayerID);
        float shootAxisRight = InputManager.Instance.GetAxis("Deadzone_ShootRight_" + m_PlayerID);

        float shootAxis = Mathf.Max(shootAxisLeft, shootAxisRight);

        //#if UNITY_EDITOR
        //        if (shootAxis == 0.0f && m_PlayerID == 1)
        //        {
        //            bool db = Input.GetButtonDown("ShootDebug");
        //            if (db) shootAxis = 11.0f;
        //        }
        //#endif

        if (shootAxis != 0.0f && m_IsReloading == false)
        {
            //Calculate spawn position
            Vector3 offset = transform.right * 90.0f * Time.deltaTime;
            Vector2 spawnPos = transform.position + offset;

            //Do a raycast check
            int layerMask = 1 << 9; //walls
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), new Vector2(transform.right.x, transform.right.y), 1.0f, layerMask);

            if (hit.collider != null)
            {
                //Don't shoot
                return;
            }

            //Shoot
            GameObject obj = GameObject.Instantiate(m_BulletPrefab, spawnPos, transform.rotation) as GameObject;
            Bullet bullet = obj.GetComponent<Bullet>();

            //Colour ourselves just like our parent
            Color color = gameObject.transform.parent.GetComponent<SpriteRenderer>().color;
            color.a = 1.0f;
            bullet.SetColor(color);

            bullet.OwnerID = m_Parent.PlayerID;
            bullet.OwnerBarrel = this;

            //Make parent visible
            m_Parent.SetVisible();

            //Start reloading
            m_IsReloading = true;

            if (GameplayManager.Instance.CurrentGameMode != GameplayManager.GameMode.SingleBulletMode)
            {
                StartCoroutine(ReloadRoutine(m_ReloadSpeed));
            }
        }
    }

    private IEnumerator ReloadRoutine(float speed)
    {
        yield return new WaitForSeconds(speed);
        m_IsReloading = false;

        yield return null;
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

    public void OnBulletHit(int otherID)
    {
        //When a bullet we fired hit someone this is called
        if (GameplayManager.Instance.CurrentGameMode == GameplayManager.GameMode.SingleBulletMode)
        {
            if (m_PlayerID != otherID)
            {
                StartCoroutine(ReloadRoutine(m_ReloadSpeed / 4.0f));
            }
            else
            {
                m_IsReloading = false;
            }
        }
    }
}
