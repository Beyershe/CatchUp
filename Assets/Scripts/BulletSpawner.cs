using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BulletSpawner : NetworkBehaviour
{
    [SerializeField]
    private GameObject bullet;
    [SerializeField]
    private GameObject bulletSpawnLoc;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0) && IsOwner)
        {
            SpawnBulletServerRPC(bulletSpawnLoc.transform.position, bulletSpawnLoc.transform.rotation, GetComponent<NetworkObject>().OwnerClientId);
        }
    }

    [ServerRpc]
    public void SpawnBulletServerRPC(Vector3 pos, Quaternion rot, ulong ownerID)
    {
        GameObject tempBullet = Instantiate(bullet, pos, rot);
        tempBullet.GetComponent<NetworkObject>().SpawnWithOwnership(ownerID);
        Destroy(tempBullet, 3);
    }

}
