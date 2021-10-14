using System;
using System.Collections;

using Mirror;

using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    public static NetworkPlayer LocalPlayer { get; private set; }
    public bool IsBlocking { get; set; }
    public bool EnableActions { get; set; }

    [SerializeField]
    private Renderer m_PlayerRenderer;

    [SerializeField]
    private Material m_BlockingMaterial;
    private Material m_OriginalMaterial;
    private Color m_TeamColor;
    private Color m_TeamBulletColor;


    private bool m_CanBlock = true;
    [SerializeField]
    private NetworkPlayerSettings m_PlayerSettings;

    [SerializeField]
    private PlayerController m_Controller;

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

    public event Action < Vector3 > OnRespawn;
    

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

    public Color TeamBulletColor => m_TeamBulletColor;

    public Color TeamColor => m_TeamColor;

    #region Public

    private void Awake()
    {
        m_OriginalMaterial = Instantiate(m_PlayerRenderer.material);
        m_PlayerRenderer.material = m_OriginalMaterial;
        m_Controller.Player = this;
    }
    public override void OnStartLocalPlayer()
    {
        m_Controller.enabled = true;
        LocalPlayer = this;
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
    public void SetEnableActions(bool enable)
    {
        EnableActions = enable;
        RpcEnableActions( enable );
    }
    [ClientRpc]
    private void RpcEnableActions(bool enable)
    {
        EnableActions = enable;
    }

    [ClientRpc]
    public void RpcAddToPlayerList()
    {
        if ( !netIdentity.isServer && netIdentity.connectionToServer!=null )
        {
            Debug.Log( "Adding Player to List over RPC Call" );
            GameManager.AllPlayers.Add( this, netIdentity.connectionToServer);
        }
    }
    [ClientRpc]
    private void RpcRespawn(Vector3 spawn)
    {
        transform.position = spawn;
        OnRespawn?.Invoke(spawn);
    }
    public void Respawn(Vector3 spawn)
    {
        transform.position = spawn;
        OnRespawn?.Invoke(spawn);
        RpcRespawn( spawn );
    }

    public void SetSettings(
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
        int gameStartCountdown,
        Color teamColor,
        Color teamBulletColor )
    {
        InnerSetSettings(
                         maxHp,
                         moveSpeed,
                         weaponCooldown,
                         bulletDamage,
                         bulletBounces,
                         bulletSpeed,
                         bulletHasMaxTravelTime,
                         bulletMaxTravelTime,
                         blockingCooldown,
                         blockingTime,
                         gameStartCountdown,
                         teamColor,
                         teamBulletColor
                        );
        RpcSetSettings(
                         maxHp,
                         moveSpeed,
                         weaponCooldown,
                         bulletDamage,
                         bulletBounces,
                         bulletSpeed,
                         bulletHasMaxTravelTime,
                         bulletMaxTravelTime,
                         blockingCooldown,
                         blockingTime,
                         gameStartCountdown,
                         teamColor,
                         teamBulletColor
                        );
    }

    private void InnerSetSettings(
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
        int gameStartCountdown,
        Color teamColor,
        Color teamBulletColor )
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
        m_TeamColor = teamColor;
        m_TeamBulletColor = teamBulletColor;
        m_OriginalMaterial.color = TeamColor;
        m_Health = maxHp;
        OnHPChange?.Invoke();
    }

    [ClientRpc]
    private void RpcSetSettings(
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
        int gameStartCountdown,
        Color teamColor,
        Color teamBulletColor)
    {
        InnerSetSettings(
                         maxHp,
                         moveSpeed,
                         weaponCooldown,
                         bulletDamage,
                         bulletBounces,
                         bulletSpeed,
                         bulletHasMaxTravelTime,
                         bulletMaxTravelTime,
                         blockingCooldown,
                         blockingTime,
                         gameStartCountdown,
                         teamColor,
                         teamBulletColor
                        );
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
        if(NetworkClient.active)
        CmdShoot( m_ModelForwardTip.position, m_ModelForwardTip.rotation );
        else
        {
            InnerShoot( m_ModelForwardTip.position, m_ModelForwardTip.rotation );
        }
    }

    #endregion

    #region Private

    private void InnerShoot(Vector3 position, Quaternion rotation)
    {
        GameObject o = Instantiate(m_BulletPrefab, position, rotation);
        NetworkServer.Spawn(o);
        NetworkBullet bullet = o.GetComponent<NetworkBullet>();
        bullet.Owner = gameObject;
        bullet.Damage = m_PlayerSettings.BulletDamage;
        bullet.Bounces = m_PlayerSettings.BulletBounces;
        bullet.Speed = m_PlayerSettings.BulletSpeed;
        bullet.HasMaxTravelTime = m_PlayerSettings.BulletHasMaxTravelTime;
        bullet.MaxTravelTime = m_PlayerSettings.BulletMaxTravelTime;
        bullet.SetColor(TeamBulletColor);
    }
    [Command]
    private void CmdShoot( Vector3 position, Quaternion rotation )
    {
        InnerShoot( position, rotation );
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
