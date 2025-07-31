using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int currentScore = 0;

    public event Action<int> OnScoreChanged;

    public void AddScore(int amount)
    {
        currentScore += amount;
        OnScoreChanged?.Invoke(currentScore);
    }

    public int GetScore()
    {
        return currentScore;
    }
}