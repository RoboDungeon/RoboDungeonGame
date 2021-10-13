using System.Collections;

using UnityEngine;

public abstract class GameLogic : ScriptableObject
{
    public abstract string Name { get; }

    public NetworkPlayerSettings PlayerSettings => m_PlayerSettings;

    [SerializeField]
    protected NetworkPlayerSettings m_PlayerSettings;
    public virtual IEnumerator StartGame(GameObject map){yield break;}

}