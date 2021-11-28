using System.Collections.Generic;
using UnityEngine;

public class FieldSector : MonoBehaviour
{
    #region Properties
    public (int Hight, int Width) Indexes { get; set; }
    #endregion

    #region Fields
    List<GameObject> players = new List<GameObject>();
    #endregion

    #region Methods
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            GameObject playerGameObject = collision.gameObject;
            players.Add(playerGameObject);

            Player player = playerGameObject.GetComponent<Player>();
            ServerPacketsSender.CurrentFieldSectorUpdate(player.Id, this);
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            players.Remove(collision.gameObject);
    }
    #endregion
}