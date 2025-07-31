using System;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    private int currentCoins = 0;

    public event Action<int> OnCoinsChanged;

    public void AddCoins(int amount)
    {
        currentCoins += amount;
        OnCoinsChanged?.Invoke(currentCoins);
    }

    public int GetCoins()
    {
        return currentCoins;
    }
}