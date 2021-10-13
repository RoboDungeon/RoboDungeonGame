using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ArenaSceneData : Singleton <ArenaSceneData>
{

    [SerializeField]
    private int m_StartGameAtPlayerCount = -1;

    [SerializeField]
    private bool m_CloseIfEmpty;
    [SerializeField]
    private string m_HostName;
    [SerializeField]
    private ushort m_HostPort;
    [SerializeField]
    private ArenaSceneConnectionType m_ConnectionType;

    protected override void Awake()
    {
        if ( InstanceIfInitialized != null )
        {
            DestroyImmediate(InstanceIfInitialized.gameObject);
        }
        base.Awake();
        DontDestroyOnLoad( gameObject );
    }


    public void SetHostName(string name) => m_HostName = name;
    public void SetHostPort(ushort port) => m_HostPort = port;
    public void SetHostPort(string port) => m_HostPort = ushort.Parse(port);

    public void StartClient()
    {
        m_ConnectionType = ArenaSceneConnectionType.Client;
        SceneManager.LoadScene( "ArenaScene" );
    }
    public void StartHost()
    {
        m_ConnectionType = ArenaSceneConnectionType.Host;
        SceneManager.LoadScene("ArenaScene");
    }
    public void StartServer()
    {
        m_ConnectionType = ArenaSceneConnectionType.Server;
        SceneManager.LoadScene("ArenaScene");
    }

    public string HostName
    {
        get => m_HostName;
        set => m_HostName = value;
    }
    public ushort HostPort
    {
        get => m_HostPort;
        set => m_HostPort = value;
    }

    public ArenaSceneConnectionType ConnectionType
    {
        get => m_ConnectionType;
        set => m_ConnectionType = value;
    }

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

}
