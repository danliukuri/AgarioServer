using System.Collections.Generic;
using UnityEngine;

public class FieldSector : MonoBehaviour
{
    #region Properties
    public (int Hight, int Width) Indexes { get; set; }
    public IReadOnlyList<Player> Players => players;
    #endregion

    #region Fields
    List<Player> players = new List<Player>();
    #endregion

    #region Methods
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            players.Add(player);
            FieldSector currentPlayerFieldSector = this;
            player.CurrentFieldSector = currentPlayerFieldSector;

            ServerPacketsSender.SpawnVisiblePlayers(player);
            ServerPacketsSender.CurrentFieldSectorUpdate(player.Id, currentPlayerFieldSector);
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            players.Remove(player);

            ServerPacketsSender.RemoveInvisiblePlayers(player, this);
        }
    }
    #endregion
}