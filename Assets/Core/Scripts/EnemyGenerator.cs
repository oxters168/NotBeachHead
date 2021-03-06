using UnityEngine;
using UnityHelpers;

public class EnemyGenerator : MonoBehaviour
{
    public Enemy enemyPrefab;
    public float secondsPerSpawn = 1;
    public int maxEnemies = 5;
    public Vector3 randomSpawnVolume = Vector3.zero;

    private ObjectPool<Enemy> enemiesPool;
    private float prevSpawnTime = float.MinValue;

    void Start()
    {
        enemiesPool = new ObjectPool<Enemy>(enemyPrefab, 5, false, true, transform);
    }
    void Update()
    {
        if (Time.time - prevSpawnTime >= secondsPerSpawn)
        {
            prevSpawnTime = Time.time;

            if (enemiesPool.activeCount < maxEnemies)
            {
                enemiesPool.Get(enemy =>
                {
                    enemy.SetOrigin(this);
                    Vector3 spawnExtents = randomSpawnVolume / 2;
                    Vector3 posOffset = new Vector3(Random.Range(-1f, 1f) * spawnExtents.x, Random.Range(-1f, 1f) * spawnExtents.y, Random.Range(-1f, 1f) * spawnExtents.z);
                    enemy.transform.position = transform.position + posOffset;
                    enemy.transform.rotation = transform.rotation;
                });
            }
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (Mathf.Approximately(randomSpawnVolume.x, 0) && Mathf.Approximately(randomSpawnVolume.y, 0) && Mathf.Approximately(randomSpawnVolume.z, 0))
            Gizmos.DrawSphere(transform.position, 0.06f);
        else
            Gizmos.DrawWireCube(transform.position, randomSpawnVolume);
    }

    public void Return(Enemy child)
    {
        enemiesPool.Return(child);
    }
    public void ReturnAll()
    {
        enemiesPool.ReturnAll();
    }
}
