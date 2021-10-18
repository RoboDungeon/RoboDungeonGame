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

    private bool m_EndGame = false;
    private void SpawnPlayers(MapData data)
    {

        int team = 0;

        foreach ( KeyValuePair < NetworkPlayer, NetworkConnection> player in GameManager.AllPlayers )
        {
            player.Key.SetTeamID(team);
            player.Key.SetSettings( PlayerSettings, data.TeamData[team]);
            player.Key.Respawn(data.TeamData[team].SpawnPoints[0].position);
            player.Key.OnDeath += () => OnPlayerDead(player.Value, player.Key);
            m_AlivePlayers.Add( player.Key);
            team++;
            if ( team >= data.TeamData.Length )
            {
                team = 0;
            }
        }
    }

    private void EnablePlayerActions( bool enable )
    {
        foreach ( KeyValuePair <NetworkPlayer, NetworkConnection> player in GameManager.AllPlayers )
        {
            player.Key.SetEnableActions( enable );
        }
    }

    private void OnPlayerDead(NetworkConnection c, NetworkPlayer p)
    {
        p.DisablePlayer();

        if ( PlayerEliminatedUI.Instance != null )
            PlayerEliminatedUI.Instance.TargetDisplay( p.netIdentity.connectionToClient );

        m_AlivePlayers.Remove( p );

        if ( m_AlivePlayers.Count == 1 && PlayerWinUI.Instance != null)
        {
            PlayerWinUI.Instance.TargetDisplay(m_AlivePlayers[0].netIdentity.connectionToClient);
            m_AlivePlayers.Clear();
            m_EndGame = true;
        }
    }

    public override IEnumerator StartGame( GameObject map )
    {

        m_EndGame = false;


        m_AlivePlayers.Clear();
        if (GameStartCountdownUI.Instance != null)
            GameStartCountdownUI.Instance.RpcStartGameCountdown(PlayerSettings.GameStartCountdown);

        SpawnPlayers(map.GetComponent<MapData>());
        yield return new WaitForSeconds(PlayerSettings.GameStartCountdown);

        EnablePlayerActions( true );

        while ( !m_EndGame && GameManager.AllPlayers.Count!=0)
        {
            yield return new WaitForSeconds( 1 );
        }

        GameManager.Instance.EndGame();

    }

}
