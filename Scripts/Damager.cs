using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damager : MonoBehaviour
{
    public float damage = 1;
    private void OnTriggerEnter(Collider other)
    {
        PlayerCharacter player = other.GetComponent<PlayerCharacter>();
        if (player != null)
        {
            player.Hurt(damage);
        }
    }
}
