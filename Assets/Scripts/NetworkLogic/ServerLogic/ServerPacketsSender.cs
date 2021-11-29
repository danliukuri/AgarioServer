using System.Collections.Generic;
using UnityEngine;

class ServerPacketsSender
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

            packet.Write(Field.StartSectorPosition);
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
            (int HightIndex, int WidthIndex) = fieldSector.Indexes;
            packet.Write(HightIndex);
            packet.Write(WidthIndex);

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
    #endregion

    #region PacketsSendingExtensions
    public static void SpawnVisiblePlayers(Player player)
    {
        int playerId = player.Id;

        List<Player> otherPlayers = Field.GetOtherPlayersInExtendedZone(playerId,
            player.CurrentFieldSector, Field.ExpansionMagnitudeOfVisibleSectors);
        foreach (Player otherPlayer in otherPlayers)
        {
            if (!player.VisiblePlayers.Contains(otherPlayer))
            {
                player.VisiblePlayers.Add(otherPlayer);
                SpawnPlayer(playerId, otherPlayer); // Spawn visible players for player
            }
            if (!otherPlayer.VisiblePlayers.Contains(player))
            {
                otherPlayer.VisiblePlayers.Add(player);
                SpawnPlayer(otherPlayer.Id, player); // Spawn player for visible players
            }
        }
    }
    public static void RemoveInvisiblePlayers(Player player, FieldSector previousPlayerFieldSector)
    {
        int playerId = player.Id;

        List<Player> previousOtherPlayers = Field.GetOtherPlayersInExtendedZone(playerId,
            previousPlayerFieldSector, Field.ExpansionMagnitudeOfInvisibleSectors);
        List<Player> currentOtherPlayers = Field.GetOtherPlayersInExtendedZone(playerId,
            player.CurrentFieldSector, Field.ExpansionMagnitudeOfInvisibleSectors);

        foreach (Player previousOtherPlayer in previousOtherPlayers)
        {
            bool isPreviousOtherPlayerNotCurrent = true;
            foreach (Player currentOtherPlayer in currentOtherPlayers)
                if (previousOtherPlayer == currentOtherPlayer)
                    isPreviousOtherPlayerNotCurrent = false;

            if (isPreviousOtherPlayerNotCurrent)
            {
                if (player.VisiblePlayers.Contains(previousOtherPlayer))
                {
                    player.VisiblePlayers.Remove(previousOtherPlayer);
                    RemovePlayer(playerId, previousOtherPlayer.Id); // Remove invisible players for player 
                }
                if (previousOtherPlayer.VisiblePlayers.Contains(player))
                {
                    previousOtherPlayer.VisiblePlayers.Remove(player);
                    RemovePlayer(previousOtherPlayer.Id, playerId); // Remove player for invisible players
                }
            }
        }
    }
    public static void PlayerDisconnected(Player player)
    {
        int playerId = player.Id;

        List<Player> otherPlayers = Field.GetOtherPlayersInExtendedZone(playerId,
            player.CurrentFieldSector, Field.ExpansionMagnitudeOfVisibleSectors);
        foreach (Player otherPlayer in otherPlayers)
            if (otherPlayer.VisiblePlayers.Contains(player))
            {
                otherPlayer.VisiblePlayers.Remove(player);
                RemovePlayer(otherPlayer.Id, playerId); // Remove player for visible players
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