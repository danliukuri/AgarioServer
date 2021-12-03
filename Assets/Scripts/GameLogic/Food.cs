using UnityEngine;

public class Food : MonoBehaviour
{
    #region Properties
    public float SizeChange { get => sizeChange; }
    public float SpeedChange { get => speedChange; }
    public int Id { get; set; }
    public FieldSector FieldSector { get; set; }
    #endregion

    #region Fields
    [SerializeField] float sizeChange;
    [SerializeField] float speedChange;
    #endregion

    #region Methods
    public void Reset()
    {
        Id = default;
        FieldSector.Food.Remove(this);
        FieldSector = default;
    }
    #endregion
}