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

    public static Dictionary <NetworkConnection, NetworkPlayer > AllPlayers { get; } = new Dictionary<NetworkConnection, NetworkPlayer>();

    [Header("Debug UI")]
    [SerializeField]
    private Text m_ServerDebugInfo;

    [SerializeField]
    private Button m_StartGameButton;
    [SerializeField]
    private Button m_DisconnectButton;

    [SerializeField]
    private int m_StartGameAtPlayerCount = -1;
    [SerializeField]
    private bool m_CloseIfEmpty;

    public bool IsRunning { get; private set; }
    [SerializeField]
    private GameSettings m_Settings;

    [SerializeField]
    private GameManagerMapData[] m_Maps;

    public GameSettings Settings => m_Settings;

    public int StartGameAtPlayerCount
    {
        get => m_StartGameAtPlayerCount;
        set => m_StartGameAtPlayerCount = value;
    }

    public bool CloseIfEmpty
    {
        get => m_CloseIfEmpty;
        set => m_CloseIfEmpty = value;
    }

    private void Start()
    {
        m_CloseIfEmpty = ArenaSceneData.Instance.CloseIfEmpty;
        m_StartGameAtPlayerCount = ArenaSceneData.Instance.StartGameAtPlayerCount;
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

                if ( m_DisconnectButton != null )
                {
                    m_DisconnectButton.onClick.AddListener( () => mgr.StopServer() );
                }
                mgr.StartServer();
                break;

            case ArenaSceneConnectionType.Host:
                if (m_StartGameButton != null)
                {
                    m_StartGameButton.gameObject.SetActive(true);
                }
                if (m_DisconnectButton != null)
                {
                    m_DisconnectButton.onClick.AddListener(() => mgr.StopHost());
                }
                mgr.StartHost();
                break;

            case ArenaSceneConnectionType.Client:
                if (m_StartGameButton != null)
                {
                    m_StartGameButton.gameObject.SetActive(false);
                }
                if (m_DisconnectButton != null)
                {
                    m_DisconnectButton.onClick.AddListener(() => mgr.StopClient());
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
        AllPlayers.Remove( obj );

        if ( CloseIfEmpty && AllPlayers.Count==0)
        {
            Application.Quit();
        }
    }

    private void OnClientConnected( NetworkConnection obj, GameObject o )
    {
        Debug.Log( "Add Player: " + o );
        AllPlayers[obj] = o.GetComponent<NetworkPlayer>();

        if ( StartGameAtPlayerCount != -1 && StartGameAtPlayerCount <= AllPlayers.Count )
        {
            StartGame();
        }
    }

    private GameObject SpawnMap()
    {
        GameObject prefab = m_Maps.First( x => x.Name == m_Settings.MapName ).Prefab;
        GameObject instance = Instantiate( prefab );
        NetworkServer.Spawn( instance );

        return instance;
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

        if ( m_Settings.GameLogic != null )
        {
            m_Settings.GameLogic.StartGame(map);
        }
        else
        {
            Debug.LogWarning( "No Game Logic attached for game" );
        }
    }

}