﻿using System;
using System.Collections.Generic;

using Mirror;

using UnityEngine;
using UnityEngine.UI;

public class TeamHealthUI : NetworkBehaviour
{

    [SerializeField]
    private Slider[] m_Sliders;

    private void Start()
    { 
        //Hack: The Objects Y Coord gets changed to a seemingly random value on clients.
        //So we put it back where it should be.....
        Vector3 p = transform.position;
        p.y = 25; 
        transform.position = p;
    }
    private void Update()
    {
        if(isServer)
            for ( int i = 0; i < m_Sliders.Length; i++ )
            {
                UpdateHealthUI( i );
            }
    }

    private void UpdateHealthUI(int team)
    {
        int current = 0;
        int max = 0;
        Slider s = m_Sliders[team];
        foreach ( KeyValuePair < NetworkConnection, NetworkPlayer > networkPlayer in GameManager.AllPlayers )
        {
            NetworkPlayer p = networkPlayer.Value;

            if ( p.TeamID == team )
            {
                current += p.Health;
                max += p.PlayerSettings.MaxHealth;
            }
        }

        if ( Mathf.Abs( s.maxValue - max ) > 0.001 || Mathf.Abs( s.value - current ) > 0.001)
        {
            s.maxValue = max;
            s.value = current;
            RpcUpdateHealthUI(team, current, max);
        }
    }

    [ClientRpc]
    private void RpcUpdateHealthUI( int team, int cur, int max )
    {
        m_Sliders[team].maxValue = max;
        m_Sliders[team].value = cur;
    }
}