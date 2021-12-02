using Pool;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EdgeCollider2D))]
public class Field : MonoBehaviour
{
    #region Properties
    public static float Height { get; private set; }
    public static float Width { get; private set; }

    public static int NumberOfSectorsPerHeight => instance.numberOfSectorsPerHeight;
    public static int NumberOfSectorsPerWidth => instance.numberOfSectorsPerWidth;

    public static Vector2 Position => instance.transform.position;
    public static Vector2 SectorSize { get; private set; }

    public static int ExpansionMagnitudeOfVisibleSectors => instance.expansionMagnitudeOfVisibleSectors;
    public static int ExpansionMagnitudeOfInvisibleSectors => instance.expansionMagnitudeOfInvisibleSectors;
    #endregion

    #region Fields
    [SerializeField] int numberOfSectorsPerHeight;
    [SerializeField] int numberOfSectorsPerWidth;

    [SerializeField] GameObject sectorGameObject;

    [Tooltip("The number by how much you need to expand the area of visible sectors. " +
        "Means that if this value is 1, the visible zone will be 3x3, if 2 then 5x5, etc.")]
    [SerializeField] int expansionMagnitudeOfVisibleSectors;
    [Tooltip("The number by how much you need to expand the zone of invisible sectors. " +
        "Means that if this value is 1, the invisible zone will be 3x3, if 2 then 5x5, etc.")]
    [SerializeField] int expansionMagnitudeOfInvisibleSectors;

    static FieldSector[,] sectors;
    EdgeCollider2D edgeCollider;

    static Field instance;
    #endregion

    #region Methods
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Initialize();
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }
    void Initialize()
    {
        edgeCollider = GetComponent<EdgeCollider2D>();
        SectorSize = sectorGameObject.GetComponent<BoxCollider2D>().size;

        Height = SectorSize.y * numberOfSectorsPerHeight;
        Width = SectorSize.x * numberOfSectorsPerWidth;

        sectors = new FieldSector[numberOfSectorsPerHeight, numberOfSectorsPerWidth];
    }

    public static Vector2 GetRandomPositionInTheSector(Vector3 sectorPosition,
        float percentageIndentFromSectorEdges) => new Vector2(
        sectorPosition.x + (1f - percentageIndentFromSectorEdges) * SectorSize.x * (Random.value - 0.5f),
        sectorPosition.y + (1f - percentageIndentFromSectorEdges) * SectorSize.y * (Random.value - 0.5f));
    public static FieldSector GetRandomFieldSector(int numberOfSectorsToIndentToSpawnPlayers = 0) => sectors[
        Random.Range(numberOfSectorsToIndentToSpawnPlayers,
            NumberOfSectorsPerHeight - numberOfSectorsToIndentToSpawnPlayers),
        Random.Range(numberOfSectorsToIndentToSpawnPlayers,
            NumberOfSectorsPerWidth - numberOfSectorsToIndentToSpawnPlayers)];

    public static List<FieldSector> GetSectorsInExtendedZone(FieldSector currentFieldSector, int expansionMagnitude)
    {
        ((int Min, int Max) Hight, (int Min, int Max) Width) sectorsExtendedZone =
            ArrayZone.GetExtendedZone(currentFieldSector.Indexes, expansionMagnitude,
            (NumberOfSectorsPerHeight, NumberOfSectorsPerWidth));

        List<FieldSector> sectorsInExtendedZone = new List<FieldSector>();
        for (int i = sectorsExtendedZone.Hight.Min; i <= sectorsExtendedZone.Hight.Max; i++)
            for (int j = sectorsExtendedZone.Width.Min; j <= sectorsExtendedZone.Width.Max; j++)
                sectorsInExtendedZone.Add(sectors[i, j]);
        return sectorsInExtendedZone;
    }
    public static List<Player> GetOtherPlayersInExtendedZone(int playerId,
        FieldSector playerFieldSector, int expansionMagnitude)
    {
        List<Player> otherPlayers = new List<Player>();
        List<FieldSector> sectorsInExtendedZone = GetSectorsInExtendedZone(playerFieldSector, expansionMagnitude);
        foreach (FieldSector fieldSector in sectorsInExtendedZone)
            foreach (Player otherPlayer in fieldSector.Players)
                if (otherPlayer.Id != playerId)
                    otherPlayers.Add(otherPlayer);
        return otherPlayers;
    }

    public static void Generate()
    {
        Vector2 startSectorPosition = new Vector2(SectorSize.x / 2f + instance.transform.position.x,
                                                  SectorSize.y / 2f + instance.transform.position.y);
        Vector2 position = startSectorPosition;

        for (int i = 0; i < instance.numberOfSectorsPerHeight; i++)
        {
            for (int j = 0; j < instance.numberOfSectorsPerWidth; j++)
            {
                GameObject gameObject = PoolManager.GetGameObject(instance.sectorGameObject.name);
                gameObject.transform.position = position;
                FieldSector sector = gameObject.GetComponent<FieldSector>();
                sector.Indexes = (i, j);
                gameObject.SetActive(true);

                sectors[i, j] = sector;
                position.x += SectorSize.x;
            }
            position.x = startSectorPosition.x;
            position.y += SectorSize.y;
        }
        GenerateBorders();
    }
    static void GenerateBorders()
    {
        float minX = 0;
        float minY = 0;
        float maxX = Width;
        float maxY = Height;

        Vector2 startPoint = new Vector2(minX, minY);
        instance.edgeCollider.points = new Vector2[]
        {
            startPoint,
            new Vector2(minX, maxY),
            new Vector2(maxX, maxY),
            new Vector2(maxX, minY),
            startPoint
        };
    }
    #endregion
}