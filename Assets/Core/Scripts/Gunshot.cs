using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityHelpers;

public class Gunshot : MonoBehaviour
{
    public ObjectPool<Gunshot> origin;
    public Ray shootRay;
    public float maxShotDistance;
    public float ttl = 0.1f;
    private float lifestart;

    void OnEnable()
    {
        lifestart = Time.time;
        var shootLine = GetComponentInChildren<LineRenderer>();
        shootLine.SetPositions(new Vector3[] { shootRay.origin, shootRay.origin + shootRay.direction * maxShotDistance });

        RaycastHit[] hits = Physics.RaycastAll(shootRay.origin, shootRay.direction, maxShotDistance, ~(1 << LayerMask.NameToLayer("Environment")) & ~(1 << LayerMask.NameToLayer("Obstacle")), QueryTriggerInteraction.Collide);
        for (int i = 0; i < hits.Length; i++)
        {
            hits[i].transform.root.GetComponentInChildren<Enemy>().Hit();
            // Debug.Log(hits[i].transform.root.name);
        }
    }

    void Update()
    {
        if (Time.time - lifestart >= ttl)
        {
            origin.Return(this);
            //return
        }
    }
}
