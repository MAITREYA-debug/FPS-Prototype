using System.Collections;
using UnityEngine;

public class Bullet_script : MonoBehaviour
{
    [SerializeField] float lifeTime = 1f;
    [SerializeField] float speed = 200f;
    [SerializeField] float arrivalThreshold = 0.05f;

    TrailRenderer trailRenderer;

    void Awake()
    {
        trailRenderer = GetComponent<TrailRenderer>();
    }

    public void Initialize(Vector3 hitPoint)
    {
        trailRenderer?.Clear();
        StopAllCoroutines();
        StartCoroutine(BulletMove(hitPoint));
    }

    IEnumerator BulletMove(Vector3 target)
    {
        float elapsed = 0f;

        while (Vector3.Distance(transform.position, target) > arrivalThreshold)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            elapsed += Time.deltaTime;

            if (elapsed >= lifeTime)
            {
                break;
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}
