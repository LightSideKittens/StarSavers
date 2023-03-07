namespace FastSpriteMask
{
    public interface IMultiMask : IMask
    {
        //MaskResolution Resolution { get; }
        int DataIndex { get; set; }

        IMultiMaskData MultiMaskData { get; set; }
    }
}
