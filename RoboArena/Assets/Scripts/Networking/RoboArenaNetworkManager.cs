using System;

using Mirror;

using UnityEngine;

namespace Assets.Scripts.Networking
{

    public class RoboArenaNetworkManager : NetworkManager
    {

        public event Action OnQuit;
        public event Action OnClientStart;
        public event Action OnServerStart;
        public event Action OnHostStart;
        public event Action OnClientStop;
        public event Action OnServerStop;
        public event Action OnHostStop;
        public event Action<string, SceneOperation, bool> OnChangeClientScene;
        public event Action<string> OnChangeServerScene;
        public event Action<string> OnChangedServerScene;
        public event Action<NetworkConnection> OnDisconnectClient;
        public event Action<NetworkConnection> OnConnectClient;
        public event Action<NetworkConnection> OnNotReadyClient;
        public event Action<NetworkConnection> OnChangedClientScene;
        public event Action<NetworkConnection, GameObject> OnAddServerPlayer; 
        public event Action<NetworkConnection> OnConnectServer;
        public event Action<NetworkConnection> OnDisconnectServer;
        public event Action<NetworkConnection> OnReadyServer;
        public event Action<Exception> OnClientErrored;
        public event Action<NetworkConnection, Exception> OnServerErrored;

        public Transport Transport => transport;


        public override void OnApplicationQuit()
        {
            OnQuit?.Invoke();
            base.OnApplicationQuit();
        }

        public override void OnClientChangeScene( string newSceneName, SceneOperation sceneOperation, bool customHandling )
        {
            base.OnClientChangeScene( newSceneName, sceneOperation, customHandling );
            OnChangeClientScene?.Invoke(newSceneName, sceneOperation, customHandling);
        }

        public override void OnClientConnect( NetworkConnection conn )
        {
            base.OnClientConnect( conn );
            OnConnectClient?.Invoke(conn);
        }

        public override void OnClientDisconnect( NetworkConnection conn )
        {
            base.OnClientDisconnect( conn );
            OnDisconnectClient?.Invoke(conn);
        }

        public override void OnClientError( Exception exception )
        {
            base.OnClientError( exception );
            OnClientErrored?.Invoke( exception );
        }

        public override void OnClientNotReady( NetworkConnection conn )
        {
            base.OnClientNotReady( conn );
            OnNotReadyClient?.Invoke( conn );
        }

        public override void OnClientSceneChanged( NetworkConnection conn )
        {
            base.OnClientSceneChanged( conn );
            OnChangedClientScene?.Invoke(conn);
        }

        public GameObject AddPlayerObject(NetworkConnection conn)
        {
            Transform startPos = GetStartPosition();
            GameObject player = startPos != null
                                    ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
                                    : Instantiate(playerPrefab);

            // instantiating a "Player" prefab gives it the name "Player(clone)"
            // => appending the connectionId is WAY more useful for debugging!
            player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
            NetworkServer.AddPlayerForConnection(conn, player);

            return player;
        }

        public override void OnServerAddPlayer( NetworkConnection conn )
        {
            GameObject o = AddPlayerObject( conn );
            NetworkPlayer p = o.GetComponent < NetworkPlayer >();
            p.RpcAddToPlayerList();
            OnAddServerPlayer?.Invoke(conn, o);
        }

        public override void OnServerChangeScene( string newSceneName )
        {
            base.OnServerChangeScene( newSceneName );
            OnChangeServerScene?.Invoke( newSceneName );
        }

        public override void OnServerConnect( NetworkConnection conn )
        {
            base.OnServerConnect( conn );
            OnConnectServer?.Invoke( conn );
        }

        public override void OnServerDisconnect( NetworkConnection conn )
        {
            base.OnServerDisconnect( conn );
            OnDisconnectServer?.Invoke(conn);
        }

        public override void OnServerError( NetworkConnection conn, Exception exception )
        {
            base.OnServerError( conn, exception );
            OnServerErrored?.Invoke( conn, exception );
        }

        public override void OnServerReady( NetworkConnection conn )
        {
            base.OnServerReady( conn );
            OnReadyServer?.Invoke(conn);
        }

        public override void OnServerSceneChanged( string sceneName )
        {
            base.OnServerSceneChanged( sceneName );
            OnChangedServerScene?.Invoke( sceneName );
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            OnClientStart?.Invoke();
        }

        public override void OnStartHost()
        {
            base.OnStartHost();
            OnHostStart?.Invoke();
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            OnServerStart?.Invoke();
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            OnClientStop?.Invoke();
        }

        public override void OnStopHost()
        {
            base.OnStopHost();
            OnHostStop?.Invoke();
        }

        public override void OnStopServer()
        {
            base.OnStopServer();
            OnServerStop?.Invoke();
        }


    }

}
