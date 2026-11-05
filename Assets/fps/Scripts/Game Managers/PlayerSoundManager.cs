using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundManager : MonoBehaviour
{
    public static PlayerSoundManager instance { get; private set; }

    [System.Serializable]
    public class SoundProfile
    {
        
        public Sound.SoundType type; 
        public AudioClip[] clips; // all the clips for this sound type

        [Range(0f, 1f)]
        public float volume = 1f;

        public float range = 20f;
        public bool notifyAI = false;
        public bool Randompitch  = false;
        public bool RandomClipsPlay = false;

        [HideInInspector]
        public int lastClipIndex = -1;
    }

    [Header("Audio")]
    [SerializeField] private AudioSource source;
    [SerializeField] private SoundProfile[] soundProfiles;

    private Dictionary<Sound.SoundType, SoundProfile> soundLookup;



    private void Awake()
    {
        // data structure to store sounds profile for each sound type, for quick lookup
        soundLookup = new Dictionary<Sound.SoundType, SoundProfile>(
            soundProfiles.Length
        );

        // stores all the sound in soundlookup data structure 
        foreach (SoundProfile profile in soundProfiles)
        {
            if (profile == null) // checks if their is no profile
                continue;

            if (soundLookup.ContainsKey(profile.type)) // dupicate profile check
            {
                Debug.LogWarning(
                    $"Duplicate sound profile: {profile.type}", this);
                continue;
            }

            soundLookup.Add(profile.type, profile); // adds profiles with type as key and profile as values
        }

        if (source == null) // source is the component that plays the sound
            source = GetComponent<AudioSource>();
    }

    public void Play(Sound.SoundType type)
    {
        if (source == null)
            return;

        if (!soundLookup.TryGetValue(type, out SoundProfile profile)) // checks for type in soundlookup if present out profile
            return;

        AudioClip[] clips = profile.clips; // makes a list of profile clips for playing

        if (clips == null || clips.Length == 0) // checks if clips is not empty or null
            return;

        int index = GetRandomClipIndex(profile, clips.Length); // gets a random index out of all the clips length
        AudioClip clip = clips[index];

        if (clip == null)
            return;

        // Play without interrupting sounds already playing.
        source.PlayOneShot(clip, profile.volume);

        // Notify the AI only for sounds configured to be detectable.
        if (profile.notifyAI)
        {
            Sound sound = new Sound(
                transform.position,
                profile.range,
                type
            );

            Sounds.makeSound(sound);
        }
    }

    private int GetRandomClipIndex(SoundProfile profile, int count)
    {
        if (count <= 1)
            return 0;

        int index = Random.Range(0, count);

        // Prevent the same clip from playing twice consecutively.
        if (index == profile.lastClipIndex)
            index = (index + Random.Range(1, count)) % count;

        profile.lastClipIndex = index;
        return index;
    }
}