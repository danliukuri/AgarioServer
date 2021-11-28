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

        // TODO: Spawn in random sector
        FieldSector startFieldSector = Field.GetSector(0, 0);
        // TODO: Spawn in random position in start sector
        playerGameObject.transform.position = startFieldSector.transform.position; 

        Player player = playerGameObject.GetComponent<Player>();
        player.Controller.TargetPosition = playerGameObject.transform.position;
        player.Initialize(id, playerName);
        playerGameObject.SetActive(true);

        ServerPacketsSender.FieldGenerated(id);
        ServerPacketsSender.CurrentFieldSectorUpdate(id, startFieldSector);
        return player;
    }
    public static void RemovePlayer(Player player)
    {
        player.Reset();
        player.gameObject.SetActive(false);
    }
    #endregion
}