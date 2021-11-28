using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Fields
    [SerializeField] Field field;
    #endregion

    #region Methods
    void Start()
    {
        field.Generate();
    }
    #endregion
}