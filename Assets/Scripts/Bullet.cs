using UnityEngine;
using System.Collections;

[RequireComponent (typeof(CircleCollider2D))]
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

    private Barrel m_OwnerBarrel = null; //The barrel that shot me, for on hit callback
    public Barrel OwnerBarrel
    {
        get { return m_OwnerBarrel; }
        set { m_OwnerBarrel = value; }
    }

    private float m_Radius = 0.0f;
    private Collider2D m_LastCollider = null;

    //Functions
    private void Start()
    {
        m_MoveSpeed = m_MaxMoveSpeed;
        m_Bounces = m_MaxBounces;

        m_Radius = gameObject.GetComponent<CircleCollider2D>().radius;
    }

    private void Update()
    {
        //Moving
        Vector2 velocity = transform.right * m_MoveSpeed * Time.deltaTime;
        transform.Translate(velocity, Space.World);
    }

    private void FixedUpdate()
    {
        Debug.DrawLine(transform.position, transform.position + (transform.forward * 3.0f), Color.red);
        Debug.DrawLine(transform.position, transform.position + (transform.right * 3.0f), Color.green);

        PredictCollision();
    }

    private void PredictCollision()
    {
        LayerMask layerMask1 = 1 << LayerMask.NameToLayer("ShadowLayer");
        LayerMask layerMask2 = 1 << LayerMask.NameToLayer("Bullet");
        LayerMask combiLayerMask = ~(layerMask1 | layerMask2);

        //Minimum distance
        float distance = m_MoveSpeed * Time.deltaTime;
        if (distance < (m_Radius * 2)) { distance = m_Radius * 2; }

        RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x - (transform.right.x * m_Radius), transform.position.y - (transform.right.y * m_Radius)), //Start at the back of the circle
                                             new Vector2(transform.right.x, transform.right.y),
                                             distance, combiLayerMask);

        if (hit)
        {
            if (hit.collider == m_LastCollider) { return; }
            m_LastCollider = hit.collider;

            //Next frame we are going to hit something
            if (hit.collider.gameObject.tag == "Player")
            {
                Tank tank = hit.collider.gameObject.transform.GetComponent<Tank>();

                m_OwnerBarrel.OnBulletHit(tank.PlayerID);
                tank.Damage(m_Damage, m_OwnerID);

                //Destroy ourselves
                m_OwnerBarrel.OnBulletDestroyed();
                GameObject.Destroy(gameObject);
                return;
            }

            //It's a wall, let's bounce!
            if (m_Bounces > 0 || GameplayManager.Instance.CurrentGameMode == GameplayManager.GameMode.SingleBulletMode)
            {
                Debug.DrawLine(hit.point, hit.point + (hit.normal * 3.0f), Color.cyan, 5.0f);

                if (hit.normal == -new Vector2(transform.right.x, transform.right.y) &&
                    hit.normal != new Vector2(0.0f, 1.0f) &&
                    hit.normal != new Vector2(0.0f, -1.0f) &&
                    hit.normal != new Vector2(1.0f, 0.0f) &&
                    hit.normal != new Vector2(-1.0f, 0.0f))
                {
                    Debug.Log("WRONG REFLECT NORMAL, recalculating...");

                    //Shoot it backwards, and see what we encounter
                    hit = Physics2D.Raycast(hit.point,
                                            new Vector2(-transform.right.x, -transform.right.y),
                                            distance, combiLayerMask);
                }


                if (hit.normal == -new Vector2(transform.right.x, transform.right.y) &&
                    hit.normal != new Vector2(0.0f, 1.0f) &&
                    hit.normal != new Vector2(0.0f, -1.0f) &&
                    hit.normal != new Vector2(1.0f, 0.0f) &&
                    hit.normal != new Vector2(-1.0f, 0.0f))
                {
                    Debug.Log("WRONG REFLECT NORMAL AGAIN");
                }

                Reflect(hit.normal);

                return;
            }

            //No more bounces, let's just destroy ourselves
            m_OwnerBarrel.OnBulletDestroyed();
            GameObject.Destroy(gameObject);
        }
    }

    public void SetColor(Color color)
    {
        for (int i = 0; i < transform.childCount; ++i)
        {
            Renderer renderer = transform.GetChild(i).gameObject.GetComponent<Renderer>();

            if (renderer != null)
            {
                Material material = renderer.material;

                if (material != null)
                {
                    material.color = color;
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Only used for player collision, bouncing is predicted in PredictCollision
        if (collision.gameObject.tag == "Player")
        {
            Tank tank = collision.gameObject.transform.GetComponent<Tank>();

            m_OwnerBarrel.OnBulletHit(tank.PlayerID);
            tank.Damage(m_Damage, m_OwnerID);

            //Destroy ourselves
            GameObject.Destroy(gameObject);
            return;
        }
    }

    private void Reflect(Vector3 normal)
    {
        transform.right = Vector3.Reflect(transform.right, normal);

        //Reduce movespeed by a xth
        m_MoveSpeed -= (m_MoveSpeed / m_MaxBounces);
        --m_Bounces;
    }
}
