using System.Collections;
using System.Collections.Generic;

using Mirror;

using UnityEngine;

[CreateAssetMenu(menuName = "RoboArena/GameModes/TestPVEMode")]
public class TestPVEGameMode : GameLogic
{

    public override string Name => m_ModeName;

    private readonly List<NetworkPlayer> m_AlivePlayers = new List<NetworkPlayer>();
    private readonly List<NetworkPlayer> m_AliveAIPlayers = new List<NetworkPlayer>();
    [SerializeField]
    private string m_ModeName = "TestPVEMode";
    [SerializeField]
    private int m_AIPlayers = 1;
    [SerializeField]
    private int m_PlayerTeam = 1;
    [SerializeField]
    private int m_AITeam = 0;
    [SerializeField]
    private GameObject m_AIPlayerPrefab;

    private void SpawnPlayers(MapData data)
    {

        int team = m_PlayerTeam;

        foreach (KeyValuePair<NetworkPlayer, NetworkConnection> player in GameManager.AllPlayers)
        {
            player.Key.SetTeamID(team);
            player.Key.SetSettings(PlayerSettings, data.TeamData[team]);
            player.Key.Respawn(data.TeamData[team].SpawnPoints[0].position);
            player.Key.OnDeath += () => OnPlayerDead(player.Value, player.Key);
            m_AlivePlayers.Add(player.Key);
        }

        team = m_AITeam;

        for ( int i = 0; i < m_AIPlayers; i++ )
        {
            Debug.Log("Spawing AI");
            NetworkPlayer p = Instantiate( m_AIPlayerPrefab ).GetComponent < NetworkPlayer >();
            GameManager.AllPlayers.Add( p, null );
            if(NetworkPlayer.LocalPlayer?.connectionToClient != null)
                NetworkServer.Spawn(p.gameObject, NetworkPlayer.LocalPlayer.connectionToClient);
            else
                NetworkServer.Spawn(p.gameObject);
            p.SetTeamID( team );
            p.SetSettings( PlayerSettings, data.TeamData[team] );
            p.Respawn( data.TeamData[team].SpawnPoints[0].position );
            p.OnDeath += () => OnAIPlayerDead(p);
            m_AliveAIPlayers.Add( p );
            m_AlivePlayers.Add( p );
        }
    }

    private void EnablePlayerActions(bool enable)
    {
        foreach (KeyValuePair<NetworkPlayer, NetworkConnection> player in GameManager.AllPlayers)
        {
            player.Key.SetEnableActions(enable);
        }

        foreach ( NetworkPlayer player in m_AliveAIPlayers)
        {
            player.SetEnableActions(enable);
        }
    }

    private void OnAIPlayerDead( NetworkPlayer p )
    {
        NetworkServer.Destroy(p.gameObject);
        m_AlivePlayers.Remove(p);
        m_AliveAIPlayers.Remove(p);
        if (m_AliveAIPlayers.Count == 0 && PlayerWinUI.Instance != null)
        {
            foreach ( NetworkPlayer networkPlayer in m_AlivePlayers )
            {
                PlayerWinUI.Instance.TargetDisplay(networkPlayer.netIdentity.connectionToClient);
            }
        }

        GameManager.AllPlayers.Remove( p );
    }

    private void OnPlayerDead(NetworkConnection c, NetworkPlayer p)
    {
        NetworkServer.Destroy(p.gameObject);

        if (PlayerEliminatedUI.Instance != null)
            PlayerEliminatedUI.Instance.TargetDisplay(p.netIdentity.connectionToClient);

        m_AlivePlayers.Remove(p);

        
    }

    public override IEnumerator StartGame(GameObject map)
    {
        m_AlivePlayers.Clear();
        m_AliveAIPlayers.Clear();
        if (GameStartCountdownUI.Instance != null)
            GameStartCountdownUI.Instance.RpcStartGameCountdown(PlayerSettings.GameStartCountdown);

        SpawnPlayers(map.GetComponent<MapData>());
        yield return new WaitForSeconds(PlayerSettings.GameStartCountdown);

        EnablePlayerActions(true);

    }

}
