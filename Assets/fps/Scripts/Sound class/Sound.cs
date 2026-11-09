using UnityEngine;

public class Sound 
{

    public enum SoundType
    {
        Default = -1,
        WeaponBullets,
        Footsteps,
        Running,
        Jump,
        Landing,
        WeaponPickUp,
        WeaponSwitch,
        Reload,
        Melee

    }


    public readonly Vector3 pos;
    public readonly float range;
    public readonly SoundType soundType;

    /// <summary>
    /// This the intensity of the sound.
    /// </summary>


    public Sound(Vector3 _pos, float _range, SoundType type)
    {
        pos = _pos;
        range = _range;
        soundType = type;
    }



}
