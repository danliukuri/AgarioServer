/// <summary>Sent from server to client.</summary>
public enum ServerPackets
{
    Welcome,
    PlayerDisconnected,
    FieldGenerated,
    CurrentFieldSectorUpdate,
    SpawnPlayer,
    PlayerMovement
}