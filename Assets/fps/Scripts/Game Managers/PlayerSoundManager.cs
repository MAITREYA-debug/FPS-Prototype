using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundManager : MonoBehaviour
{
    [System.Serializable]
    public class SoundProfile
    {

        public Sound.SoundType type; 
        public AudioClip[] clips; // all the clips for this sound type

        [Range(0f, 1f)]
        public float volume = 1f;

        public float range = 20f;
        public bool notifyAI = false;

        [HideInInspector]
        public int lastClipIndex = -1;
    }

    [Header("Audio")]
    [SerializeField] private AudioSource source;
    [SerializeField] private SoundProfile[] soundProfiles;

    private Dictionary<Sound.SoundType, SoundProfile> soundLookup;



    private void Awake()
    {
        soundLookup = new Dictionary<Sound.SoundType, SoundProfile>(
            soundProfiles.Length
        );

        foreach (SoundProfile profile in soundProfiles)
        {
            if (profile == null)
                continue;

            if (soundLookup.ContainsKey(profile.type))
            {
                Debug.LogWarning(
                    $"Duplicate sound profile: {profile.type}", this);
                continue;
            }

            soundLookup.Add(profile.type, profile);
        }

        if (source == null)
            source = GetComponent<AudioSource>();
    }

    public void Play(Sound.SoundType type)
    {
        if (source == null)
            return;

        if (!soundLookup.TryGetValue(type, out SoundProfile profile))
            return;

        AudioClip[] clips = profile.clips;

        if (clips == null || clips.Length == 0)
            return;

        int index = GetRandomClipIndex(profile, clips.Length);
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