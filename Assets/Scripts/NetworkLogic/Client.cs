﻿using UnityEngine;

class Client
{
    #region Properties
    public int Id { get; private set; }
    public Player Player { get; set; }
    public TCP Tcp { get; private set; }
    public UDP Udp { get; private set; }
    public bool IsConnected { get; set; }
    #endregion

    #region Methods
    public Client(int id)
    {
        Id = id;
        Tcp = new TCP(this);
        Udp = new UDP(this);
    }

    /// <summary>Sends the client into the game and informs other clients of the new player.</summary>
    /// <param name="playerName">The username of the new player.</param>
    public void SendIntoGame(string playerName)
    {
        Player = PlayersManager.SpawnPlayer(Id, playerName);

        ServerPacketsSender.SpawnPlayer(Id, Player);
        ServerPacketsSender.SpawnVisiblePlayers(Player);
    }
    /// <summary>Disconnects the client and stops all network traffic.</summary>
    public void Disconnect()
    {
        if (IsConnected)
        {
            IsConnected = false;
            Debug.Log($"{Tcp.Socket.Client.RemoteEndPoint} has disconnected.");

            Tcp.Disconnect();
            Udp.Disconnect();

            if (Player != default)
            {
                ServerPacketsSender.RemovePlayerForInvisiblePlayers(Player);
                PlayersManager.RemovePlayer(Player);
            }
        }
    }
    #endregion
}