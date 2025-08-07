using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField] private Slider hpBar;
    [SerializeField] private PlayerController player;

    private float maxHp;
    
    //[SerializeField] private TextMeshProUGUI scoreText;
    //[SerializeField] private ScoreManager scoreManager;
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private TextMeshProUGUI endText;
    [SerializeField] private CurrencyManager currencyManager;

    private void Start()
    {
        maxHp = player.hp;

        player.OnHpChanged += UpdateHpBar;
        currencyManager.OnCoinsChanged += UpdateCoinUI;

        UpdateHpBar(player.hp);
        
        //if (scoreManager == null)
        //{
        //    Debug.LogError("Falta ScoreManager en HUDManager");
        //    return;
       //}

        //scoreManager.OnScoreChanged += UpdateScoreUI;
        //UpdateScoreUI(scoreManager.GetScore());
        
        //if (currencyManager == null)
        //{
        //    Debug.LogError("CurrencyManager no asignado en HUD");
        //    return;
        //}

        //currencyManager.OnCoinsChanged += UpdateCoinUI;
        //UpdateCoinUI(currencyManager.GetCoins());
    }
    
    private void UpdateCoinUI(int coins)
    { 
        coinText.text = coins.ToString();
        endText.text = coins.ToString();
    }
    private void UpdateScoreUI(int score)
    {
        //scoreText.text = score.ToString();
    }

    private void UpdateHpBar(float currentHp)
    {
        hpBar.value = Mathf.Clamp01(currentHp / maxHp);
    }
    

    private void OnDestroy()
    {
        player.OnHpChanged -= UpdateHpBar;
       // if (scoreManager != null)
       //     scoreManager.OnScoreChanged -= UpdateScoreUI;
       // if (currencyManager != null)
       // {
       //     currencyManager.OnCoinsChanged -= UpdateCoinUI;
       // }
    }
}