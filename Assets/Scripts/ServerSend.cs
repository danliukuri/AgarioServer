class ServerSend
{
    private static void SendTCPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.GetClient(_toClient).Tcp.SendData(_packet);
    }
    private static void SendUDPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.GetClient(_toClient).Udp.SendData(_packet);
    }
    private static void SendTCPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.GetClient(i).Tcp.SendData(_packet);
        }
    }
    private static void SendUDPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.GetClient(i).Udp.SendData(_packet);
        }
    }
    #region Packets
    public static void Welcome(int toClient, string msg)
    {
        using (Packet packet = new Packet((int)ServerPackets.Welcome))
        {
            packet.Write(msg);
            packet.Write(toClient);

            SendTCPData(toClient, packet);
        }
    }
    public static void SpawnPlayer(int toClient, Player player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.SpawnPlayer))
        {
            _packet.Write(player.Id);
            _packet.Write(player.Username);
            _packet.Write(player.transform.position);

            SendTCPData(toClient, _packet);
        }
    }
    #endregion
}