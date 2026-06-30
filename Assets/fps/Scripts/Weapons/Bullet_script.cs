using UnityEngine;

public class Bullet_script : MonoBehaviour
{
    public float lifeTime = 1;

    private void Awake()
    {        
        Destroy(gameObject, lifeTime);
    }
    
}
