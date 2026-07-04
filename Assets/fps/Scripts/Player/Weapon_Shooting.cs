using TMPro;
using UnityEngine;
using static System.Net.WebRequestMethods;

public class Weapon_Shooting : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] Player_Controller playerController;
    [SerializeField] Camera camera;
    [SerializeField] Animator animator;
    [SerializeField] TextMeshProUGUI AmmoTxt;

    [Header("fireing")]
    [SerializeField] GameObject bulletPrefab;    
    [SerializeField] GameObject muzzleFlash;
    [SerializeField] GameObject HitVisual;

    [SerializeField] Transform muzzleFlashSpawnPoint;
    [SerializeField] LayerMask shootLayer;
    

    [SerializeField] float fireRate;
    [SerializeField] float errorRange = 0.5f;

    [SerializeField] int magzineSize = 9;
    [SerializeField] int CurrentBullets;



    float currentFireRate;
    bool isFiring;
    bool isFireErrorOn;
    Vector3 firingErrorOffset;
    void Start()
    {
        CurrentBullets = 9 ;
    }

   
    void Update()
    {
        Shoot();

    }

    private void OnEnable()
    {
        InputManager.OnReload += Reload;
    }

   

    private void OnDisable()
    {
        InputManager.OnReload -= Reload;
    }

    #region - shooting -
    void Shoot()
    {
        isFiring = InputManager.instance.isFiring;
        isFireErrorOn = (playerController.weaponAnimation_Speed > 0.1f ? true : false);
        if (isFiring & CurrentBullets > 0)
        {
                
            if (currentFireRate > fireRate)
            {

                animator.SetTrigger("fire");
                currentFireRate = 0;
                CurrentBullets--;
                AmmoTxt.SetText("" + CurrentBullets);

                if (isFireErrorOn) firingErrorOffset = new Vector3(Random.Range(-errorRange, errorRange), Random.Range(-errorRange, errorRange), 0);
                else firingErrorOffset = Vector3.zero;


                // randomize ray direction so it can be in error if is firingErroron
                var origin = camera.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0) + firingErrorOffset);
                var dir = transform.forward;
                RaycastHit hit;

                var flash = Instantiate(muzzleFlash, muzzleFlashSpawnPoint.position, muzzleFlashSpawnPoint.rotation);
                flash.transform.SetParent(muzzleFlashSpawnPoint.transform);
                Destroy(flash, 1f);

                

                Bullet_script bullet = Instantiate(bulletPrefab, muzzleFlashSpawnPoint.position, Quaternion.identity).GetComponent<Bullet_script>();
                if (Physics.Raycast(origin, out hit, 50f, shootLayer))
                {
                    var hiteffect = Instantiate(HitVisual, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(hiteffect, 10f);
                    
                    Debug.Log(hit.point);
                    bullet.Initialize(hit.point);


                }
                else
                {
                    bullet.Initialize(origin.origin + origin.direction * 100f);
                }

            }
        }
        currentFireRate += Time.deltaTime;
    }
    #endregion


    void Reload()
    {
        Debug.Log("reload called");
        CurrentBullets = magzineSize;
        AmmoTxt.SetText("" + CurrentBullets);
    }

}
