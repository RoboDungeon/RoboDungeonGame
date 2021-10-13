using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingObstacle : NetworkBehaviour
{
    [SerializeField]
    private float m_RotateSpeed = 20;

    private void Start()
    {
        if ( !isServer)
            enabled = false;
    }

    private void Update()
    {
        if ( !isServer )
            return;

        transform.Rotate( Vector3.up, m_RotateSpeed * Time.deltaTime );
    }
}
