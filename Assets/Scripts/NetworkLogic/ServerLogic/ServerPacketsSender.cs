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
    public static void PlayerDisconnected(int playerId)
    {
        using (Packet packet = new Packet((int)ServerPackets.PlayerDisconnected))
        {
            packet.Write(playerId);

            SendTCPDataToAll(packet);
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
    public static void PlayerMovement(int playerId, Vector2 position)
    {
        using (Packet packet = new Packet((int)ServerPackets.PlayerMovement))
        {
            packet.Write(playerId);
            packet.Write(position);

            SendUDPDataToAll(packet);
        }
    }
    #endregion

    #region WaysToSend
    private static void SendTCPData(int toClient, Packet packet)
    {
        packet.WriteLength();
        Server.GetClient(toClient).Tcp.SendData(packet);
    }
    private static void SendUDPData(int toClient, Packet packet)
    {
        packet.WriteLength();
        Server.GetClient(toClient).Udp.SendData(packet);
    }
    private static void SendTCPDataToAll(Packet packet)
    {
        packet.WriteLength();
        for (int i = 0; i < Server.MaxPlayers; i++)
        {
            Server.GetClient(i).Tcp.SendData(packet);
        }
    }
    private static void SendUDPDataToAll(Packet packet)
    {
        packet.WriteLength();
        for (int i = 0; i < Server.MaxPlayers; i++)
        {
            Server.GetClient(i).Udp.SendData(packet);
        }
    }
    #endregion
}