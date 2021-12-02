using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Properties
    public Vector2 TargetPosition { get; set; }
    public float Speed { get => speed; set => speed = value > lowerSpeedLimit ? value : lowerSpeedLimit; }
    #endregion

    #region Fields
    [SerializeField] float startSpeed;
    [SerializeField] float lowerSpeedLimit;
    
    Player player;
    float speed;
    #endregion

    #region Methods
    void Awake()
    {
        player = GetComponent<Player>();
        speed = startSpeed;
    }
    public void FixedUpdate()
    {
        Move(TargetPosition);
    }
    public void Reset()
    {
        transform.position = TargetPosition = Vector3.zero;
        speed = startSpeed;
    }

    void Move(Vector2 position)
    {
        Vector2 newPosition = transform.position = Vector2.MoveTowards(
            transform.position,
            position,
            speed * Time.fixedDeltaTime);

        ServerPacketsSender.PlayerMovementForVisiblePlayers(player, newPosition);
    }
    #endregion
}