using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ArenaSceneData : Singleton <ArenaSceneData>
{

    [SerializeField]
    private ServerMatchListData m_MatchData;
    [SerializeField]
    private int m_CurrentMatch = 0;
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

    public void MoveNextMatch()
    {
        m_CurrentMatch++;

        if ( CurrentData == null )
            m_CurrentMatch = 0;
    }
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

    public ServerMatchData CurrentData =>
        m_MatchData.MatchData.Length > m_CurrentMatch ? m_MatchData.MatchData[m_CurrentMatch] : null;

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

    public ServerMatchListData MatchData
    {
        get => m_MatchData;
        set => m_MatchData = value;
    }

    public int CurrentMatch
    {
        get => m_CurrentMatch;
        set => m_CurrentMatch = value;
    }

}
