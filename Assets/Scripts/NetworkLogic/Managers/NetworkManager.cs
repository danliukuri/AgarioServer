using UnityEngine;

public class NetworkManager : MonoBehaviour
{

    #region Fields
    [SerializeField] int maxPlayers;
    [SerializeField] int port;

    [SerializeField] GameObject playerPrefab;
    static NetworkManager instance;
    #endregion

    #region Methods
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 120;

#if UNITYEDITOR
        Debug.Log("Build the project to start the server!");
#else
        Server.Start(maxPlayers, port);
#endif
    }

    public static Player InstantiatePlayer()
    {
        return Instantiate(instance.playerPrefab, Vector3.zero, Quaternion.identity).GetComponent<Player>();
    }
    #endregion
}