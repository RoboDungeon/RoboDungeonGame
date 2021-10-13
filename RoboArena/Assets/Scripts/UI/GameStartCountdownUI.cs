using System;
using System.Collections;
using System.Collections.Generic;

using Mirror;

using UnityEngine;
using UnityEngine.UI;

public class GameStartCountdownUI : NetworkBehaviour
{
    public static GameStartCountdownUI Instance { get; private set; }
    [SerializeField]
    private Text m_Text;

    [SerializeField]
    private CanvasGroup m_Group;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        
        if (Math.Abs(transform.position.y - 25) > 0.0001f)
        {
            Vector2 p = ((RectTransform)transform).anchoredPosition;
            p.y = -25;
            ((RectTransform)transform).anchoredPosition = p;
        }
    }

    [ClientRpc]
    public void RpcStartGameCountdown(int cd)
    {
        StartCoroutine( CountdownRoutine(cd) );
    }

    private IEnumerator CountdownRoutine(int cd)
    {
        m_Group.alpha = 1;
        for ( int i = cd; i >= 0; i-- )
        {
            yield return new WaitForSeconds(1);

            m_Text.text = $"Match Starts in: {i}";
        }
        m_Group.alpha = 0;
    }
}
