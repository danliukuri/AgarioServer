using Pool;
using UnityEngine;

public class PlayersManager : MonoBehaviour
{
    #region Fields
    [SerializeField] GameObject playerPrefab;
    [Tooltip("The number that shows how many sectors you need to retreat " +
        "from the edge of the field for players to spawn.")]
    [SerializeField] int numberOfSectorsToIndentToSpawnPlayers;
    [SerializeField] float percentageIndentFromSectorEdgesToSpawnPlayers;
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

        FieldSector startFieldSector = Field.GetRandomFieldSector(instance.numberOfSectorsToIndentToSpawnPlayers);
        playerGameObject.transform.position = Field.GetRandomPositionInTheSector(
            startFieldSector.transform.position, instance.percentageIndentFromSectorEdgesToSpawnPlayers);

        Player player = playerGameObject.GetComponent<Player>();
        player.Controller.TargetPosition = playerGameObject.transform.position;

        player.Initialize(id, playerName, startFieldSector);
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