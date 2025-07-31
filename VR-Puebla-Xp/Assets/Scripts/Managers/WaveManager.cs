using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private Canvas hudCanvas;
    [SerializeField] private Canvas shopCanvas;
    public void WaveEnded()
    {
        Time.timeScale = 0;
        hudCanvas.enabled = false;
        shopCanvas.enabled = true;
    }

    public void ContinueGame()
    {
        Time.timeScale = 1;
        hudCanvas.enabled = true;
        shopCanvas.enabled = false;
    }
}
