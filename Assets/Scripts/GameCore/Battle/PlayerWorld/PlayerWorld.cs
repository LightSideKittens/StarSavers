using Core.Server;

namespace Battle
{
    public class PlayerWorld : BasePlayerWorld<PlayerWorld>
    {
        private void Start()
        {
            UserId = User.Id;
        }
    }
}