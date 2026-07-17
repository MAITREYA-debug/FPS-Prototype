using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AdaptivePerformance;

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

    [Header("Reference")]
    [SerializeField] Player_Controller playercontroller;
    [SerializeField] Weapon_Controller weaponContoller;
    //[SerializeField] Weapon_Shooting weaponShooting;
    //[SerializeField] Animator WeaponAnimation;
    //[SerializeField] Animator playerAnimation;



    [SerializeField] WeaponType currentWeaponType;
    [SerializeField] List<GameObject> WeaponPool = new List<GameObject>();
    [SerializeField] Dictionary<int , Gun> weapons = new Dictionary<int , Gun>();

    [SerializeField] int currentWeaponId = 0;
    [SerializeField] int selectedWeaponId = 0;

    [SerializeField]  int primaryId = 1;
    [SerializeField] int secondaryId = 101;
    [SerializeField] int meleeId = 102;

    [SerializeField] Gun CurrentWeaponobj;    

    Transform WeaponPosition;


    enum WeaponType
    {
        primary = 1, 
        secondary, 
        melee
    }

    private void OnEnable()
    {
        InputManager.OnWeaponSelected += HandleWeaponSelected;
    }
    private void OnDisable()
    {
        InputManager.OnWeaponSelected -= HandleWeaponSelected;
    }

    private void HandleWeaponSelected(int weaponId)
    {
        // Handle the weapon selection logic here

        if((int)currentWeaponType == weaponId)
        {
            return;
        }
        currentWeaponType = (WeaponType)weaponId;
    }

    private void Awake()
    {
        //    playercontroller = GetComponent<Player_Controller>();
        //    weaponContoller = GetComponent<Weapon_Controller>();

        selectedWeaponId = primaryId;
        currentWeaponId = selectedWeaponId;


        foreach (var weapon in WeaponPool)
        {
            var gun = weapon.GetComponent<Gun>();
            if (gun != null)
            {
                weapons.Add(gun.gunData.gunId, gun);

                if (gun.gunData.gunId == currentWeaponId)
                {
                    CurrentWeaponobj = gun;
                    CurrentWeaponobj.gameObject.SetActive(true);
                }
                else
                {
                    gun.gameObject.SetActive(false);
                }
            }
        }
    }

    


    private void Update()
    {
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

        if (currentWeaponId != selectedWeaponId)
        { 
            SelectId();
        }
        currentWeaponId = CurrentWeaponobj.gunData.gunId;
    }

    void SelectId()
    {
        if(100 >= selectedWeaponId &&  selectedWeaponId <= 200)
        {

        }

               
            Debug.Log("Weapon Changed");            
            currentWeaponId = selectedWeaponId;

            CurrentWeaponobj.gameObject.SetActive(false);
            CurrentWeaponobj = weapons.TryGetValue(selectedWeaponId, out Gun gun) ? gun : CurrentWeaponobj;
            CurrentWeaponobj.gameObject.SetActive(true);
            currentWeaponId = selectedWeaponId;
        
    }


}
