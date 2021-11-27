using Pool;
using UnityEngine;

public class PlayersManager : MonoBehaviour
{
    #region Fields
    [SerializeField] GameObject playerPrefab;
    static PlayersManager instance;
    #endregion

    #region Methods
    void Awake()
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

    public static Player SpawnPlayer(int id, string playerName)
    {
        GameObject playerGameObject = PoolManager.GetGameObject(instance.playerPrefab.name);
        Player player = playerGameObject.GetComponent<Player>();
        player.Initialize(id, playerName);
        playerGameObject.SetActive(true);

        return player;
    }
    public static void RemovePlayer(Player player)
    {
        player.Reset();
        player.gameObject.SetActive(false);
    }
    #endregion
}