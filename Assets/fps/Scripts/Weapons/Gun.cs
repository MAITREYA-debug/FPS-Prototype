using System.Collections;
using TMPro;
//using UnityEditor.Animations;
using UnityEngine;

public abstract class Gun : Weapons
{
   

    [SerializeField] TextMeshProUGUI AmmoTxt;


    [Header("Firing")]
    [SerializeField] public GameObject bulletPrefab;
    [SerializeField] public GameObject muzzleFlash;
    [SerializeField] public GameObject hitImpact;
    [SerializeField] public Transform muzzleFlashPosition;
    [SerializeField] public Transform ScopePosition;



    [Header("Current gun")]
    public int currentBullets = 0;
    
    public bool isReloading = false;
    

    public void Awake()
    {
        base.Awake();

        currentBullets = gunData.magazine;
        muzzleFlashPosition = this.transform.Find("muzzleFlashPosition");



        if (muzzleFlashPosition == null)
        {
            Debug.LogError("MuzzleFlashPosition not found in " + this.name);
        }      

        ScopePosition = transform.Find("InSight").GetComponent<Transform>();
    }

      

    virtual
    public void tryShoot()
    {
        AttackRateCounter += Time.deltaTime;

        if (!InputManager.instance.isFiring || currentBullets <= 0) return;

        if (AttackRateCounter < gunData.FireRate) return;
      
        if (isReloading) return;

        shoot();

    }

    

    public abstract void shoot();

    //public abstract void shootSound();


    public Ray BuildAimRay(bool applySpread)
    {
        float spreadX = 0f;
        float spreadY = 0f;

        if (applySpread && gunData.ErrorRange > 0f)
        {
            spreadX = (Random.value - 0.5f) * 2f * gunData.ErrorRange / Screen.width;
            spreadY = (Random.value - 0.5f) * 2f * gunData.ErrorRange / Screen.height;
        }

        return Playercamera.ViewportPointToRay(new Vector3(0.5f + spreadX, 0.5f + spreadY, 0f));
    }


    public void tryReload()
    {
        if (currentBullets < gunData.magazine)
        {
            StartCoroutine(Reloading());
        }
    }
    

    public IEnumerator Reloading()
    {

        isReloading = true;

        yield return new WaitForSeconds(gunData.reloadTime);
        currentBullets = gunData.magazine;
        isReloading = false;
        PlayerSoundManager.instance.Play(Sound.SoundType.Reload);
        UpdateAmmoDisplay();

        Debug.Log("Gun is reloaded");

    }

    public void callFireAnimation()
    {

        if (animator != null)
        {
            animator.SetTrigger("fire");
        }

        var flash = Instantiate(muzzleFlash, muzzleFlashPosition.position, muzzleFlashPosition.rotation, muzzleFlashPosition);
        Destroy(flash, 1f);
        PlayerSoundManager.instance.Play(Sound.SoundType.WeaponBullets);
    }

    public void callReloadAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Reload");
        }       
    }

    public void UpdateAmmoDisplay()
    {
        if (AmmoTxt != null)
        {
            AmmoTxt.SetText(currentBullets.ToString());
        }
    }
}
