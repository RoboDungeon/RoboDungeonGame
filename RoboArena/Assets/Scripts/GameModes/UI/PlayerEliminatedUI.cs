using Mirror;

using UnityEngine;

public class PlayerEliminatedUI: NetworkBehaviour
{

    public static PlayerEliminatedUI Instance { get; private set; }

    [SerializeField]
    private GameObject m_EliminatedUI;
    

    private void Awake()
    {
        Instance = this;
    }
    [TargetRpc]
    public void TargetDisplay(NetworkConnection c)
    {
        m_EliminatedUI.SetActive(true);
    }
    [ClientRpc]
    public void RpcHide()
    {
        m_EliminatedUI.SetActive(false);
    }

}