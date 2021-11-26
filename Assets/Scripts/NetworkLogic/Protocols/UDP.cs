using System.Net;

class UDP
{
    #region Properties
    public IPEndPoint EndPoint { get; private set; }
    #endregion

    #region Fields
    Client client;
    #endregion

    #region Methods
    public UDP(Client client)
    {
        this.client = client;
    }

    public void Connect(IPEndPoint endPoint)
    {
        EndPoint = endPoint;
    }
    public void Disconnect()
    {
        EndPoint = null;
    }

    public void SendData(Packet packet)
    {
        Server.SendUDPData(EndPoint, packet);
    }
    public void HandleData(Packet packetData)
    {
        int packetLength = packetData.ReadInt();
        byte[] packetBytes = packetData.ReadBytes(packetLength);

        ThreadManager.ExecuteOnMainThread(() =>
        {
            using (Packet packet = new Packet(packetBytes))
            {
                int packetId = packet.ReadInt();
                Server.GetPacketHandler(packetId)(client.Id, packet);
            }
        });
    }
    #endregion
}