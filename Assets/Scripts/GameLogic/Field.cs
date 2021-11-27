using Pool;
using UnityEngine;

[RequireComponent(typeof(EdgeCollider2D))]
public class Field : MonoBehaviour
{
    #region Fields
    [SerializeField] GameObject sector;
    [SerializeField] Vector2 startSectorPosition;
    [SerializeField] int numberOfSectorsPerHeight;
    [SerializeField] int numberOfSectorsPerWidth;
    Vector2 sectorSize;
    EdgeCollider2D edgeCollider;
    #endregion

    #region Methods
    private void Awake()
    {
        edgeCollider = GetComponent<EdgeCollider2D>();
        sectorSize = sector.GetComponent<BoxCollider2D>().size;
    }
    public void Generate()
    {
        Vector2 position = startSectorPosition;
        
        for (int i = 0; i < numberOfSectorsPerHeight; i++)
        {
            for (int j = 0; j < numberOfSectorsPerWidth; j++)
            {
                GameObject gameObject = PoolManager.GetGameObject(sector.name);
                gameObject.transform.position = position;
                gameObject.SetActive(true);

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