using Mirror;

using UnityEngine;

public class PlayerWinUI : NetworkBehaviour
{

    public static PlayerWinUI Instance { get; private set; }
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

}