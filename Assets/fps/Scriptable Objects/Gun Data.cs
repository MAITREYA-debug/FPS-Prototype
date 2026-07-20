using UnityEngine;

[CreateAssetMenu(fileName = "GunData", menuName = "Scriptable Objects/GunData")]
public class GunData : ScriptableObject
{

    [Header("Weapon info")]
    public int gunId = 0;
    public string name = new string("");
    public LayerMask shootlayer;

    [Header("specs")]
    public int magazine = 0;
    public float reloadTime = 0;

    public float FireRate = 0;
    public float ErrorRange = 0;
    public float MaxTravelDistance = 0;
    public float swayScale = 0;
    public float moveRotationAmount = 0;
    public float lookRotationAmount = 0;
    public float smoothSpeed = 0;



}
