using System;

using UnityEngine;

[Serializable]
public struct GameManagerMapData
{
    [SerializeField]
    private string m_Name;
    [SerializeField]
    private GameObject m_Prefab;

    public string Name
    {
        get => m_Name;
        set => m_Name = value;
    }

    public GameObject Prefab
    {
        get => m_Prefab;
        set => m_Prefab = value;
    }

}