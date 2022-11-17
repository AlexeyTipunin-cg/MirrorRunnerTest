using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _score;
    void Start()
    {
        Score.clientOnScoreUpdate += UpdateScore;
    }

    private void OnDestroy()
    {
        Score.clientOnScoreUpdate -= UpdateScore;
    }

    private void UpdateScore(int newScore)
    {
        _score.text = $"Score: {newScore}";
    }

}
