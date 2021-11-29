using Pool;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EdgeCollider2D))]
public class Field : MonoBehaviour
{
    #region Properties
    public static int NumberOfSectorsPerHeight => instance.numberOfSectorsPerHeight;
    public static int NumberOfSectorsPerWidth => instance.numberOfSectorsPerWidth;

    public static Vector2 StartSectorPosition => instance.startSectorPosition;
    public static Vector2 SectorSize => instance.sectorSize;

    public static int ExpansionMagnitudeOfVisibleSectors => instance.expansionMagnitudeOfVisibleSectors;
    public static int ExpansionMagnitudeOfInvisibleSectors => instance.expansionMagnitudeOfInvisibleSectors;
    #endregion

    #region Fields
    [SerializeField] int numberOfSectorsPerHeight;
    [SerializeField] int numberOfSectorsPerWidth;

    [SerializeField] GameObject sectorGameObject;
    [SerializeField] Vector2 startSectorPosition;

    [Tooltip("The number by how much you need to expand the area of visible sectors. " +
        "Means that if this value is 1, the visible zone will be 3x3, if 2 then 5x5, etc.")]
    [SerializeField] int expansionMagnitudeOfVisibleSectors;
    [Tooltip("The number by how much you need to expand the zone of invisible sectors. " +
        "Means that if this value is 1, the invisible zone will be 3x3, if 2 then 5x5, etc.")]
    [SerializeField] int expansionMagnitudeOfInvisibleSectors;

    static FieldSector[,] sectors;
    Vector2 sectorSize;
    EdgeCollider2D edgeCollider;

    static Field instance;
    #endregion

    #region Methods
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            edgeCollider = GetComponent<EdgeCollider2D>();
            sectorSize = sectorGameObject.GetComponent<BoxCollider2D>().size;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    public static FieldSector GetSector(int heightIndex, int widthIndex) => sectors[heightIndex, widthIndex];

    static List<FieldSector> GetSectorsInExtendedZone(FieldSector currentFieldSector, int expansionMagnitude)
    {
        ((int Min, int Max) Hight, (int Min, int Max) Width) sectorsExtendedZone =
            GetExtendedZone(currentFieldSector.Indexes, expansionMagnitude,
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

    public void Generate()
    {
        sectors = new FieldSector[numberOfSectorsPerHeight, numberOfSectorsPerWidth];
        Vector2 position = startSectorPosition;
        
        for (int i = 0; i < numberOfSectorsPerHeight; i++)
        {
            for (int j = 0; j < numberOfSectorsPerWidth; j++)
            {
                GameObject gameObject = PoolManager.GetGameObject(sectorGameObject.name);
                gameObject.transform.position = position;
                FieldSector sector = gameObject.GetComponent<FieldSector>();
                sector.Indexes = (i, j);
                gameObject.SetActive(true);

                sectors[i, j] = sector;
                position.x += sectorSize.x;
            }
            position.x = startSectorPosition.x;
            position.y += sectorSize.y;
        }
        GenerateBorders(sectorSize);
    }
    void GenerateBorders(Vector2 size)
    {
        Vector2 startPoint = Vector2.zero;
        float maxHeight = startPoint.y + size.y * numberOfSectorsPerHeight;
        float maxWidth = startPoint.x + size.x * numberOfSectorsPerWidth;

        edgeCollider.points = new Vector2[]
        {
            startPoint,
            new Vector2(startPoint.x, maxHeight),
            new Vector2(maxWidth, maxHeight),
            new Vector2(maxWidth, startPoint.y),
            startPoint
        };
    }
    #endregion

    #region ArrayZoneMethods
    static int GetExtendedMinIndex(int index, int expansionMagnitude) =>
        index < expansionMagnitude ? 0 : index - expansionMagnitude;
    static int GetExtendedMaxIndex(int index, int expansionMagnitude, int maxArrayLength) =>
        index >= maxArrayLength - expansionMagnitude ? maxArrayLength - 1 : index + expansionMagnitude;
    static ((int Min, int Max) Hight, (int Min, int Max) Width) GetExtendedZone(
           ((int Min, int Max) Hight, (int Min, int Max) Width) zone,
           int expansionMagnitude, (int Hight, int Width) arrayDimensions)
    {
        ((int Min, int Max) Hight, (int Min, int Max) Width) expandedZone;
        expandedZone.Hight.Min = GetExtendedMinIndex(zone.Hight.Min, expansionMagnitude);
        expandedZone.Hight.Max = GetExtendedMaxIndex(zone.Hight.Max, expansionMagnitude, arrayDimensions.Hight);
        expandedZone.Width.Min = GetExtendedMinIndex(zone.Width.Min, expansionMagnitude);
        expandedZone.Width.Max = GetExtendedMaxIndex(zone.Width.Max, expansionMagnitude, arrayDimensions.Width);
        return expandedZone;
    }
    static ((int Min, int Max) Hight, (int Min, int Max) Width) GetExtendedZone(
        (int Hight, int Width) zone, int expansionMagnitude, (int Hight, int Width) arrayDimensions) =>
        GetExtendedZone(((zone.Hight, zone.Hight), (zone.Width, zone.Width)), expansionMagnitude, arrayDimensions);
    #endregion
}