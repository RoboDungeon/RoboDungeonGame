using System;

using UnityEngine;

[Serializable]
public class GameSettings
{

    [SerializeField]
    private string m_MapName;
    [SerializeField]
    private GameLogic m_GameLogic;

    public string MapName
    {
        get => m_MapName;
        set => m_MapName = value;
    }

    public GameLogic GameLogic
    {
        get => m_GameLogic;
        set => m_GameLogic = value;
    }

}