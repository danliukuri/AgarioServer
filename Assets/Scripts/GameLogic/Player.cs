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
    #endregion
}