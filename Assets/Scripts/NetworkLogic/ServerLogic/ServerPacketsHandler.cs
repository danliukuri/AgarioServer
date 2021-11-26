using UnityEngine;

class ServerPacketsHandler
{
    #region PacketsHandling
    public static void WelcomeReceived(int fromClient, Packet packet)
    {
        int clientIdCheck = packet.ReadInt();
        string username = packet.ReadString();

        Debug.Log($"{Server.GetClient(fromClient).Tcp.Socket.Client.RemoteEndPoint} connected successfully and is now player {fromClient}.");
        if (fromClient != clientIdCheck)
        {
            Debug.Log($"Player \"{username}\" (ID: {fromClient}) has assumed the wrong client ID ({clientIdCheck})!");
        }
        Server.GetClient(fromClient).SendIntoGame(username);
    }

    public static void PlayerMovement(int fromClient, Packet packet)
    {
        Vector2 position = packet.ReadVector2();
        Server.GetClient(fromClient).Player.Controller.TargetPosition = position;
    }
    #endregion
}