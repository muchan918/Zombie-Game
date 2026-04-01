using UnityEngine;
using UnityEngine.AI;

public class ItemSpawner : MonoBehaviour
{
    public GameObject[] items;
    public float maxDistance = 10f;
    private float lastSpawnTime = 0f;
    private float spawnInterval = 3f;
    public float itemDuration = 10f;

    private void Update()
    {
        if (Time.time > lastSpawnTime + spawnInterval)
        {
            Spawn();
            lastSpawnTime = Time.time;
        }
    }

    private void Spawn()
    {
        var randomPos = Random.insideUnitSphere * maxDistance;
        var item = items[Random.Range(0, items.Length)];

        if (!NavMesh.SamplePosition(randomPos, out NavMeshHit hit, maxDistance, NavMesh.AllAreas))
        {
            return;
        }
        randomPos = hit.position;
        randomPos.y += 0.5f;

        var spawnItem = Instantiate(item, randomPos, Quaternion.identity);
        Destroy(spawnItem, itemDuration);
    }
}
