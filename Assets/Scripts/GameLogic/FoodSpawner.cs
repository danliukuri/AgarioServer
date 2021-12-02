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

    int foodId = 0;
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
        FieldSector fieldSector = Field.GetRandomFieldSector();
        foodGameObject.transform.position = Field.GetRandomPositionInTheSector(
            fieldSector.transform.position, percentageIndentFromSectorEdgesToSpawnFood);

        Food food = foodGameObject.GetComponent<Food>();
        food.Id = foodId++;
        food.FieldSector = fieldSector;
        fieldSector.Food.Add(food);
        foodGameObject.SetActive(true);
    }
    #endregion
}
