using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

/*
	Documentation: https://mirror-networking.gitbook.io/docs/guides/networkbehaviour
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

// NOTE: Do not put objects in DontDestroyOnLoad (DDOL) in Awake.  You can do that in Start instead.

public class Score : NetworkBehaviour
{
    [SyncVar(hook = nameof(ClientOnScoreUpdate))]
    private int _score;
    public int score => _score;

    public static event Action<int> clientOnScoreUpdate;

    [Server]
    public void ServerGivePoints(int points)
    {
        _score += points;
    }

    [Server]
    public void ServerResetScore()
    {
        _score = 0;
    }

    [Client]
    private void ClientOnScoreUpdate(int oldScore, int newScore)
    {
        if (isLocalPlayer)
        {
            clientOnScoreUpdate?.Invoke(newScore);
        }
    }
}
