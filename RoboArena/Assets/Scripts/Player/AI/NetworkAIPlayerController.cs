using UnityEngine;

public class NetworkAIPlayerController : PlayerController
{

    private float m_CurrentWeaponCooldown;
    [SerializeField]
    private NetworkAIMovement m_Movement;
    private void Start()
    {
        m_Movement.SetTarget(this);
    }
    private void Update()
    {
        if (!Player.EnableActions)
            return;

        m_Movement.Move();


        m_CurrentWeaponCooldown -= Time.deltaTime;
        if (m_CurrentWeaponCooldown < 0 )
        {
            m_CurrentWeaponCooldown = Player.PlayerSettings.WeaponCooldown;
            Player.Shoot();
        }
    }




}