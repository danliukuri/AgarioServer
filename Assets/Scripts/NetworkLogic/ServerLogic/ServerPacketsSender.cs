using System.Collections.Generic;
using UnityEngine;

static class ServerPacketsSender
{
    #region PacketsSending
    public static void Welcome(int toClient, string msg)
    {
        using (Packet packet = new Packet((int)ServerPackets.Welcome))
        {
            packet.Write(msg);
            packet.Write(toClient);

            SendTCPData(toClient, packet);
        }
    }

    public static void FieldGenerated(int toClient)
    {
        using (Packet packet = new Packet((int)ServerPackets.FieldGenerated))
        {
            packet.Write(Field.NumberOfSectorsPerHeight);
            packet.Write(Field.NumberOfSectorsPerWidth);

            packet.Write(Field.Position);
            packet.Write(Field.SectorSize);

            packet.Write(Field.ExpansionMagnitudeOfVisibleSectors);
            packet.Write(Field.ExpansionMagnitudeOfInvisibleSectors);

            SendTCPData(toClient, packet);
        }
    }
    public static void CurrentFieldSectorUpdate(int toClient, FieldSector fieldSector)
    {
        using (Packet packet = new Packet((int)ServerPackets.CurrentFieldSectorUpdate))
        {
            (int hightIndex, int widthIndex) = fieldSector.Indexes;
            packet.Write(hightIndex);
            packet.Write(widthIndex);

            SendTCPData(toClient, packet);
        }
    }

    public static void SpawnPlayer(int toClient, Player player)
    {
        using (Packet packet = new Packet((int)ServerPackets.SpawnPlayer))
        {
            packet.Write(player.Id);
            packet.Write(player.Username);
            packet.Write(player.transform.position);

            SendTCPData(toClient, packet);
        }
    }
    static void RemovePlayer(int toClient, int playerId)
    {
        using (Packet packet = new Packet((int)ServerPackets.RemovePlayer))
        {
            packet.Write(playerId);

            SendTCPData(toClient, packet);
        }
    }

    static void PlayerMovement(int toClient, int fromClient, Vector2 position)
    {
        using (Packet packet = new Packet((int)ServerPackets.PlayerMovement))
        {
            packet.Write(fromClient);
            packet.Write(position);

            SendUDPData(toClient, packet);
        }
    }

    static void SpawnFood(int toClient, FieldSector fieldSector, Vector2 position)
    {
        using (Packet packet = new Packet((int)ServerPackets.SpawnFood))
        {
            (int hightIndex, int widthIndex) = fieldSector.Indexes;
            packet.Write(hightIndex);
            packet.Write(widthIndex);
            packet.Write(position);

            SendTCPData(toClient, packet);
        }
    }
    static void RemoveFood(int toClient, FieldSector fieldSector)
    {
        using (Packet packet = new Packet((int)ServerPackets.RemoveFood))
        {
            (int hightIndex, int widthIndex) = fieldSector.Indexes;
            packet.Write(hightIndex);
            packet.Write(widthIndex);

            SendTCPData(toClient, packet);
        }
    }
    #endregion

