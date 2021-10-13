using System;
using System.Collections;

using Mirror;

using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    public bool IsBlocking { get; set; }
    public bool EnableActions { get; set; }

    [SerializeField]
    private Renderer m_PlayerRenderer;

    [SerializeField]
    private Material m_BlockingMaterial;
    private Material m_OriginalMaterial;


    private bool m_CanBlock = true;
    [SerializeField]
    private NetworkPlayerSettings m_PlayerSettings;

    [SerializeField]
    private NetworkPlayerController m_Controller;

    [SerializeField]
    private Transform m_ModelForwardTip;

    [SerializeField]
    private GameObject m_BulletPrefab;

    [SerializeField]
    private int m_TeamID;

    [SerializeField]
    private int m_Health;

    public event Action OnDeath;

    public event Action OnHPChange;
    

    public int TeamID
    {
        get => m_TeamID;
        set => m_TeamID = value;
    }

    public NetworkPlayerSettings PlayerSettings
    {
        get => m_PlayerSettings;
        set => m_PlayerSettings = value;
    }

    public int Health => m_Health;

    #region Public

    private void Start()
    {
        m_OriginalMaterial = m_PlayerRenderer.material;
    }
    public override void OnStartLocalPlayer()
    {
        m_Controller.enabled = true;
        m_Controller.Player = this;
        base.OnStartLocalPlayer();
    }

    public void ResetHealth()
    {
        m_Health = m_PlayerSettings.MaxHealth;
        OnHPChange?.Invoke();
        RpcResetHealth();
    }
    [ClientRpc]
    private void RpcResetHealth()
    {
        m_Health = m_PlayerSettings.MaxHealth;
        OnHPChange?.Invoke();
    }
    [ClientRpc]
    public void RpcEnableActions(bool enable)
    {
        EnableActions = enable;
    }

    [ClientRpc]
    public void RpcAddToPlayerList()
    {
        if ( !netIdentity.isServer && netIdentity.connectionToServer!=null )
        {
            Debug.Log( "Adding Player to List over RPC Call" );
            GameManager.AllPlayers.Add(netIdentity.connectionToServer, this);
        }
    }
    [ClientRpc]
    public void RpcRespawn( Vector3 spawn )
    {
        transform.position = spawn;
    }
    

    [ClientRpc]
    public void RpcSetSettings(
        int maxHp,
        float moveSpeed,
        float weaponCooldown,
        int bulletDamage,
        int bulletBounces,
        float bulletSpeed,
        bool bulletHasMaxTravelTime,
        float bulletMaxTravelTime,
        float blockingCooldown,
        float blockingTime,
        int gameStartCountdown)
    {
        m_PlayerSettings.MaxHealth = maxHp;
        m_PlayerSettings.WeaponCooldown = weaponCooldown;
        m_PlayerSettings.MoveSpeed = moveSpeed;
        m_PlayerSettings.BulletDamage = bulletDamage;
        m_PlayerSettings.BulletBounces = bulletBounces;
        m_PlayerSettings.BulletSpeed = bulletSpeed;
        m_PlayerSettings.BulletHasMaxTravelTime = bulletHasMaxTravelTime;
        m_PlayerSettings.BulletMaxTravelTime = bulletMaxTravelTime;
        m_PlayerSettings.BlockingCooldown = blockingCooldown;
        m_PlayerSettings.BlockingTime = blockingTime;
        m_PlayerSettings.GameStartCountdown = gameStartCountdown;
        m_Health = maxHp;
        OnHPChange?.Invoke();
    }

    [ClientRpc]
    private void RpcSetTeamID(int id)
    {
        m_TeamID = id;
    }

    public void SetTeamID(int id)
    {
        m_TeamID = id;
        RpcSetTeamID( id );
    }

    public void TakeDamage( int damage )
    {
        m_Health -= damage;
        OnHPChange?.Invoke();

        if (m_Health <= 0)
        {
            OnDeath?.Invoke();
        }

        RpcTakeDamage( damage );
    }
    [ClientRpc]
    private void RpcTakeDamage( int damage )
    {
        m_Health -= damage;
        OnHPChange?.Invoke();

        if ( m_Health <= 0 )
        {
            OnDeath?.Invoke();
        }
    }

    public void Shoot()
    {
        CmdShoot( m_ModelForwardTip.position, m_ModelForwardTip.rotation );
    }

    #endregion

    #region Private

    [Command]
    private void CmdShoot( Vector3 position, Quaternion rotation )
    {
        GameObject o = Instantiate( m_BulletPrefab, position, rotation );
        NetworkServer.Spawn( o );
        NetworkBullet bullet = o.GetComponent < NetworkBullet >();
        bullet.Owner = gameObject;
        bullet.Damage = m_PlayerSettings.BulletDamage;
        bullet.Bounces = m_PlayerSettings.BulletBounces;
        bullet.Speed = m_PlayerSettings.BulletSpeed;
        bullet.HasMaxTravelTime = m_PlayerSettings.BulletHasMaxTravelTime;
        bullet.MaxTravelTime = m_PlayerSettings.BulletMaxTravelTime;
    }

    public void Block()
    {
        CmdBlock();
    }
    [Command]
    private void CmdBlock()
    {
        if ( m_CanBlock )
        {
            StartCoroutine( BlockingRoutine() );
            RpcBlock();
        }

    }

    [ClientRpc]
    private void RpcBlock()
    {
        IsBlocking = true;
        m_PlayerRenderer.material = m_BlockingMaterial;
    }
    [ClientRpc]
    private void RpcUnBlock()
    {
        IsBlocking = false;
        m_PlayerRenderer.material = m_OriginalMaterial;
    }

    private IEnumerator BlockingRoutine()
    {
        m_CanBlock = false;
           IsBlocking = true;

        yield return new WaitForSeconds( m_PlayerSettings.BlockingTime );

        IsBlocking = false;
        RpcUnBlock();

        yield return new WaitForSeconds( m_PlayerSettings.BlockingCooldown );

        m_CanBlock = true;
    }

    #endregion

}
