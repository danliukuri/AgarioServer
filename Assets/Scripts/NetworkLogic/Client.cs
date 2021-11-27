using UnityEngine;

class Client
{
    #region Properties
    public int Id { get; private set; }
    public Player Player { get; private set; }
    public TCP Tcp { get; private set; }
    public UDP Udp { get; private set; }
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

        // Send all players to the new player
        for (int i = 0; i < Server.ClientsCount(); i++)
        {
            Client client = Server.GetClient(i);
            if (client.Player != null)
            {
                if (client.Id != Id)
                {
                    ServerPacketsSender.SpawnPlayer(Id, client.Player);
                }
            }
        }

        // Send the new player to all players (including himself)
        for (int i = 0; i < Server.ClientsCount(); i++)
        {
            Client client = Server.GetClient(i);
            if (client.Player != null)
            {
                ServerPacketsSender.SpawnPlayer(client.Id, Player);
            }
        }
    }
    /// <summary>Disconnects the client and stops all network traffic.</summary>
    public void Disconnect()
    {
        Debug.Log($"{Tcp.Socket.Client.RemoteEndPoint} has disconnected.");

        ThreadManager.ExecuteOnMainThread(() =>
        {
            PlayersManager.RemovePlayer(Player);
            Player = null;
        });

        Tcp.Disconnect();
        Udp.Disconnect();

        ServerPacketsSender.PlayerDisconnected(Id);
    }
    #endregion
}