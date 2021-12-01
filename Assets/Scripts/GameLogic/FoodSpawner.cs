using Pool;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    #region Properties
    public float SpawnRate => spawnRate;
    #endregion

    #region Fields
    [SerializeField] GameObject foodPrefab;
    [SerializeField] float spawnRate;
    [SerializeField] float percentageIndentFromSectorEdgesToSpawnFood;
    #endregion

    #region Methods
    public void Spawn(int count)
    {
        for (int i = 0; i < count; i++)
            Spawn();
    }
    public void Spawn()
    {
        GameObject foodGameObject = PoolManager.GetGameObject(foodPrefab.name);
        Transform foodTransform = foodGameObject.transform;

        FieldSector fieldSector = Field.GetRandomFieldSector();
        fieldSector.Food.Add(foodTransform);

        foodTransform.position = Field.GetRandomPositionInTheSector(
            fieldSector.transform.position, percentageIndentFromSectorEdgesToSpawnFood);
        foodGameObject.SetActive(true);
    }
    #endregion
}
