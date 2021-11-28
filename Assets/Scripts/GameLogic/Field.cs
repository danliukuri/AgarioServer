using Pool;
using UnityEngine;

[RequireComponent(typeof(EdgeCollider2D))]
public class Field : MonoBehaviour
{
    #region Properties
    public static int NumberOfSectorsPerHeight => instance.numberOfSectorsPerHeight;
    public static int NumberOfSectorsPerWidth => instance.numberOfSectorsPerWidth;
    public static Vector2 StartSectorPosition => instance.startSectorPosition;
    public static Vector2 SectorSize => instance.sectorSize;
    #endregion

    #region Fields
    [SerializeField] GameObject sectorGameObject;
    [SerializeField] Vector2 startSectorPosition;
    [SerializeField] int numberOfSectorsPerHeight;
    [SerializeField] int numberOfSectorsPerWidth;

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
}