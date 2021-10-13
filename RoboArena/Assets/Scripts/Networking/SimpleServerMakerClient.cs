using System.Collections;
using System.Collections.Generic;
using System.Net;

using UnityEngine;
using UnityEngine.Networking;

public class SimpleServerMakerClient : MonoBehaviour
{
    [SerializeField]
    private bool m_UseSSL;
    [SerializeField]
    private string m_Url;
    [SerializeField]
    private string m_Port;
    [SerializeField]
    private string m_Key;

    private void Awake()
    {
        ServicePointManager.ServerCertificateValidationCallback = ( sender, certificate, chain, errors ) => true;
    }

    public void RequestServer()
    {
        StartCoroutine( RequestRoutine() );
    }

    private IEnumerator RequestRoutine()
    {
        
        //http://localhost:6969/make_server?key=12345678
        string url;
        if ( m_UseSSL )
        {
            url = $"https://{m_Url}:{m_Port}/make_server?key={m_Key}";
        }
        else
        {
            url = $"http://{m_Url}:{m_Port}/make_server?key={m_Key}";
        }
        UnityWebRequest r = UnityWebRequest.Get(url);

        yield return r.SendWebRequest();
        if (r.result != UnityWebRequest.Result.Success)
        {
            Debug.LogFormat("Error downloading file: <color=red>{0}</color> | Error code: <color=red>{1}</color>", r.downloadHandler.text, r.error);
        }
        else if ( r.responseCode == 200 )
        {
            ushort port = ushort.Parse(r.downloadHandler.text);
            Debug.Log( $"Server Running on Address: {m_Url}:{port}" );
            ArenaSceneData.Instance.SetHostName(m_Url);
            ArenaSceneData.Instance.SetHostPort(port);

            yield return new WaitForSeconds( 1 );
            ArenaSceneData.Instance.StartClient();
        }
        else
        {
            Debug.Log( "Invalid Response: " + r.responseCode );
        }
    }
}
