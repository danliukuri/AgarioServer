using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance => instance;

    [SerializeField] GameObject playerPrefab;
    static NetworkManager instance;

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
        #if UNITY_EDITOR
        Debug.Log("Build the project to start the server!");
        #else
        Server.Start(50, 26950);
        #endif
    }

    public Player InstantiatePlayer()
    {
        return Instantiate(playerPrefab, Vector3.zero, Quaternion.identity).GetComponent<Player>();
    }
}