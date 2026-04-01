using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public Text ammoText;
    public Text scoreText;
    public Text waveText;

    public GameObject gameOverUI;

    public void OnEnable()
    {
        //SetAmmoText(0, 0);
        SetScoreText(0);
        SetWaveInfoText(0, 0);
        SetActiveGameOverUI(false);
    }

    public void SetAmmoText(int magAmmo, int remainAmmo)
    {
        ammoText.text = $"{magAmmo} / {remainAmmo}";
    }

    public void SetScoreText(int score)
    {
        scoreText.text = $"Score: {score}";
    }

    public void SetWaveInfoText(int wave, int count)
    {
        waveText.text = $"Wave: {wave}\nEnemy Left: {count}";
    }

    public void SetActiveGameOverUI(bool active)
    {
        gameOverUI.SetActive(active);
    }

    public void OnClickRestart()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex);
    }
}
