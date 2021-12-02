using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class Player : MonoBehaviour
{
    #region Properties
    public int Id { get; private set; }
    public string Username { get; private set; }
    public PlayerController Controller { get; private set; }
    public FieldSector CurrentFieldSector { get; set; }
    public List<Player> VisiblePlayers { get; } = new List<Player>();
    public List<Food> VisibleFood { get; } = new List<Food>();
    #endregion

    #region Methods
    public void Initialize(int id, string username, FieldSector startFieldSector)
    {
        Id = id;
        Username = username;
        CurrentFieldSector = startFieldSector;
    }

    void Awake()
    {
        Controller = GetComponent<PlayerController>();
    }
    public void Reset()
    {
        Id = default;
        Username = default;
        Controller?.Reset();
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Food"))
        {
            GameObject foodGameObject = collision.gameObject;
            Food food = foodGameObject.GetComponent<Food>(); ;

            food.FieldSector.Food.Remove(food);
            food.FieldSector = default;

            foodGameObject.SetActive(false);
            EatFood(food);
        }
    }
    public void EatFood(Food food)
    {
        float sizeChange = food.SizeChange;
        transform.localScale += new Vector3(sizeChange, sizeChange);

        Controller.Speed += food.SpeedChange;

        ServerPacketsSender.EatingFoodForVisiblePlayers(this, food, sizeChange);
    }
    #endregion
}