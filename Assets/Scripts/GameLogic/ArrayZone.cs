class ArrayZone
{
    #region Methods
    static int GetExtendedMinIndex(int index, int expansionMagnitude) =>
        index < expansionMagnitude ? 0 : index - expansionMagnitude;
    static int GetExtendedMaxIndex(int index, int expansionMagnitude, int maxArrayLength) =>
        index >= maxArrayLength - expansionMagnitude ? maxArrayLength - 1 : index + expansionMagnitude;
    public static ((int Min, int Max) Height, (int Min, int Max) Width) GetExtendedZone(
           ((int Min, int Max) Height, (int Min, int Max) Width) zone,
           int expansionMagnitude, (int Height, int Width) arrayDimensions)
    {
        ((int Min, int Max) Height, (int Min, int Max) Width) extendedZone;
        extendedZone.Height.Min = GetExtendedMinIndex(zone.Height.Min, expansionMagnitude);
        extendedZone.Height.Max = GetExtendedMaxIndex(zone.Height.Max, expansionMagnitude, arrayDimensions.Height);
        extendedZone.Width.Min = GetExtendedMinIndex(zone.Width.Min, expansionMagnitude);
        extendedZone.Width.Max = GetExtendedMaxIndex(zone.Width.Max, expansionMagnitude, arrayDimensions.Width);
        return extendedZone;
    }
    public static ((int Min, int Max) Height, (int Min, int Max) Width) GetExtendedZone(
        (int Height, int Width) zone, int expansionMagnitude, (int Height, int Width) arrayDimensions) =>
        GetExtendedZone(((zone.Height, zone.Height), (zone.Width, zone.Width)), expansionMagnitude, arrayDimensions);
    #endregion
}