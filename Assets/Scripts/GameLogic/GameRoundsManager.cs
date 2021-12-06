using UnityEngine;

public class GameRoundsManager : MonoBehaviour
{
    #region Fields
    [SerializeField] float roundDuration;
    Timer timer;
    #endregion

    #region Methods
    void Awake()
    {
        timer = new Timer();
    }
    void Start()
    {
        timer.Run(roundDuration);
    }
    void Update()
    {
        if (timer.Running)
            timer.Update();
        else
        {
            timer.Reset();
            timer.Run(roundDuration);

            ServerPacketsSender.SendListOfTheBestPlayersNamesBySize();
            // TODO: Reset player sizes
        }
    }
    #endregion
}
