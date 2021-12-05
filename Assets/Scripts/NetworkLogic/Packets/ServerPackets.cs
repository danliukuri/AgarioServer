/// <summary>Sent from server to client.</summary>
public enum ServerPackets
{
    Welcome,
    FieldGenerated,
    CurrentFieldSectorUpdate,
    SpawnPlayer,
    RemovePlayer,
    PlayerMovement,
    SpawnFood,
    RemoveFood,
    EatingFood,
    Losing
}