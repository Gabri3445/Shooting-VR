using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    public TMP_Text scoreText;
    public string template = "Points: ";
    private int _points = 0;

    public void ResetScore()
    {
        _points = 0;
    }

    public void AddPoints(int points)
    {
        _points += points;
        scoreText.text = template + _points;
    }
}
