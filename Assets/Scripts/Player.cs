using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class Player : MonoBehaviour
{
    public int Id { get; private set; }
    public string Username { get; private set; }
    public PlayerController Controller { get; private set; }
    

    public void Initialize(int id, string username)
    {
        Id = id;
        Username = username;
    }

    void Awake()
    {
        Controller = GetComponent<PlayerController>();
    }
}