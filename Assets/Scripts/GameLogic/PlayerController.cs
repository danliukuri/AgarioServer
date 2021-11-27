using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Properties
    public Vector2 TargetPosition { get; set; }
    #endregion

    #region Fields
    Player player;
    float speed;
    const float defaultSpeed = 5f;
    #endregion

    #region Methods
    void Awake()
    {
        player = GetComponent<Player>();
        speed = defaultSpeed;
    }
    public void FixedUpdate()
    {
        Move(TargetPosition);
    }
    public void Reset()
    {
        transform.position = TargetPosition = Vector3.zero;
        speed = defaultSpeed;
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