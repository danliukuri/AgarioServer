using System.Collections.Generic;
using UnityEngine;

public class FieldSector : MonoBehaviour
{
    #region Properties
    public (int Height, int Width) Indexes { get; set; }
    public List<Player> Players { get; set; } = new List<Player>();
    public List<Food> Food { get; set; } = new List<Food>();
    #endregion

    #region Methods
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            Players.Add(player);
            FieldSector currentPlayerFieldSector = this;
            player.CurrentFieldSector = currentPlayerFieldSector;

            ServerPacketsSender.SpawnVisiblePlayers(player);
            ServerPacketsSender.SpawnVisibleFood(player);
            ServerPacketsSender.CurrentFieldSectorUpdate(player.Id, currentPlayerFieldSector);
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            Players.Remove(player);
            FieldSector previousPlayerFieldSector = this;

            ServerPacketsSender.RemoveInvisiblePlayers(player, previousPlayerFieldSector);
            ServerPacketsSender.RemoveInvisibleFood(player, previousPlayerFieldSector);
        }
    }
    #endregion
}