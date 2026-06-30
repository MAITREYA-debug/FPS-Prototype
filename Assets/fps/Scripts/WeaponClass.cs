using UnityEngine;

public class WeaponClass : MonoBehaviour 
{

    [Header("Weapon Data")]
    [SerializeField] private int weaponId;
    [SerializeField] private string weaponName;    
    [SerializeField] private int BulletDamage;
    [SerializeField] private int fireRate;
    [SerializeField] private int maxineSize;

    [SerializeField] public Animator animator;
 
    public void idle() {
    
    }


    public void fire() {
        animator.SetTrigger("fire");
    }

    public void Reload() {
        
    }


    public void RighClick() {
    
    }

    public void Sprint() { 
    
    }

}

