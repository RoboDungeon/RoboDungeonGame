using System;

using UnityEngine;

[Serializable]
public class TeamData
{
    [SerializeField]
    private Transform[] m_SpawnPoints;

    public Transform[] SpawnPoints => m_SpawnPoints;

}
