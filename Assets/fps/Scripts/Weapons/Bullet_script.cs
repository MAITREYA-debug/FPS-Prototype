using System.Collections;
using UnityEngine;

public class Bullet_script : MonoBehaviour
{
    public float lifeTime = 1;
    [SerializeField] float speed = 200f;



    private void Awake()
    {    
       
    }

    public void Initialize(Vector3 hitPoint)
    {
        transform.GetComponent<TrailRenderer>().Clear();
        StartCoroutine(BulletMove(hitPoint));
    }


    IEnumerator BulletMove(Vector3 target)
    {
        yield return null;

        while (Vector3.Distance(transform.position, target) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            Debug.Log("corutine for bullet");
            yield return null;
        }
        Destroy(gameObject,lifeTime);


    }
    
}
