using UnityEngine;

public class PickUpGun_Script : MonoBehaviour
{

    [SerializeField] Rigidbody rb;
    [SerializeField] public GunData gunData;    
    [SerializeField] float throwForce = 10f;

    public int bulletCount;


    void Awake()
    {
        rb = this.gameObject.GetComponent<Rigidbody>();

        if(rb == null)
        {
           Debug.LogError("Rigidbody component not found on the GameObject.");
        }

        bulletCount = gunData.magazine;
       
    }

  
    void Update()
    {
               
    }
    

    public GunData getGunData()
    {
       return gunData;
    }

    public int getBulletCount()
    {
        return bulletCount;
    }   

    public void saveGunData(GunData gundata, int bulletCount)
    {
        this.gunData = gundata;
        this.bulletCount = bulletCount;
    }



    public void throwWeapon(Transform PlayerPos)
    {
        if (rb != null)
        {
            // Reset any existing motion
            rb.linearVelocity = Vector3.zero;   // Use velocity if you're on older Unity versions
            rb.angularVelocity = Vector3.zero;

            // Throw forward with a slight upward arc
            Vector3 throwDirection = (PlayerPos.transform.forward + Vector3.up * 0.2f).normalized;
            rb.AddForce(throwDirection * throwForce, ForceMode.Impulse);

            // Add spin
            rb.AddTorque(
                PlayerPos.transform.right * 8f +
                PlayerPos.transform.up * 4f,
                ForceMode.Impulse
            );

        }
    }


}
