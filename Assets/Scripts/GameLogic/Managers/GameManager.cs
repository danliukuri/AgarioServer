using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Fields
    [SerializeField] FoodSpawner foodSpawner;
    [SerializeField] int startFoodCountToSpawn;
    [SerializeField] float startFoodSpawnDelay;
    #endregion

    #region Methods
    void Start()
    {
        Field.Generate();
        foodSpawner.Spawn(startFoodCountToSpawn);
        foodSpawner.InvokeRepeating(nameof(foodSpawner.Spawn), startFoodSpawnDelay, foodSpawner.SpawnRate);
    }
    #endregion
}