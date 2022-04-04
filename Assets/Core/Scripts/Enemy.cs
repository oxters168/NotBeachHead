using UnityEngine;

public class Enemy : MonoBehaviour
{
    private EnemyGenerator origin;
    public float speed = 0.1f;
    public float nextWaypointDistance = 3;
    private int waypointIndex = 0;
    private GameObject target;
    private Pathfinding.Path path;

    void OnEnable()
    {
        target = FindNearestTarget();
        var seeker = GetComponent<Pathfinding.Seeker>();
        seeker.StartPath(transform.position, target.transform.position, OnPathCalculated);
    }
    void Update()
    {
        if (path != null)
        {
            if (waypointIndex < path.vectorPath.Count)
            {
                Vector3 nextWaypoint = path.vectorPath[waypointIndex];
                float distanceToWaypoint = Vector3.Distance(transform.position, nextWaypoint);
                while (distanceToWaypoint < nextWaypointDistance)
                {
                    waypointIndex++;
                    if (waypointIndex >= path.vectorPath.Count)
                        break;
                    nextWaypoint = path.vectorPath[waypointIndex];
                    distanceToWaypoint = Vector3.Distance(transform.position, nextWaypoint);
                }

                if (waypointIndex < path.vectorPath.Count)
                {
                    float distOffset = speed * Time.deltaTime;
                    Vector3 moveDir = (nextWaypoint - transform.position).normalized;
                    transform.position += moveDir * distOffset;
                }
            }
        }
    }

    public void OnPathCalculated(Pathfinding.Path p) {
        if (!p.error)
        {
            waypointIndex = 0;
            path = p;
        }
        else
            Debug.LogError(p.errorLog);
    }

    private GameObject FindNearestTarget()
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
        return closestTarget;
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
