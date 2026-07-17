using UnityEngine;

public class Melee : Gun
{

  

    bool firePressed = false;

    #region - onEnable/onDisable -
    void OnEnable()
    {
        InputManager.OnReload += tryReload;
        InputManager.OnFire += Firepress;
    }


    void OnDisable()
    {
        InputManager.OnReload -= tryReload;
        InputManager.OnFire -= Firepress;
    }
    #endregion


    void Start()
    {
        
    }

    private void Firepress()
    {
        Debug.Log("fire ");
        if (!InputManager.instance.isFiring || currentBullets <= 0)
        {
            return;
        }

        if (fireDelayCounter < gunData.FireRate)
        {
            return;
        }

        if (isReloading) return;

        shoot();
    }

    

    void Update()
    {
        tryShoot();
    }

    
    public override void tryShoot()
    {
        fireDelayCounter += Time.deltaTime;        
    }



    public override void shoot()
    {
        --currentBullets;
        fireDelayCounter = 0;

        
        Ray ray = BuildAimRay(false);        


        if (Physics.Raycast(ray, out RaycastHit hit, gunData.MaxTravelDistance, gunData.shootlayer))
        {
            if (hitImpact != null)
            {
                var hitEffect = Instantiate(hitImpact, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(hitEffect, 10f);
            }            
        }
       

        
    }
}
