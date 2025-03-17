using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class ChangeScene : MonoBehaviour
{
    public void MoveToScene(int sceneID)
    {
        SceneManager.LoadScene(sceneID);

        if (sceneID == 1)
        {
            if (NetworkManager.Singleton != null && !NetworkManager.Singleton.IsListening)
            {
                NetworkManager.Singleton.StartHost();
            }
        }
    }
}
