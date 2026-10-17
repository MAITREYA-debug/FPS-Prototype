using UnityEngine;

public class Sound 
{

    public enum SoundType
    {
        Default = -1,
        Interesting,
        
    }

    public readonly SoundType soundType;

    public readonly Vector3 pos;

    /// <summary>
    /// This the intensity of the sound.
    /// </summary>
    public readonly float range;

    public Sound(Vector3 _pos, float _range, SoundType type)
    {
        pos = _pos;
        range = _range;
        soundType = type;
    }



}
