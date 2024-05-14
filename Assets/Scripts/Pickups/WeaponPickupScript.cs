using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickupScript : MonoBehaviour
{
    public string theGun;
    private bool collected;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !collected)
        {
            PlayerController.instance.addGun(theGun);
            Destroy(gameObject);
            collected = true;
            AudioManager.instance.PlaySFX(4);
        }
    }
}
