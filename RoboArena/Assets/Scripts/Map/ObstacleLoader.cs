using System.Collections;
using System.Collections.Generic;

using Mirror;

using UnityEngine;

public class ObstacleLoader : NetworkBehaviour
{
    [SerializeField]
    private Transform[] m_ObstaclePositions;
    [SerializeField]
    private GameObject m_ObstaclePrefab;
    [SerializeField]
    private Transform m_ObstacleRoot;

    private void Start()
    {
        if ( !isServer )
            return;
        foreach ( Transform obstaclePosition in m_ObstaclePositions )
        {
           GameObject o= Instantiate( m_ObstaclePrefab, obstaclePosition.position, obstaclePosition.rotation, m_ObstacleRoot);
           NetworkServer.Spawn( o );
        }
    }

}
