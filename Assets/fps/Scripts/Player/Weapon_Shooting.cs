using TMPro;
using UnityEngine;

public class Weapon_Shooting : MonoBehaviour

{
    [Header("Reference")]
    [SerializeField] Player_Controller playerController;
    [SerializeField] Camera camera;
    [SerializeField] Animator animator;
    [SerializeField] TextMeshProUGUI AmmoTxt;

    [Header("Firing")]
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] GameObject muzzleFlash;
    [SerializeField] GameObject hitImpact;

    [SerializeField] Transform muzzleFlashSpawnPoint;
    [SerializeField] LayerMask shootLayer;

    [SerializeField] float fireRate = 0.2f;
    [SerializeField] float errorRange = 0.5f;
    [SerializeField] float maxShootDistance = 100f;

    [SerializeField] int magzineSize = 9;
    [SerializeField] int CurrentBullets;

    float currentFireRate;
    bool isReloading;

    void Awake()
    {
        CurrentBullets = magzineSize;
        UpdateAmmoDisplay();
    }

    void Update()
    {
        if (InputManager.instance == null || isReloading)
        {
            return;
        }

        Shoot();
        currentFireRate += Time.deltaTime;
    }

    #region - onEnable/onDisable -
    void OnEnable()
    {
        InputManager.OnReload += Reload;
    }

    void OnDisable()
    {
        InputManager.OnReload -= Reload;
    }
    #endregion

    #region - shoot -
    void Shoot()
    {
        if (!InputManager.instance.isFiring || CurrentBullets <= 0)
        {
            return;
        }

        if (currentFireRate < fireRate)
        {
            return;
        }

        if (bulletPrefab == null || muzzleFlashSpawnPoint == null || camera == null)
        {
            return;
        }

        currentFireRate = 0f;
        CurrentBullets--;
        UpdateAmmoDisplay();

        bool applySpread = playerController != null && playerController.weaponAnimation_Speed > 0.1f;
        Ray ray = BuildAimRay(applySpread);

        animator?.SetTrigger("fire");

        if (muzzleFlash != null)
        {
            var flash = Instantiate(muzzleFlash, muzzleFlashSpawnPoint.position, muzzleFlashSpawnPoint.rotation, muzzleFlashSpawnPoint);
            Destroy(flash, 1f);
        }

        Bullet_script bullet = Instantiate(bulletPrefab, muzzleFlashSpawnPoint.position, Quaternion.identity)
            .GetComponent<Bullet_script>();        

        if (Physics.Raycast(ray, out RaycastHit hit, maxShootDistance, shootLayer))
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
            bullet.Initialize(ray.origin + ray.direction * maxShootDistance);
        }
    }
    #endregion

    #region - build Aim Ray -
    Ray BuildAimRay(bool applySpread)
    {
        float spreadX = 0f;
        float spreadY = 0f;

        if (applySpread && errorRange > 0f)
        {
            spreadX = (Random.value - 0.5f) * 2f * errorRange / Screen.width;
            spreadY = (Random.value - 0.5f) * 2f * errorRange / Screen.height;
        }

        return camera.ViewportPointToRay(new Vector3(0.5f + spreadX, 0.5f + spreadY, 0f));
    }
    #endregion

    #region - Ammo reload -

    void Reload()
    {
        if (CurrentBullets >= magzineSize)
        {
            return;
        }

        isReloading = true;
        CurrentBullets = magzineSize;
        UpdateAmmoDisplay();
        isReloading = false;
    }
     

    void UpdateAmmoDisplay()
    {
        if (AmmoTxt != null)
        {
            AmmoTxt.SetText(CurrentBullets.ToString());
        }
    }

    #endregion
}