    #region PacketsSendingExtensions
    public static void SpawnVisiblePlayers(Player player)
    {
        int playerId = player.Id;

        List<Player> visibleOtherPlayers = Field.GetOtherPlayersInExtendedZone(playerId,
            player.CurrentFieldSector, Field.ExpansionMagnitudeOfVisibleSectors);
        foreach (Player visibleOtherPlayer in visibleOtherPlayers)
        {
            if (!player.VisiblePlayers.Contains(visibleOtherPlayer))
            {
                player.VisiblePlayers.Add(visibleOtherPlayer);
                SpawnPlayer(playerId, visibleOtherPlayer); // Spawn visible players for player
            }
            if (!visibleOtherPlayer.VisiblePlayers.Contains(player))
            {
                visibleOtherPlayer.VisiblePlayers.Add(player);
                SpawnPlayer(visibleOtherPlayer.Id, player); // Spawn player for visible players
            }
        }
    }
    public static void RemoveInvisiblePlayers(Player player, FieldSector fieldSector)
    {
        int playerId = player.Id;

        List<Player> invisibleOtherPlayers = Field.GetOtherPlayersInExtendedZone(playerId,
            fieldSector, Field.ExpansionMagnitudeOfInvisibleSectors + 1);
        List<Player> visibleOtherPlayers = Field.GetOtherPlayersInExtendedZone(playerId,
            fieldSector, Field.ExpansionMagnitudeOfInvisibleSectors);

        foreach (Player invisibleOtherPlayer in invisibleOtherPlayers)
        {
            bool isInvisibleOtherPlayerVisible = true;
            foreach (Player visibleOtherPlayer in visibleOtherPlayers)
                if (invisibleOtherPlayer == visibleOtherPlayer)
                    isInvisibleOtherPlayerVisible = false;

            if (isInvisibleOtherPlayerVisible)
            {
                if (player.VisiblePlayers.Contains(invisibleOtherPlayer))
                {
                    player.VisiblePlayers.Remove(invisibleOtherPlayer);
                    RemovePlayer(playerId, invisibleOtherPlayer.Id); // Remove invisible players for player 
                }
                if (invisibleOtherPlayer.VisiblePlayers.Contains(player))
                {
                    invisibleOtherPlayer.VisiblePlayers.Remove(player);
                    RemovePlayer(invisibleOtherPlayer.Id, playerId); // Remove player for invisible players
                }
            }
        }
    }
    public static void PlayerDisconnected(Player player)
    {
        int playerId = player.Id;

        List<Player> visibleOtherPlayers = Field.GetOtherPlayersInExtendedZone(playerId,
            player.CurrentFieldSector, Field.ExpansionMagnitudeOfVisibleSectors);
        foreach (Player visibleOtherPlayer in visibleOtherPlayers)
            if (visibleOtherPlayer.VisiblePlayers.Contains(player))
            {
                visibleOtherPlayer.VisiblePlayers.Remove(player);
                RemovePlayer(visibleOtherPlayer.Id, playerId); // Remove player for visible players
            }
    }

    public static void PlayerMovementForVisiblePlayers(Player player, Vector2 position)
    {
        int playerId = player.Id;
        PlayerMovement(playerId, playerId, position); // Send this player his movement
        List<Player> otherPlayers = Field.GetOtherPlayersInExtendedZone(playerId,
            player.CurrentFieldSector, Field.ExpansionMagnitudeOfVisibleSectors);
        foreach (Player otherPlayer in otherPlayers)
            if (player.VisiblePlayers.Contains(otherPlayer))
                // Send this player's movement to his visible players
                PlayerMovement(otherPlayer.Id, playerId, position);
    }

    public static void SpawnVisibleFood(Player player)
    {
        int playerId = player.Id;
        List<FieldSector> sectorsWithVisibleFood = Field.GetSectorsInExtendedZone(player.CurrentFieldSector,
            Field.ExpansionMagnitudeOfVisibleSectors);
        foreach (FieldSector fieldSector in sectorsWithVisibleFood)
            foreach (Transform food in fieldSector.Food)
                if (!player.VisibleFood.Contains(food))
                {
                    player.VisibleFood.Add(food);
                    SpawnFood(playerId, fieldSector, food.position); // Spawn visible food for player
                }
    }
    public static void RemoveInvisibleFood(Player player, FieldSector fieldSector)
    {
        List<FieldSector> sectorsWithInvisibleFood = Field.GetSectorsInExtendedZone(
            fieldSector, Field.ExpansionMagnitudeOfInvisibleSectors + 1);
        List<FieldSector> sectorsWithVisibleFood = Field.GetSectorsInExtendedZone(
            fieldSector, Field.ExpansionMagnitudeOfInvisibleSectors);

        int playerId = player.Id;

        foreach (FieldSector sectorWithInvisibleFood in sectorsWithInvisibleFood)
        {
            bool isSectorWithInvisibleFoodVisible = true;
            foreach (FieldSector sectorWithVisibleFood in sectorsWithVisibleFood)
                if (sectorWithInvisibleFood == sectorWithVisibleFood)
                    isSectorWithInvisibleFoodVisible = false;

            if (isSectorWithInvisibleFoodVisible)
                foreach (Transform food in sectorWithInvisibleFood.Food)
                    if (player.VisibleFood.Contains(food))
                    {
                        player.VisibleFood.Remove(food);
                        /// Remove invisible food for player 
                        RemoveFood(playerId, sectorWithInvisibleFood);
                    }
        }
    }
    #endregion

    #region WaysToSend
    static void SendTCPData(int toClient, Packet packet)
    {
        packet.WriteLength();
        Server.GetClient(toClient).Tcp.SendData(packet);
    }
    static void SendUDPData(int toClient, Packet packet)
    {
        packet.WriteLength();
        Server.GetClient(toClient).Udp.SendData(packet);
    }
    #endregion
}