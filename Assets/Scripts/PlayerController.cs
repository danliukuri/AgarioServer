using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerController : MonoBehaviour
{
    public Vector2 TargetPosition { get; set; }

    Player player;
    float speed = 5f;

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

        ServerSend.PlayerMovement(player.Id, newPosition);
    }
}
