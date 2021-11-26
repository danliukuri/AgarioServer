using System;
using System.Net.Sockets;
using UnityEngine;

class TCP
{
    #region Properties
    public TcpClient Socket { get; private set; }
    #endregion

    #region Fields
    static int dataBufferSize = 4096;
    readonly Client client;
    NetworkStream stream;
    Packet receivedData;
    byte[] receiveBuffer;
    #endregion

    #region Methods
    public TCP(Client client)
    {
        this.client = client;
    }

    public void Connect(TcpClient socket)
    {
        Socket = socket;
        Socket.ReceiveBufferSize = dataBufferSize;
        Socket.SendBufferSize = dataBufferSize;

        stream = Socket.GetStream();

        receivedData = new Packet();
        receiveBuffer = new byte[dataBufferSize];

        stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

        ServerPacketsSender.Welcome(client.Id, "Welcome to the server!");
    }
    public void Disconnect()
    {
        Socket.Close();
        stream = null;
        receivedData = null;
        receiveBuffer = null;
        Socket = null;
    }

    public void SendData(Packet packet)
    {
        try
        {
            if (Socket != null)
            {
                stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
            }
        }
        catch (Exception ex)
        {
            Debug.Log($"Error sending data to player {client.Id} via TCP: {ex}");
        }
    }
    bool HandleData(byte[] data)
    {
        int packetLength = 0;

        receivedData.SetBytes(data);

        if (receivedData.UnreadLength() >= 4)
        {
            packetLength = receivedData.ReadInt();
            if (packetLength <= 0)
            {
                return true;
            }
        }

        while (packetLength > 0 && packetLength <= receivedData.UnreadLength())
        {
            byte[] packetBytes = receivedData.ReadBytes(packetLength);
            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet packet = new Packet(packetBytes))
                {
                    int packetId = packet.ReadInt();
                    Server.GetPacketHandler(packetId)(client.Id, packet);
                }
            });

            packetLength = 0;
            if (receivedData.UnreadLength() >= 4)
            {
                packetLength = receivedData.ReadInt();
                if (packetLength <= 0)
                {
                    return true;
                }
            }
        }

        if (packetLength <= 1)
        {
            return true;
        }

        return false;
    }

    void ReceiveCallback(IAsyncResult result)
    {
        try
        {
            int byteLength = stream.EndRead(result);
            if (byteLength <= 0)
            {
                client.Disconnect();
                return;
            }

            byte[] data = new byte[byteLength];
            Array.Copy(receiveBuffer, data, byteLength);

            receivedData.Reset(HandleData(data));
            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
        }
        catch (Exception ex)
        {
            Debug.Log($"Error receiving TCP data: {ex}");
            client.Disconnect();
        }
    }
    #endregion
}