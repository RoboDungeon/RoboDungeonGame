using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Assets.Scripts.Networking;

using Mirror;

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RoboArenaNetworkManager))]
public class GameManager : MonoBehaviour
{
    public static Dictionary <NetworkPlayer, NetworkConnection> AllPlayers { get; } = new Dictionary<NetworkPlayer, NetworkConnection>();
    public static GameManager Instance { get; private set; }
    [Header("Debug UI")]
    [SerializeField]
    private Text m_ServerDebugInfo;

    [SerializeField]
    private Button m_StartGameButton;
    [SerializeField]
    private Button[] m_DisconnectButtons;
    [SerializeField]
    private GameLogic[] m_GameModes;

    public bool IsRunning { get; private set; }

    [SerializeField]
    private GameManagerMapData[] m_Maps;

    private GameObject m_CurrentMap = null;


    private void Start()
    {
        Instance = this;
        RoboArenaNetworkManager mgr = GetComponent < RoboArenaNetworkManager >();
        mgr.OnAddServerPlayer+= OnClientConnected;
        mgr.OnDisconnectServer+= OnClientDisconnected;
        mgr.networkAddress = ArenaSceneData.Instance.HostName;
        mgr.Transport.Port = ArenaSceneData.Instance.HostPort;

        if ( m_ServerDebugInfo != null )
        {
            m_ServerDebugInfo.text = $"{mgr.networkAddress}:{mgr.Transport.Port}";
        }

        switch ( ArenaSceneData.Instance.ConnectionType )
        {
            case ArenaSceneConnectionType.Server:
                if (m_StartGameButton != null)
                {
                    m_StartGameButton.gameObject.SetActive(true);
                }

                foreach ( Button disconnectButton in m_DisconnectButtons )
                {
                    disconnectButton.onClick.AddListener(
                                                         () =>
                                                         {
                                                             IsRunning = false;
                                                             mgr.StopServer();
                                                             AllPlayers.Clear();
                                                         } );
                }
                mgr.StartServer();
                break;

            case ArenaSceneConnectionType.Host:
                if (m_StartGameButton != null)
                {
                    m_StartGameButton.gameObject.SetActive(true);
                }

                foreach (Button disconnectButton in m_DisconnectButtons)
                {
                    disconnectButton.onClick.AddListener(
                                                         () =>
                                                         {
                                                             IsRunning = false;
                                                             mgr.StopHost();
                                                             AllPlayers.Clear();
                                                         });
                }
                mgr.StartHost();
                break;

            case ArenaSceneConnectionType.Client:
                if (m_StartGameButton != null)
                {
                    m_StartGameButton.gameObject.SetActive(false);
                }

                foreach (Button disconnectButton in m_DisconnectButtons)
                {
                    disconnectButton.onClick.AddListener(() => mgr.StopClient());
                }
                mgr.StartClient();
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void OnClientDisconnected( NetworkConnection obj )
    {
        Debug.Log( "Disconnected Player: " + obj);

        NetworkPlayer p = null;
        foreach ( KeyValuePair < NetworkPlayer, NetworkConnection > networkConnection in AllPlayers )
        {
            if ( networkConnection.Value == obj )
                p = networkConnection.Key;
        }
        if(p != null)
            AllPlayers.Remove( p );

        
    }

    private void OnClientConnected( NetworkConnection obj, GameObject o )
    {
        Debug.Log( "Add Player: " + o );
        AllPlayers[o.GetComponent<NetworkPlayer>()] = obj;

        if ( ArenaSceneData.Instance.CurrentData.MinPlayers <= AllPlayers.Count )
        {
            StartGame();
        }
    }

    private GameObject SpawnMap()
    {
        GameObject prefab = m_Maps.First( x => x.Name == ArenaSceneData.Instance.CurrentData.Map).Prefab;
        GameObject instance = Instantiate( prefab );
        NetworkServer.Spawn( instance );

        return instance;
    }

    [Server]
    public void EndGame()
    {
        if ( !IsRunning )
        {
            Debug.LogError("No Game logic running.");
        }

        if ( m_CurrentMap != null )
        {
            NetworkServer.Destroy( m_CurrentMap );
        }
        IsRunning = false;

        ArenaSceneData.Instance.MoveNextMatch();

        if (PlayerEliminatedUI.Instance != null)
            PlayerEliminatedUI.Instance.RpcHide();
        if (PlayerWinUI.Instance != null)
            PlayerWinUI.Instance.RpcHide();

        if ( ArenaSceneData.Instance.CurrentData != null )
        {
            StartCoroutine( StartGameDelayed() );
        }
        else
        {
            StartCoroutine(QuitDelayed());
        }
    }

    [Server]
    public void StartGame()
    {
        if ( IsRunning )
        {
            Debug.LogError( "Game logic already running." );

            return;
        }

        IsRunning = true;
        GameObject map = SpawnMap();
        GameLogic mode = m_GameModes.FirstOrDefault( x => x.Name == ArenaSceneData.Instance.CurrentData.GameMode );
        m_CurrentMap = map;
        if (mode != null )
        {
            StartCoroutine(mode.StartGame(map));
        }
        else
        {
            Debug.LogWarning( "No Game Logic attached for game" );
        }
    }


    private IEnumerator QuitDelayed()
    {
        yield return new WaitForSeconds(10);

        Application.Quit();
    }

    private IEnumerator StartGameDelayed()
    {
        yield return new WaitForSeconds( 10 );

        StartGame();
    }
}
