using System.Collections.Generic;
using UnityEngine;

public class Weapon_Manager : MonoBehaviour
{

    /* ++++++++     weapon manager ++++++    
        holds all the Weapons object
        Weapon State primary- smg, short gun , rifle secondary - short gun  melee 
        places current weapon on the pivot position 
        handles input keys as per the user input current Guns 
        Know which gun can be removed and which cannot 
        Controlls animation here
    */

    public static Weapon_Manager instance { get; private set; }

    [Header("Reference")]
    [SerializeField] Player_Controller playercontroller;
    [SerializeField] Weapon_Controller weaponContoller;
    //[SerializeField] Weapon_Shooting weaponShooting;
    //[SerializeField] Animator WeaponAnimation;
    //[SerializeField] Animator playerAnimation;



    [SerializeField] WeaponType currentWeaponType;
    [SerializeField] List<GameObject> Use_WeaponPool = new List<GameObject>();
    [SerializeField] Dictionary<int, Gun> holdWeapons = new Dictionary<int, Gun>();

    [SerializeField] List<GameObject> Throw_WeaponPool = new List<GameObject>();
    [SerializeField] Dictionary<int, PickUpGun_Script> throwWeapons = new Dictionary<int, PickUpGun_Script>();



    [SerializeField] int currentWeaponId = 0;
    [SerializeField] int selectedWeaponId = 0;

    [SerializeField] int primaryId = 1;
    [SerializeField] int secondaryId = 101;
    [SerializeField] int meleeId = 102;

    [SerializeField] Gun currentHoldWeapon_Obj;

    Transform WeaponPosition;

    bool canDrop = false;

    public enum WeaponType
    {
        primary = 1,
        secondary,
        melee
    }

    #region - onEnable/ onDisable -

    private void OnEnable()
    {
        InputManager.OnWeaponSelected += WeaponTypeSelection;

        InputManager.onDrop += try_DropWeapon;
    }



    private void OnDisable()
    {
        InputManager.OnWeaponSelected -= WeaponTypeSelection;

        InputManager.onDrop -= try_DropWeapon;
    }

    #endregion

    private void Awake()
    {
        instance = this;

        selectedWeaponId = primaryId;
        currentWeaponId = selectedWeaponId;


        foreach (var weapon in Use_WeaponPool)
        {
            var gun = weapon.GetComponent<Gun>();
            if (gun != null)
            {
                holdWeapons.Add(gun.gunData.gunId, gun);

                if (gun.gunData.gunId == currentWeaponId)
                {
                    currentHoldWeapon_Obj = gun;
                    currentHoldWeapon_Obj.gameObject.SetActive(true);
                }
                else
                {
                    gun.gameObject.SetActive(false);
                }
            }
        }

        foreach (var weapon in Throw_WeaponPool)
        {
            var gun = weapon.GetComponent<PickUpGun_Script>();
            if (gun != null)
            {
                throwWeapons.Add(gun.gunData.gunId, gun);
            }
        }

        act_Deactivate_Weapon();

    }


    private void Update()
    {
        
    }
#region - Handle Weapon Selection input -

    private void WeaponTypeSelection(int weaponType_no)
    {
        if (weaponType_no == 1 && primaryId == 0 || weaponType_no == 2 && secondaryId == 0) return;
        if (currentWeaponType == (WeaponType)weaponType_no) return;       

        currentWeaponType = (WeaponType)weaponType_no;
        SetHoldWeapon_ID();
        act_Deactivate_Weapon();
    }

#endregion

#region - set Hold Weapon Type -
    void SetHoldWeapon_ID()
    {
        Debug.Log("Setting Hold Weapon Type " + currentWeaponType);

        switch (currentWeaponType)
        {
            case WeaponType.primary:
                selectedWeaponId = primaryId;
                break;
            case WeaponType.secondary:
                selectedWeaponId = secondaryId;
                break;
            case WeaponType.melee:
                selectedWeaponId = meleeId;
                break;
            default:
                break;
        }
    }
#endregion

#region - activate Selected Weapon -

    void act_Deactivate_Weapon()
    {

        Debug.Log("Weapon Activate deactivate ");
        if (selectedWeaponId == 0)
        {
            selectedWeaponId = currentWeaponId;
            return;
        }

        currentWeaponId = selectedWeaponId;
        currentHoldWeapon_Obj.gameObject.SetActive(false);

        currentHoldWeapon_Obj = holdWeapons.TryGetValue(selectedWeaponId, out Gun gun) ? gun : currentHoldWeapon_Obj;

        currentHoldWeapon_Obj.gameObject.SetActive(true);
        //currentWeaponId = selectedWeaponId;
    }

    #endregion   

#region - picked Weapon set -
    public void PickUp_Weapon(PickUpGun_Script Pickedweapon)
    {
        Debug.Log("Weapon Picked Up in WeaponManager ");
        setOrNull_AnyWeapon(Pickedweapon.gunData.gunId);
        currentHoldWeapon_Obj.currentBullets = Pickedweapon.getBulletCount();

    }
    #endregion

#region - Drop Weapon -  
    public void try_DropWeapon()
    {

        if (currentWeaponType == WeaponType.melee)
        {
            Debug.Log("Cannot drop melee weapon");
            return;
        }

        dropWeapon(currentHoldWeapon_Obj.gunData.gunId);
      
    }

    public void dropWeapon(int id)
    {
        if (throwWeapons.TryGetValue(id, out PickUpGun_Script pickupGun))
        {
            Debug.Log("Dropping Weapon " + pickupGun.gunData.gunId);

            // Weapon is Created in the Scene
            var obj = Instantiate(pickupGun.gameObject, playercontroller.transform.position + playercontroller.transform.forward, Quaternion.identity);

            // Scipt Reference for calling and Getting the gun data
            var throwWeapon_Sci = obj.GetComponent<PickUpGun_Script>();

            // gets the object that will be droped weapon To Drop
            var temp = holdWeapons.TryGetValue(id, out Gun gun) ? gun : currentHoldWeapon_Obj;

            throwWeapon_Sci.saveGunData(temp.gunData, temp.currentBullets);
            throwWeapon_Sci.throwWeapon(playercontroller.transform);

            // changing hold Weapon After Thrown
            setOrNull_AnyWeapon(id, true);

        }
    }

    #endregion

#region - set value OR set null Weapons -

    void setOrNull_AnyWeapon(int WeaponId , bool SetNull = false)
    {

        // what does it do 
        // checks the Type of the Id
        // sets it in the Position of the primary secondary Hold Values
        // if SetNull is true then it will set the value to 0
        // if position are not 0 and setNull = false the DropWeapon method is run to drop that weapon and select new one;

        if (SetNull)
        {
            if (100 < WeaponId && WeaponId < 200)
            {
                primaryId = 0;
               
            }
            else if (0 < WeaponId && WeaponId < 10)
            {
                secondaryId = 0;
               
            }
            currentWeaponType = WeaponType.melee;
        }
        else
        {
            if (100 < WeaponId && WeaponId < 200)
            {
                if (primaryId != 0) dropWeapon(primaryId);                
                primaryId = WeaponId;
                currentWeaponType = WeaponType.primary;
            }
            else if (0 < WeaponId && WeaponId < 10)
            {
                if (secondaryId != 0) dropWeapon(secondaryId);
                secondaryId = WeaponId;
                currentWeaponType = WeaponType.secondary;
            }
        }

        SetHoldWeapon_ID();
        act_Deactivate_Weapon();

    }

    #endregion


}
