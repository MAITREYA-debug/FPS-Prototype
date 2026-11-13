using UnityEngine;

public class Pistol : Gun {



    #region - onEnable/onDisable -
    public void OnEnable() {
            base.OnEnable();
        InputManager.OnReload += tryReload;
        InputManager.OnFire += Firepress;
    }



    void OnDisable() {
        InputManager.OnReload -= tryReload;
        InputManager.OnFire -= Firepress;
    }
    #endregion


    void Start() {
        currentBullets = gunData.magazine;
    }

    private void Firepress() {
        Debug.Log("fire ");
        if(currentBullets <= 0) {
            return;
        }

        if(AttackRateCounter < gunData.FireRate) {
            return;
        }
        if(isReloading) return;

        shoot();
    }



    void Update() {
        tryShoot();

    }


    public override void tryShoot() {
        AttackRateCounter += Time.deltaTime;

        if(!InputManager.instance.isFiring) {
            bulletfiredcounter = 0;
            tempRecoilX = 0;
        }
        //base.tryShoot();
    }






    public override void shoot() {
        --currentBullets;
        UpdateAmmoDisplay();
        HandleBulletRecoil();


        AttackRateCounter = 0;

        bool applySpread = playerController != null && playerController.weaponAnimation_Speed > 0.1f;
        Ray ray = BuildAimRay(applySpread);

        Bullet_script bullet = Instantiate(bulletPrefab, muzzleFlashPosition.position, Quaternion.identity)
           .GetComponent<Bullet_script>();

        callFireEffects();


        if(Physics.Raycast(ray, out RaycastHit hit, gunData.MaxTravelDistance, gunData.shootlayer)) {
            Debug.Log("pistol hit");
            if(hitImpact != null) {
                var hitEffect = Instantiate(hitImpact, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(hitEffect, 10f);
            }

            bullet.Initialize(hit.point);
        }
        else {
            bullet.Initialize(ray.origin + ray.direction * gunData.MaxTravelDistance);
        }

    }
}
