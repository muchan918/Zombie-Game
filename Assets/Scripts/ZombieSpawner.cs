using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public GameManager gameManager;
    public UIManager uiManager;
    public Zombie prefab;

    public ZombieData[] zombieDatas;
    public Transform[] spawnPoints;

    private List<Zombie> zombies = new List<Zombie>();

    private int wave = 0;

    private void Update()
    {
        if (zombies.Count == 0)
        {
            SpawnWave();
        }
    }

    private void SpawnWave()
    {
        wave++;
        int count = Mathf.RoundToInt(wave * 1.5f);
        for (int i = 0; i < count; i++)
        {
            CreateZombie();
        }
        uiManager.SetWaveInfoText(wave, zombies.Count);
    }

    private void CreateZombie()
    {
        var point = spawnPoints[Random.Range(0, spawnPoints.Length)];
        var zombie = Instantiate(prefab, point.position, point.rotation);
        zombie.Setup(zombieDatas[Random.Range(0, zombieDatas.Length)]);
        zombies.Add(zombie);

        zombie.OnDead.AddListener(() => zombies.Remove(zombie));
        zombie.OnDead.AddListener(() => gameManager.AddScore(100));
        zombie.OnDead.AddListener(() => uiManager.SetWaveInfoText(wave, zombies.Count));
        zombie.OnDead.AddListener(() => Destroy(zombie.gameObject, 5f));
    }
}
