using System;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    private int currentCoins = 0;
    [SerializeField] private AudioSource sfxCoin;
    [SerializeField] private EditableTimer timer;
    [SerializeField] private int enemsInGame;

    private void Awake()
    {
        timer = gameObject.GetComponent<EditableTimer>();
    }

    public event Action<int> OnCoinsChanged;

    public void AddCoins(int amount)
    {
        currentCoins += amount;
        OnCoinsChanged?.Invoke(currentCoins);
        sfxCoin.Play();
        Debug.Log("AUDIO");
        if (currentCoins >= enemsInGame)
        {
            timer.EndTime();
            EditableTimer.onTimerEnd?.Invoke();
        }
    }

    public int GetCoins()
    {
        return currentCoins;
    }
}