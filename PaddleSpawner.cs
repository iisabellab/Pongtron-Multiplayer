using Unity.Netcode;
using UnityEngine;

public class PaddleSpawner : NetworkBehaviour
{
    public GameObject paddle2Prefab; // Assign Paddle2 Prefab in the Inspector

    public override void OnNetworkSpawn()
    {
        if (IsServer && NetworkManager.Singleton != null) // Ensure NetworkManager exists
        {
            NetworkManager.Singleton.OnClientConnectedCallback += SpawnPaddleForClient;
        }
    }

    private void SpawnPaddleForClient(ulong clientId)
    {
        if (clientId != NetworkManager.Singleton.LocalClientId) // Only spawn for the joining client
        {
            GameObject paddle2 = Instantiate(paddle2Prefab, new Vector3(7f, 0, 0), Quaternion.identity);
            NetworkObject paddleNetworkObject = paddle2.GetComponent<NetworkObject>();
            paddleNetworkObject.SpawnWithOwnership(clientId); // Give control to the joining client
        }
    }

    private void OnDestroy()
    {
        if (IsServer && NetworkManager.Singleton != null) 
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= SpawnPaddleForClient;
        }
    }
}
