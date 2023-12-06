using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PowerUpScore : BasePowerUp
{
    protected override bool ApplyToPlayer(Player thePickerUpper)
    {
        NetworkManager.Singleton.ConnectedClients[thePickerUpper.GetComponent<NetworkObject>().OwnerClientId].PlayerObject.GetComponent<NetwrokPlayerData>().score.Value += 10;
        return true;
    }
}
