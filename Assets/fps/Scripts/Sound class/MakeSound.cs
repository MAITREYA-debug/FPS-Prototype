using UnityEngine;

public class MakeSound : MonoBehaviour
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
            if (source.isPlaying) return ;

            source.Play();


            Sound sound = new Sound(transform.position, range, Sound.SoundType.Interesting);
            Sounds.makeSound(sound);
            
        }
    }
}
