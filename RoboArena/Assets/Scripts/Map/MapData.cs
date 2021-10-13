using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapData : MonoBehaviour
{

    [SerializeField]
    private TeamData[] m_TeamData;

    public TeamData[] TeamData => m_TeamData;

}
