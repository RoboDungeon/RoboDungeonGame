using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

using Mirror;

using UnityEngine;

[CreateAssetMenu(menuName = "RoboArena/GameModes/TestMode")]
public class TestGameMode : GameLogic
{

    [SerializeField]
    private bool m_DisconnectOnDeath;
    [SerializeField]
    private NetworkPlayerSettings m_PlayerSettings;

    private void SpawnPlayers(MapData data)
    {

        int team = 0;

        foreach ( KeyValuePair < NetworkConnection, NetworkPlayer > player in GameManager.AllPlayers )
        {
            player.Value.RpcSetTeamID(team);
            player.Value.RpcSetSettings( m_PlayerSettings );
            player.Value.RpcRespawn(data.TeamData[team].SpawnPoints[0].position);
            if(m_DisconnectOnDeath)
            player.Value.OnDeath += () => OnPlayerDead(player.Key, player.Value);
            team++;
            if ( team >= data.TeamData.Length )
            {
                team = 0;
            }
        }
    }

    private void OnPlayerDead(NetworkConnection c, NetworkPlayer p)
    {
       NetworkServer.Destroy( p.gameObject );
    }

    public override void StartGame( GameObject map )
    {
        SpawnPlayers( map.GetComponent < MapData >() );

    }

}
