using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Netcode;
using Unity.VisualScripting;

public class Player : NetworkBehaviour
{
    public float movementSpeed = 50f;
    public float rotationSpeed = 130f;
    public NetworkVariable<Color> PlayerColor = new NetworkVariable<Color>(Color.red);
    public BulletSpawner bulletSpawner;
    public NetworkVariable<int> ScoreNetVar = new NetworkVariable<int>(0);
    public NetworkVariable<int> playerHP = new NetworkVariable<int>();

    private Camera playerCamera;
    public GameObject playerBody;


    private void NetworkInit()
    {
        playerCamera = transform.Find("Camera").GetComponent<Camera>();
        playerCamera.enabled = IsOwner;
        playerCamera.GetComponent<AudioListener>().enabled = IsOwner;

        ApplyPlayerColor();
        PlayerColor.OnValueChanged += OnPlayerColorChanged;

        if (IsClient)
        {
            ScoreNetVar.OnValueChanged += ClientOnScoreValueChanged;
        }
    }

    private void Awake()
    {
        NetworkHelper.Log(this, "Awake");
    }

    private void Start()
    {
        NetworkHelper.Log(this, "Start");
    }

    public override void OnNetworkSpawn()
    {
        NetworkHelper.Log(this, "OnNetworkSpawn");
        NetworkInit();
        base.OnNetworkSpawn();
        playerHP.Value = 100;
    }

    private void ClientOnScoreValueChanged(int old, int current)
    {
        if (IsOwner)
        {
            NetworkHelper.Log(this, $"My score is {ScoreNetVar.Value}");
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if(IsServer)
        {
            ServerHandleCollision(collision);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsServer)
        {
            if (other.CompareTag("powerup"))
            {
                other.GetComponent<BasePowerUp>().ServerPickUp(this);
            }
        }

        if(!IsServer)
        {
            return;
        }
        if(other.GetComponent<BulletScript>())
        {
            Debug.Log("Player Damage " + other.GetComponent<NetworkObject>().OwnerClientId);

            //NetworkManager.Singleton.ConnectedClients[other.GetComponent<NetworkObject>().OwnerClientId].PlayerObject.GetComponent<NetworkPlayerData>().score.Value += 1;
            playerHP.Value -= 10;
        }
        else if(other.GetComponent<HealthPickup>())
        {
            Debug.Log("Player HP+");
            playerHP.Value += 50;
        }
    }

    private void ServerHandleCollision(Collision collision)
    {
        if(collision.gameObject.CompareTag("bullet"))
        {
            ulong ownerId = collision.gameObject.GetComponent<NetworkObject>().OwnerClientId;
            NetworkHelper.Log(this,
                $"Hit by {collision.gameObject.name} " +
                $"Owned by {ownerId}");
            Player other = NetworkManager.Singleton.ConnectedClients[ownerId].PlayerObject.GetComponent<Player>();
            other.ScoreNetVar.Value += 1;
            Destroy(collision.gameObject);
        }
        
    }

    private void Update()
    {
        if(IsOwner) 
        {
            OwnerHandleInput();
            if (Input.GetButtonDown("Fire1"))
            {
                NetworkHelper.Log("Request Fire");
            }
        }
        
    }

    public void OnPlayerColorChanged(Color previous, Color current)
    {
        ApplyPlayerColor();
    }

    private void OwnerHandleInput()
    {
        Vector3 movement = CalcMovement();
        Vector3 rotation = CalcRotation();

        if (movement != Vector3.zero || rotation != Vector3.zero)
        {
            MoveServerRPC(movement, rotation);
        }
    }

    public void ApplyPlayerColor()
    {
        NetworkHelper.Log(this, $"Applying color {PlayerColor.Value}");
        playerBody.GetComponent<MeshRenderer>().material.color = PlayerColor.Value;
    }

    [ServerRpc(RequireOwnership = true)]
    public void MoveServerRPC(Vector3 movement, Vector3 rotation)
    {
        transform.Translate(movement);
        transform.Rotate(rotation);
    }


    //Rotate around the y axis when shift is not pressed
    private Vector3 CalcRotation()
    {
        bool isShiftKeyDown = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        Vector3 rotVect = Vector3.zero;
        if (!isShiftKeyDown)
        {
            rotVect = new Vector3(0, Input.GetAxis("Horizontal"), 0);
            rotVect *= rotationSpeed * Time.deltaTime;
        }
        return rotVect;
    }


    // Move up and back, and strafe when shift is pressed
    private Vector3 CalcMovement()
    {
        bool isShiftKeyDown = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        float x_move = 0.0f;
        float z_move = Input.GetAxis("Vertical");

        if (isShiftKeyDown)
        {
            x_move = Input.GetAxis("Horizontal");
        }

        Vector3 moveVect = new Vector3(x_move, 0, z_move);
        moveVect *= movementSpeed * Time.deltaTime;

        return moveVect;
    }


}
