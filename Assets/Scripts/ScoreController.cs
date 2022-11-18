using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using System.Collections;

/*
	Documentation: https://mirror-networking.gitbook.io/docs/guides/networkbehaviour
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

// NOTE: Do not put objects in DontDestroyOnLoad (DDOL) in Awake.  You can do that in Start instead.

public class ScoreController : NetworkBehaviour
{
    [SerializeField] private int _winScore; 

    [SyncVar(hook = nameof(ClientOnScoreUpdate))]
    private int _score;
    public int score => _score;

    public static event Action<int> clientOnScoreUpdate;
    public static event Action<string> clientOnGameOver;
    private bool _gameIsOver;

    [Server]
    public void ServerGivePoints(int points)
    {
        if (!_gameIsOver)
        {
            _score += points;
            if (_score == _winScore)
            {
                _gameIsOver = true;
                GameOver(connectionToClient.identity.name);
                StartCoroutine(ResetGame());
            }
        }
    }

    public IEnumerator ResetGame()
    {
        yield return new WaitForSeconds(10f);
        NetworkManager.singleton.ServerChangeScene("BattleScene");
    }

    [Server]
    public void ServerResetScore()
    {
        _gameIsOver = false;
        _score = 0;
    }

    [ClientRpc]
    private void GameOver(string name)
    {
        clientOnGameOver?.Invoke(name);
        NetworkClient.Ready();
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
