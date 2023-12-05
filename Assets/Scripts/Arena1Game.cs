using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Arena1Game : NetworkBehaviour
{
    public Player playerPrefab;
    public Player playerHost;
    public Camera arenaCamera;
    public GameObject healthPickups;

    private NetworkedPlayers networkedPlayers;
    private int positionIndex = 0;
    private int hpPositionIndex = 0;

    private Vector3[] startPositions = new Vector3[]
    {
        new Vector3(15, 2, 0),
        new Vector3(-15, 2, 0),
        new Vector3(0, 2, 15),
        new Vector3(0, 2, -15)
    };

    private Vector3[] healthPickupPositions = new Vector3[]
    {
        new Vector3(20, 2.5f, 0),
        new Vector3(-20, 2.5f, 0),
        new Vector3(0, 2.5f, 20),
        new Vector3(0, 2.5f, -20)
    };


    private Vector3 HPPickupNextPosition()
    {
        Vector3 pos = healthPickupPositions[hpPositionIndex];
        hpPositionIndex += 1;
        if(hpPositionIndex > healthPickupPositions.Length - 1)
        {
            hpPositionIndex = 0;
        }
        return pos;
    }


    void Start()
    {
        arenaCamera.enabled = !IsClient;
        arenaCamera.GetComponent<AudioListener>().enabled = !IsClient;
        networkedPlayers = GameObject.Find("NetworkedPlayers").GetComponent<NetworkedPlayers>();
        if (IsServer)
        {
            SpawnPlayers();
            SpawnHealthPickUps();
        }
        
    }

    private Vector3 NextPosition()
    {
        Vector3 pos = startPositions[positionIndex];
        positionIndex += 1;
        if (positionIndex > startPositions.Length - 1)
        {
            positionIndex = 0;
        }
        return pos;
    }


    private void SpawnPlayers()
    {
        foreach (NetworkPlayerInfo info in networkedPlayers.allNetPlayers)
        {
            Player prefab = playerPrefab;

            Player playerSpawn = Instantiate(
                prefab,
                NextPosition(),
                Quaternion.identity);
            playerSpawn.GetComponent<NetworkObject>().SpawnAsPlayerObject(info.clientId);
            playerSpawn.PlayerColor.Value = info.color;
        }
    }

    private void SpawnHealthPickUps()
    {
        foreach (Vector3 hpSpawnLoc in healthPickupPositions)
        {
            GameObject hpPickup = Instantiate(healthPickups, HPPickupNextPosition(), Quaternion.identity);
            hpPickup.GetComponent<NetworkObject>().Spawn();
        }
    }

}
