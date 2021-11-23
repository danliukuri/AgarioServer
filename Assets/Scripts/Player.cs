using UnityEngine;

public class Player : MonoBehaviour
{
    public int Id => id;
    public string Username => username;

    int id;
    string username;

    public void Initialize(int id, string username)
    {
        this.id = id;
        this.username = username;
    }
}