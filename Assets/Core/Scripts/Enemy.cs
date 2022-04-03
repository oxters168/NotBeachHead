using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 0.1f;
    private EnemyGenerator origin;

    void Update()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("EnemyTarget");
        float minSqrMag = float.MaxValue;
        GameObject closestTarget = null;
        for (int i = 0; i < targets.Length; i++)
        {
            //Change to A* distance eventually
            float currentSqrMag = Vector3.SqrMagnitude(targets[i].transform.position - transform.position);
            if (currentSqrMag < minSqrMag)
            {
                minSqrMag = currentSqrMag;
                closestTarget = targets[i];
            }
        }

        float distOffset = speed * Time.deltaTime;
        Vector3 moveDir = (closestTarget.transform.position - transform.position).normalized;
        transform.position += moveDir * distOffset;
    }

    public void Hit()
    {
        origin.Return(this);
    }

    public void SetOrigin(EnemyGenerator generator)
    {
        origin = generator;
    }
}
