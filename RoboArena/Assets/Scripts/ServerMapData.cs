using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class ServerMatchListData
{

    public ServerMatchData[] MatchData;

}

[Serializable]
public class ServerMatchData
{

    public string Map;
    public string GameMode;
    public int MinPlayers;
    public NetworkPlayerSettings Settings;

}
