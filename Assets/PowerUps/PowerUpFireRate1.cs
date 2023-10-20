using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpFireRate1 : BasePowerUp
{
    public float timeBetweenBullets = .1f;
    protected override bool ApplyToPlayer(Player thePickerUpper)
    {
        if(thePickerUpper.bulletSpawner.timeBetweenBullets <= timeBetweenBullets)
        {
            return false;
        }
        else
        {
            thePickerUpper.bulletSpawner.timeBetweenBullets = timeBetweenBullets;
            return true;
        }
        
    }
}
