using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    public int ammo = 10;

    public void Pickup(out int ammoToAdd)
    {
        ammoToAdd = ammo;
        Destroy(gameObject);
    }
}
