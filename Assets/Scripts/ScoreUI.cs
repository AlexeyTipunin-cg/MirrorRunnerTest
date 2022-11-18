using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _score;
    [SerializeField] private TMP_Text _winnerText;
    void Start()
    {
        ScoreController.clientOnScoreUpdate += UpdateScore;
        ScoreController.clientOnGameOver += OnGameOver;
        _winnerText.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        ScoreController.clientOnScoreUpdate -= UpdateScore;
        ScoreController.clientOnGameOver -= OnGameOver;
    }

    private void UpdateScore(int newScore)
    {
        _score.text = $"Score: {newScore}";
    }

    private void OnGameOver(string winnerName)
    {
        _winnerText.gameObject.SetActive(true);
        _winnerText.text = $"Winner is player: {winnerName}";
    }

}
