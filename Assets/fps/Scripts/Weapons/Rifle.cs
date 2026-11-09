using UnityEngine;

public class Rifle : Gun
{


    //bool firePressed = false;

    #region - onEnable/onDisable -
    void OnEnable()
    {
        InputManager.OnReload += tryReload;
       
    }


    void OnDisable()
    {
        InputManager.OnReload -= tryReload;
        
    }
    #endregion


    void Start()
    {
        
    }

    void Update()
    {
       tryShoot();
       
    }

    public override void tryShoot()
    {
        base.tryShoot();
    }


    public override void shoot()
    {

        Debug.Log("riffle fire");

        --currentBullets;
        UpdateAmmoDisplay();

        AttackRateCounter = 0;

        bool applySpread = playerController.weaponAnimation_Speed > 0.1f ? true : false;
        Ray ray = BuildAimRay(applySpread);        

        Bullet_script bullet = Instantiate(bulletPrefab, muzzleFlashPosition.position, Quaternion.identity)
           .GetComponent<Bullet_script>();
        
        callFireAnimation();
        


        if (Physics.Raycast(ray, out RaycastHit hit, gunData.MaxTravelDistance, gunData.shootlayer))
        {
            if (hitImpact != null)
            {
                var hitEffect = Instantiate(hitImpact, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(hitEffect, 10f);
            }

            bullet.Initialize(hit.point);
        }
        else
        {
            bullet.Initialize(ray.origin + ray.direction * gunData.MaxTravelDistance);
        }

        
    }
}
