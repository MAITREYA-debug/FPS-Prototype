using UnityEngine;

public class Melee : Weapons {

    public void Awake() {
        base.Awake();
        WeaponRightHandpos = gameObject.transform.Find("uploads_files_4687838_KNIFE_VAR2/WeaponRight_Hand");
    }

    #region - onEnable/onDisable -
    void OnEnable() {
        InputManager.OnFire += Firepress;
    }


    void OnDisable() {
        InputManager.OnFire -= Firepress;
    }
    #endregion


    void Start() {

    }
    public void callFireAnimation() {

        if(animator != null) {
            Debug.Log("fire ");
            animator.SetTrigger("fire");
        }

        PlayerSoundManager.instance.Play(Sound.SoundType.Melee);
    }

    public void attack() {

        AttackRateCounter = 0;
        callFireAnimation();


        Ray ray = Playercamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));


        if(Physics.Raycast(ray, out RaycastHit hit, gunData.MaxTravelDistance, gunData.shootlayer)) {
            // Debug.Log("hit " + hit.collider.name);
        }

    }



    private void Firepress() {



        if(AttackRateCounter < gunData.FireRate) {
            return;
        }

        attack();
    }



    void Update() {
        AttackRateCounter += Time.deltaTime;
        if(AttackRateCounter > 100) {
            AttackRateCounter = gunData.FireRate;
        }

    }



}
