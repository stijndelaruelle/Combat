using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    //Datamembers
    [SerializeField]
    private float m_MaxMoveSpeed = 10.0f;
    private float m_MoveSpeed;

    [SerializeField]
    private int m_Damage = 1;

    [SerializeField]
    private int m_MaxBounces = 0;
    private int m_Bounces = 0;

    private int m_OwnerID = 0;
    public int OwnerID
    {
        get { return m_OwnerID; }
        set { m_OwnerID = value; }
    }

    //Functions
    private void Start()
    {
        m_MoveSpeed = m_MaxMoveSpeed;
        m_Bounces = m_MaxBounces;
    }

    private void Update()
    {
        //Moving
        Vector2 velocity = transform.right * m_MoveSpeed * Time.deltaTime;
        transform.Translate(velocity, Space.World);

        Debug.DrawRay(new Vector2(transform.position.x - transform.right.x, transform.position.y - transform.right.y), new Vector2(transform.right.x * 2.0f, transform.right.y * 2.0f));
    }

    public void SetColor(Color color)
    {
        for (int i = 0; i < transform.childCount; ++i)
        {
            Material material = transform.GetChild(i).gameObject.renderer.material;

            if (material != null) { material.color = color; }
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        //If it's a player, damage him
        if (collider.tag == "Player")
        {
            collider.transform.parent.gameObject.GetComponent<Tank>().Damage(m_Damage, m_OwnerID);

            //Destroy ourselves
            GameObject.Destroy(gameObject);
            return;
        }

        //It's a wall, let's bounce!
        if (m_Bounces > 0 || GameplayManager.Instance.CurrentGameMode == GameplayManager.GameMode.SingleBulletMode)
        {
            int layerMask = 1 << 8; //8 = ShadowCaster layer

            RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x - transform.right.x, transform.position.y - transform.right.y), new Vector2(transform.right.x, transform.right.y), 4.0f, layerMask);
            
            if (hit.collider != null)
            {
                transform.right = Vector3.Reflect(transform.right, hit.normal);
            }

            //Reduce movespeed by a xth
            m_MoveSpeed -= (m_MoveSpeed / m_MaxBounces);
            --m_Bounces;

            return;
        }

        //No more bounces, let's just 
        GameObject.Destroy(gameObject);
    }
}
