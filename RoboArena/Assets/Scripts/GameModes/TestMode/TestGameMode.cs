using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

using Mirror;

using UnityEngine;

[CreateAssetMenu(menuName = "RoboArena/GameModes/TestMode")]
public class TestGameMode : GameLogic
{

    public override string Name => m_ModeName;

    private readonly List <NetworkPlayer> m_AlivePlayers = new List <NetworkPlayer>();
    [SerializeField]
    private string m_ModeName = "TestMode";

    private void SpawnPlayers(MapData data)
    {

        int team = 0;

        foreach ( KeyValuePair < NetworkConnection, NetworkPlayer > player in GameManager.AllPlayers )
        {
            player.Value.SetTeamID(team);
            player.Value.RpcSetSettings( PlayerSettings );
            player.Value.RpcRespawn(data.TeamData[team].SpawnPoints[0].position);
            player.Value.OnDeath += () => OnPlayerDead(player.Key, player.Value);
            m_AlivePlayers.Add( player.Value );
            team++;
            if ( team >= data.TeamData.Length )
            {
                team = 0;
            }
        }
    }

    private void EnablePlayerActions( bool enable )
    {
        foreach ( KeyValuePair < NetworkConnection, NetworkPlayer > player in GameManager.AllPlayers )
        {
            player.Value.RpcEnableActions( enable );
        }
    }

    private void OnPlayerDead(NetworkConnection c, NetworkPlayer p)
    {
        NetworkServer.Destroy( p.gameObject );

        if ( PlayerEliminatedUI.Instance != null )
            PlayerEliminatedUI.Instance.TargetDisplay( p.netIdentity.connectionToClient );

        m_AlivePlayers.Remove( p );

        if ( m_AlivePlayers.Count == 1 && PlayerWinUI.Instance != null)
        {
            PlayerWinUI.Instance.TargetDisplay(m_AlivePlayers[0].netIdentity.connectionToClient);
        }
    }

    public override IEnumerator StartGame( GameObject map )
    {
        m_AlivePlayers.Clear();
        if (GameStartCountdownUI.Instance != null)
            GameStartCountdownUI.Instance.RpcStartGameCountdown(PlayerSettings.GameStartCountdown);

        SpawnPlayers(map.GetComponent<MapData>());
        yield return new WaitForSeconds(PlayerSettings.GameStartCountdown);

        EnablePlayerActions( true );

    }

}
