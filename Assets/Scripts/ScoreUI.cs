using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    public TMP_Text scoreText;
    public string template = "Points: ";
    private int _points;

    public void ResetScore()
    {
        _points = 0;
        scoreText.text = template + _points;
    }

    public void AddPoints(int points)
    {
        _points += points;
        scoreText.text = template + _points;
    }
}