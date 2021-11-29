using Pool;
using UnityEngine;

public class PlayersManager : MonoBehaviour
{
    #region Fields
    [SerializeField] GameObject playerPrefab;
    [Tooltip("The number that shows how many sectors you need to retreat " +
        "from the edge of the field for players to spawn.")]
    [SerializeField] int numberOfSectorsToIndentToSpawnPlayers;

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

    static FieldSector GetFieldSectorForPlayerSpawn() => Field.GetSector(
        Random.Range(instance.numberOfSectorsToIndentToSpawnPlayers,
            Field.NumberOfSectorsPerHeight - instance.numberOfSectorsToIndentToSpawnPlayers),
        Random.Range(instance.numberOfSectorsToIndentToSpawnPlayers,
            Field.NumberOfSectorsPerWidth - instance.numberOfSectorsToIndentToSpawnPlayers));
    static Vector2 GetRandomPositionInTheSector(Vector2 sectorSize,
        Vector3 startFieldSectorPosition) => new Vector2(
            startFieldSectorPosition.x + sectorSize.x* Random.value - 0.5f,
            startFieldSectorPosition.y + sectorSize.y* Random.value - 0.5f);

    public static Player SpawnPlayer(int id, string playerName) 
    {
        GameObject playerGameObject = PoolManager.GetGameObject(instance.playerPrefab.name);

        FieldSector startFieldSector = GetFieldSectorForPlayerSpawn();
        playerGameObject.transform.position = GetRandomPositionInTheSector(Field.SectorSize,
            startFieldSector.transform.position);

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