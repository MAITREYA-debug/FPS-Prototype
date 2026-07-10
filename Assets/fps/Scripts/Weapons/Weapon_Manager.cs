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

    [SerializeField] WeaponType currentWeaponType;
    [SerializeField] List<GameObject> WeaponPool = new List<GameObject>();
    [SerializeField] Dictionary<int , GameObject> weapons = new Dictionary<int , GameObject>();

    [SerializeField] int currentWeaponId = 0;
    [SerializeField] int selectedWeaponId = 0;
    Transform WeaponPosition;

  
    


    enum WeaponType
    {
        primary, secondary, melee
    }

    private void Awake()
    {
      
    }


    private void Update()
    {
        if(currentWeaponId == selectedWeaponId)
        {
            return;
        }

       

    }






}
