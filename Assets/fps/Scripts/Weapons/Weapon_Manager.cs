using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor.Experimental.GraphView;
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

    public static Weapon_Manager instance { get; private set; }

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


    public enum WeaponType
    {
        primary = 1, 
        secondary, 
        melee
    }

    private void OnEnable()
    {
        InputManager.OnWeaponSelected += HandleWeaponSelected;
        
        //InputManager.onDrop += Drop_Weapon;
    }

    

    private void OnDisable()
    {
        InputManager.OnWeaponSelected -= HandleWeaponSelected;
        
        //InputManager.onDrop -= Drop_Weapon;
    }

   

    private void Awake()
    {
        

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
    private void HandleWeaponSelected(int weaponId)
    {


        if ((int)currentWeaponType == weaponId)
        {
            return;
        }
        currentWeaponType = (WeaponType)weaponId;
    }

    public void PickUp_Weapon(Gun weapon)
    {
       currentWeaponType =  CheckWeaponType(weapon.gunData.gunId);

    }

    public void Drop_Weapon(Gun dropWeapon)
    {
       var obj = Instantiate(dropWeapon.gameObject, playercontroller.transform.position + playercontroller.transform.forward * 2, Quaternion.identity);

    }


    public WeaponType CheckWeaponType(int id)
    {
        WeaponType temp;

        if(id >100 && id <200) // primary
        {
            temp = WeaponType.primary;
            primaryId = id;
        }
        else if (id <100 && id > 10)// secondary
        {
            temp = WeaponType.secondary;
            secondaryId = id;
        }
        else // melee
        {
            temp = WeaponType.melee;
            meleeId = id;
        }

        return temp;

    }

}
