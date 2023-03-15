using System.ComponentModel;
using Battle.Windows;
using BeatRoyale;

public partial class SROptions 
{
    [Category("Battle")]
    public void Win() 
    {
        MatchResultWindow.Show(true);
    }
    
    [Category("Battle")]
    public void Lose() 
    {
        MatchResultWindow.Show(false);
    }
    
    [Category("Battle")]
    public bool ShowRadius
    {
        get => DebugData.Config.needShowRadius;
        set => DebugData.Config.needShowRadius = value;
    }
}