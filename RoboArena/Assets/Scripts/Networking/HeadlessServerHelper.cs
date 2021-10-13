using System;

using UnityEngine;
using UnityEngine.Rendering;

public class HeadlessServerHelper : MonoBehaviour
{

    private bool IsHeadless => SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null;

    #region Unity Event Functions

    private void Start()
    {
        if ( IsHeadless )
        {
            string[] args = Environment.GetCommandLineArgs();
            string host = args[1];
            string port = args[2];
            Debug.Log($"Starting Server Mode: {host}:{port}");

            ArenaSceneData.Instance.SetHostName( host );
            ArenaSceneData.Instance.SetHostPort( ushort.Parse( port ) );
            ArenaSceneData.Instance.StartGameAtPlayerCount = 2;
            ArenaSceneData.Instance.CloseIfEmpty = true;
            ArenaSceneData.Instance.StartServer();
        }
    }

    #endregion

}
