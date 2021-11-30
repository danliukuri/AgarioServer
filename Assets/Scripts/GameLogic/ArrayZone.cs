class ArrayZone
{
    #region Methods
    static int GetExtendedMinIndex(int index, int expansionMagnitude) =>
        index < expansionMagnitude ? 0 : index - expansionMagnitude;
    static int GetExtendedMaxIndex(int index, int expansionMagnitude, int maxArrayLength) =>
        index >= maxArrayLength - expansionMagnitude ? maxArrayLength - 1 : index + expansionMagnitude;
    public static ((int Min, int Max) Hight, (int Min, int Max) Width) GetExtendedZone(
           ((int Min, int Max) Hight, (int Min, int Max) Width) zone,
           int expansionMagnitude, (int Hight, int Width) arrayDimensions)
    {
        ((int Min, int Max) Hight, (int Min, int Max) Width) extendedZone;
        extendedZone.Hight.Min = GetExtendedMinIndex(zone.Hight.Min, expansionMagnitude);
        extendedZone.Hight.Max = GetExtendedMaxIndex(zone.Hight.Max, expansionMagnitude, arrayDimensions.Hight);
        extendedZone.Width.Min = GetExtendedMinIndex(zone.Width.Min, expansionMagnitude);
        extendedZone.Width.Max = GetExtendedMaxIndex(zone.Width.Max, expansionMagnitude, arrayDimensions.Width);
        return extendedZone;
    }
    public static ((int Min, int Max) Hight, (int Min, int Max) Width) GetExtendedZone(
        (int Hight, int Width) zone, int expansionMagnitude, (int Hight, int Width) arrayDimensions) =>
        GetExtendedZone(((zone.Hight, zone.Hight), (zone.Width, zone.Width)), expansionMagnitude, arrayDimensions);
    #endregion
}