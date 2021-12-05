using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

class Server
{
    #region Properties
    public static int MaxPlayers { get; private set; }
    #endregion

    #region Delegates
    public delegate void PacketHandler(int fromClient, Packet packet);
    #endregion

    #region Fields
    static Client[] clients;
    static PacketHandler[] packetHandlers;

    static TcpListener tcpListener;
    static UdpClient udpListener;
    #endregion

    #region Methods
    public static void Start(int maxPlayers, int port)
    {
        MaxPlayers = maxPlayers;

        Debug.Log("Starting server...");
        InitializeServerData();

        tcpListener = new TcpListener(IPAddress.Any, port);
        tcpListener.Start();
        tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);

        udpListener = new UdpClient(port);
        udpListener.BeginReceive(UDPReceiveCallback, null);

        Debug.Log($"Server started on port {port}.");
    }
    static void InitializeServerData()
    {
        clients = new Client[MaxPlayers];
        for (int i = 0; i < MaxPlayers; i++)
            clients[i] = new Client(i);

        packetHandlers = new PacketHandler[]
        {
            ServerPacketsHandler.WelcomeReceived,
            ServerPacketsHandler.PlayerMovement
        };
        Debug.Log("Initialized packets.");
    }
    public static void Stop()
    {
        tcpListener.Stop();
        udpListener.Close();
    }

    public static Client GetClient(int index) => clients[index];

    public static PacketHandler GetPacketHandler(int index) => packetHandlers[index];

    static void TCPConnectCallback(IAsyncResult result)
    {
        TcpClient clientSocket = tcpListener.EndAcceptTcpClient(result);
        tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);
        Debug.Log($"Incoming connection from {clientSocket.Client.RemoteEndPoint}...");

        for (int i = 0; i < MaxPlayers; i++)
        {
            if (clients[i].Tcp.Socket == null)
            {
                clients[i].Tcp.Connect(clientSocket);
                clients[i].IsConnected = true;
                return;
            }
        }

        Debug.Log($"{clientSocket.Client.RemoteEndPoint} failed to connect: Server full!");
    }
    static void UDPReceiveCallback(IAsyncResult result)
    {
        try
        {
            IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] data = udpListener.EndReceive(result, ref clientEndPoint);
            udpListener.BeginReceive(UDPReceiveCallback, null);

            if (data.Length < 4)
            {
                return;
            }

            using (Packet packet = new Packet(data))
            {
                int clientId = packet.ReadInt();

                if (clients[clientId].Udp.EndPoint == null)
                {
                    clients[clientId].Udp.Connect(clientEndPoint);
                    return;
                }

                if (clients[clientId].Udp.EndPoint.ToString() == clientEndPoint.ToString())
                {
                    clients[clientId].Udp.HandleData(packet);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log($"Error receiving UDP data: {ex}");
        }
    }
    public static void SendUDPData(IPEndPoint clientEndPoint, Packet packet)
    {
        try
        {
            if (clientEndPoint != null)
            {
                udpListener.BeginSend(packet.ToArray(), packet.Length(), clientEndPoint, null, null);
            }
        }
        catch (Exception ex)
        {
            Debug.Log($"Error sending data to {clientEndPoint} via UDP: {ex}");
        }
    }
    #endregion
}