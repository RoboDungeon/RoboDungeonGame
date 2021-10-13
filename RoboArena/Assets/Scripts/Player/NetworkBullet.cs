using System;
using System.Collections;

using Mirror;

using UnityEngine;

[RequireComponent( typeof( Rigidbody ) )]
public class NetworkBullet : NetworkBehaviour
{

    [SerializeField]
    private int m_Bounces;

    [SerializeField]
    private float m_Speed;

    [SerializeField]
    private int m_Damage;

    [SerializeField]
    private bool m_HasMaxTravelTime;

    [SerializeField]
    private float m_MaxTravelTime;

    private int m_CurrentBounces;
    private int m_WallCollisionLayer;
    private int m_PlayerCollisionLayer;

    private Rigidbody m_Rigidbody;

    public GameObject Owner { get; set; }

    public int Damage
    {
        get => m_Damage;
        set => m_Damage = value;
    }

    public float Speed
    {
        get => m_Speed;
        set => m_Speed = value;
    }

    public int Bounces
    {
        get => m_Bounces;
        set => m_Bounces = value;
    }

    public bool HasMaxTravelTime
    {
        get => m_HasMaxTravelTime;
        set => m_HasMaxTravelTime = value;
    }

    public float MaxTravelTime
    {
        get => m_MaxTravelTime;
        set => m_MaxTravelTime = value;
    }

    #region Unity Event Functions

    private void OnCollisionEnter( Collision col )
    {

        if (!isServer)
        {
            return;
        }
        if ( col.collider.transform.gameObject.layer == m_WallCollisionLayer )
        {
            if ( m_CurrentBounces != 0 )
            {
                m_CurrentBounces--;
                ReflectProjectile( col.contacts[0].normal );
            }
            else if ( OnDestroy == null )
            {
                NetworkServer.Destroy( gameObject );
            }
            else
            {
                OnDestroy.Invoke();
            }
        }

        if ( col.collider.transform.gameObject.layer == m_PlayerCollisionLayer &&
             col.collider.gameObject != Owner )
        {
            Debug.Log( "Colliding with Player: " + col.gameObject );
            NetworkPlayer u = col.collider.gameObject.GetComponent < NetworkPlayer >();

            if ( u != null )
            {
                Debug.Log( "Hit Player: " + u );

                if ( u.IsBlocking )
                {
                    ReflectProjectile(col.contacts[0].normal);
                }
                else
                {
                    u.TakeDamage(Damage);
                    if (OnDestroy == null)
                    {
                        NetworkServer.Destroy(gameObject);
                    }
                    else
                    {
                        OnDestroy.Invoke();
                    }
                }
            }
        }
    }

    public event Action OnDestroy;

    private void Start()
    {
        ResetData();
        m_WallCollisionLayer = LayerMask.NameToLayer( "Wall" );
        m_PlayerCollisionLayer = LayerMask.NameToLayer( "Player" );
        m_Rigidbody = GetComponent < Rigidbody >();
    }

    private void Update()
    {
        if (!isServer)
        {
            return;
        }
        transform.position += transform.forward * Speed * Time.deltaTime;
    }

    #endregion

    #region Public

    public void ResetData()
    {
        m_CurrentBounces = Bounces;

        if ( HasMaxTravelTime )
        {
            StartCoroutine( DestroyAfter( MaxTravelTime ) );
        }
    }

    #endregion

    #region Private

    private IEnumerator DestroyAfter( float time )
    {
        yield return new WaitForSeconds( time );

        if ( OnDestroy == null )
        {
            Destroy( gameObject );
        }
        else
        {
            OnDestroy.Invoke();
        }
    }

    private void ReflectProjectile( Vector3 reflectVector )
    {
        m_Rigidbody.velocity = Vector3.zero;
        transform.forward = Vector3.Reflect( transform.forward, reflectVector );
    }

    #endregion

}
