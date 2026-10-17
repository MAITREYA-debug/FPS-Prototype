using UnityEngine;

public static class Sounds 
{
   public static void  makeSound(Sound sound)
    {
        Collider[] col = Physics.OverlapSphere(sound.pos, sound.range);

        for (int i = 0; i < col.Length; i++)
        {
            if (col[i].TryGetComponent(out Ihear hearer))
            {
                hearer.RespoundToSound(sound);
            }
        }
    }

}
