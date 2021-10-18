using UnityEngine;

public class ServerAutoJoin : MonoBehaviour
{
    [SerializeField]
    private string m_Url;
    [SerializeField]
    private string m_Port;

    public void Connect()
    {
        ArenaSceneData.Instance.SetHostName(m_Url);
        ArenaSceneData.Instance.SetHostPort(m_Port);
        ArenaSceneData.Instance.StartClient();
    }
}
