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
        fireDelayCounter = 0;

        bool applySpread = playerController.weaponAnimation_Speed > 0.1f;
        Ray ray = BuildAimRay(applySpread);

        var flash = Instantiate(muzzleFlash, muzzleFlashPosition.position, muzzleFlashPosition.rotation, muzzleFlashPosition);
        Destroy(flash, 1f);

        Bullet_script bullet = Instantiate(bulletPrefab, muzzleFlashPosition.position, Quaternion.identity)
           .GetComponent<Bullet_script>();

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
