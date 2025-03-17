using UnityEngine;
using Unity.Netcode;

public class Network : MonoBehaviour
{
    private static Network instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); 
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
