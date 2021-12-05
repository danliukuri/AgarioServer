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

    public List<Player> VisiblePlayers { get; private set; } = new List<Player>();
    public List<Food> VisibleFood { get; private set; } = new List<Food>();

    public float Size
    {
        get => transform.localScale.x;
        private set => transform.localScale = new Vector3(value, value, defaultSize);
    }
    #endregion

    #region Fields
    static float defaultSize;
    #endregion

    #region Methods
    void Awake()
    {
        Controller = GetComponent<PlayerController>();
        defaultSize = transform.localScale.x;
    }
    public void Initialize(int id, string username, FieldSector currentFieldSector, float size)
    {
        Id = id;
        Username = username;
        CurrentFieldSector = currentFieldSector;
        Size = size;
    }
    public void Reset()
    {
        Id = default;
        Username = default;
        Controller?.Reset();
        if (CurrentFieldSector != default)
        {
            if (CurrentFieldSector.Players.Contains(this))
                CurrentFieldSector.Players.Remove(this);
            CurrentFieldSector = default;
        }
        VisiblePlayers = new List<Player>();
        VisibleFood = new List<Food>();
        Size = defaultSize;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Food"))
        {
            GameObject foodGameObject = collision.gameObject;
            Food food = foodGameObject.GetComponent<Food>(); ;
            foodGameObject.SetActive(false);
            EatFood(food);
        }
    }
    public void EatFood(Food food)
    {
        float sizeChange = food.SizeChange;
        Size += sizeChange;

        Controller.Speed += food.SpeedChange;

        ServerPacketsSender.EatingFoodForVisiblePlayers(this, food, sizeChange);
        food.Reset();
    }
    #endregion
}