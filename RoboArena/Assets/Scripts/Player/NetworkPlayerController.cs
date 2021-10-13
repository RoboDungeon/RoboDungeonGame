using Mirror;

using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class NetworkPlayerController : MonoBehaviour
{

    private float m_CurrentWeaponCooldown;
    public NetworkPlayer Player { get; set; }
    [SerializeField]
    private LayerMask m_GroundLayer;
    [SerializeField]
    private Transform m_Model;
    private Rigidbody m_Rigidbody;
    private Vector3 Acceleration => 
        new Vector3( Input.GetAxis( "Horizontal" ), 0, Input.GetAxis( "Vertical" ) );

    private void Start()
    {
        m_Rigidbody = GetComponent < Rigidbody >();
    }
    private void Update()
    {
        Vector3 accel = Acceleration;
        m_Rigidbody.AddForce( accel * Player.PlayerSettings.MoveSpeed * Time.deltaTime, ForceMode.Acceleration );
        CalculateAimDirection();

        m_CurrentWeaponCooldown -= Time.deltaTime;

        if ( m_CurrentWeaponCooldown < 0 && Input.GetKey(KeyCode.Mouse0 ))
        {
            m_CurrentWeaponCooldown = Player.PlayerSettings.WeaponCooldown;
            Player.Shoot();
        }
    }

    

    private void CalculateAimDirection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, m_GroundLayer))
        {
            GameObject obj = hit.transform.gameObject;

            if (obj != null)
            {
                Vector3 p = hit.point;
                p.y = m_Model.position.y;

                m_Model.forward = p - m_Model.position;
            }
        }
    }

}