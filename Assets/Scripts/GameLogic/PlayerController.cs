using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerController : MonoBehaviour
{
    #region Properties
    public Vector2 TargetPosition { get; set; }
    #endregion

    #region Fields
    Player player;
    float speed = 5f;
    #endregion

    #region Methods
    void Awake()
    {
        player = GetComponent<Player>();
    }
    public void FixedUpdate()
    {
        Move(TargetPosition);
    }

    private void Move(Vector2 position)
    {
        Vector2 newPosition = transform.position = Vector2.MoveTowards(
            transform.position,
            position,
            speed * Time.fixedDeltaTime);

        ServerPacketsSender.PlayerMovement(player.Id, newPosition);
    }
    #endregion
}
