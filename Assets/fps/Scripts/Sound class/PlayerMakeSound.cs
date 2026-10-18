using UnityEngine;

public class PlayerMakeSound : MonoBehaviour
{
    [SerializeField] private AudioSource source;
    [SerializeField] private float range = 20f;


    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (source.isPlaying) return;

            source.Play();

            Sound sound = new Sound(transform.position, range, Sound.SoundType.Footsteps);
            Sounds.makeSound(sound);

        }
    }
}
