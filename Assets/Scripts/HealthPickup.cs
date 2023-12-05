using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HealthPickup : NetworkBehaviour
{
    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.GetComponent<Player>())
        {
            this.GetComponent<NetworkObject>().Despawn();
        }
    }
}
