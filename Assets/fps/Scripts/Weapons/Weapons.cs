using UnityEngine;

public class Weapons : MonoBehaviour
{
    // Weapons data
    [SerializeField] public GunData gunData;
    [SerializeField] public Player_Controller playerController;
    [SerializeField] public Camera Playercamera;
    [SerializeField] public Animator animator;
    [SerializeField] public RuntimeAnimatorController playerAnimationtype;
    [SerializeField] public Transform WeaponLeftHandpos;
    [SerializeField] public Transform WeaponRightHandpos;


    public float AttackRateCounter = 0;

    public void Awake()
    {
        playerController = transform.root.GetComponent<Player_Controller>();        
        Playercamera = playerController.GetComponentInChildren<Camera>();
        WeaponLeftHandpos = gameObject.transform.Find("WeaponLeft_Hand");
        WeaponRightHandpos = gameObject.transform.Find("WeaponRight_Hand");
        animator = transform.GetComponent<Animator>();
       

    }

    void Start()
    {
        
    }


    void Update()
    {
        
    }
}
