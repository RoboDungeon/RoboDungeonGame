using UnityEngine;

[CreateAssetMenu(menuName = "RoboArena/AI/SimpleAI")]
public class SimpleNetworkAIMovement : NetworkAIMovement
{

    [SerializeField]
    private Vector3 m_MoveDirection = Vector3.forward;

    [SerializeField]
    [Range(1, 100)]
    private float m_Speed = 4;
    [SerializeField]
    [Range(0, 100)]
    private float m_MoveDistance = 5;


    private Vector3 m_StartPosition;

    protected override void OnSetTarget()
    {
        m_StartPosition = Target.transform.position;
        Target.Player.OnRespawn += Player_OnRespawn;
    }
    private void Player_OnRespawn(Vector3 obj)
    {
        m_StartPosition = obj;
    }

    public override void Move()
    {
        Target.transform.position =
            m_StartPosition + m_MoveDirection * Mathf.Sin(m_Speed * Time.realtimeSinceStartup) * m_MoveDistance;
    }

}