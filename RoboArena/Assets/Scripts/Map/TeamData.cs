using System;

using UnityEngine;

[Serializable]
public class TeamData
{
    [SerializeField]
    private Transform[] m_SpawnPoints;

    [SerializeField]
    private Color m_TeamColor;
    [SerializeField]
    private Color m_TeamBulletColor;

    public Transform[] SpawnPoints => m_SpawnPoints;

    public Color TeamBulletColor => m_TeamBulletColor;

    public Color TeamColor => m_TeamColor;

}
