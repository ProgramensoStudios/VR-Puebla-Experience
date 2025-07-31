using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField] private Image hpBar;
    [SerializeField] private Image shieldBar;
    [SerializeField] private PlayerController player;

    private float maxHp;
    private float maxShield;
    
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private ScoreManager scoreManager;
    
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private CurrencyManager currencyManager;

    private void Start()
    {
        maxHp = player.hp;
        maxShield = player.maxHpShield;

        player.OnHpChanged += UpdateHpBar;
        player.OnShieldChanged += UpdateShieldBar;

        UpdateHpBar(player.hp);
        UpdateShieldBar(player.shieldHp, player.hasShield);
        
        if (scoreManager == null)
        {
            Debug.LogError("Falta ScoreManager en HUDManager");
            return;
        }

        scoreManager.OnScoreChanged += UpdateScoreUI;
        UpdateScoreUI(scoreManager.GetScore());
        
        if (currencyManager == null)
        {
            Debug.LogError("CurrencyManager no asignado en HUD");
            return;
        }

        currencyManager.OnCoinsChanged += UpdateCoinUI;
        UpdateCoinUI(currencyManager.GetCoins());
    }
    
    private void UpdateCoinUI(int coins)
    {
        coinText.text = coins.ToString();
    }
    private void UpdateScoreUI(int score)
    {
        scoreText.text = score.ToString();
    }

    private void UpdateHpBar(float currentHp)
    {
        hpBar.fillAmount = Mathf.Clamp01(currentHp / maxHp);
    }

    private void UpdateShieldBar(float currentShield, bool isActive)
    {
        if (isActive)
        {
            shieldBar.gameObject.SetActive(true);
            shieldBar.fillAmount = Mathf.Clamp01(currentShield / maxShield);
        }
        else
        {
            shieldBar.gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        player.OnHpChanged -= UpdateHpBar;
        player.OnShieldChanged -= UpdateShieldBar;
        if (scoreManager != null)
            scoreManager.OnScoreChanged -= UpdateScoreUI;
        if (currencyManager != null)
        {
            currencyManager.OnCoinsChanged -= UpdateCoinUI;
        }
    }
}