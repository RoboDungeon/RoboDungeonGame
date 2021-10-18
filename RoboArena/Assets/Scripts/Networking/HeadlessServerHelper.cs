using System;
using System.IO;

using UnityEngine;
using UnityEngine.Rendering;

public class HeadlessServerHelper : MonoBehaviour
{

    [SerializeField]
    private ServerMatchListData m_ServerData; 
    private bool IsHeadless => SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null;
    [SerializeField]
    private bool m_SerializeMapData = false;

    #region Unity Event Functions

    private void Start()
    {
        if ( IsHeadless )
        {
            string[] args = Environment.GetCommandLineArgs();
            string host = args[1];
            string port = args[2];
            string matchSettings = args[3];
            Debug.Log($"Starting Server Mode: {host}:{port}");

            ArenaSceneData.Instance.SetHostName( host );
            ArenaSceneData.Instance.SetHostPort( ushort.Parse( port ) );
            m_ServerData = JsonUtility.FromJson < ServerMatchListData >( File.ReadAllText( matchSettings ) );
            ArenaSceneData.Instance.MatchData = m_ServerData;
            ArenaSceneData.Instance.StartServer();
        }
        else if ( m_SerializeMapData )
        {
            File.WriteAllText( "./mapsettings.json", JsonUtility.ToJson( m_ServerData,true ) );
        }
    }

    #endregion

}
