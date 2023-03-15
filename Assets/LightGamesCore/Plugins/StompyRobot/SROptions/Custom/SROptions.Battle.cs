using System.ComponentModel;
using Battle.Windows;

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
}