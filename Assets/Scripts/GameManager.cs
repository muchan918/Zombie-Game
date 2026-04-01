using NUnit.Framework;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public UIManager uiManager;
    public ZombieSpawner spawner;
    private int score = 0;
    public bool IsGameOver { get; private set; }

    public void Start()
    {
        uiManager.SetScoreText(score);
    }

    public void AddScore(int add)
    {
        if (IsGameOver)
            return;

        score += add;
        uiManager.SetScoreText(score);
    }

    public void EndGame()
    {
        IsGameOver = true;
        spawner.enabled = false;
        uiManager.SetActiveGameOverUI(true);
    }
}
