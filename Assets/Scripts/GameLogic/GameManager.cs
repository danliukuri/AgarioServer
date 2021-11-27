using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Fields
    [SerializeField] Field fieldGenerator;
    #endregion

    #region Methods
    void Start()
    {
        fieldGenerator.Generate();
    }
    #endregion
}