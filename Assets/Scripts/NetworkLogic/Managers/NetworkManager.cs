using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    #region Fields
    [SerializeField] int maxPlayers;
    [SerializeField] int port;
    #endregion

    #region Methods
    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 120;

        Server.Start(maxPlayers, port);
    }
    private void OnApplicationQuit()
    {
        Server.Stop();
    }
    #endregion
}