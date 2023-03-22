using Core.Server;
using Firebase.Firestore;

namespace Core.ConfigModule
{
    public class UserDatabase<T> : DatabaseRemoteConfig<UserDatabase<T>, T> where T : BaseConfig<T>, new()
    {
        protected override DocumentReference Reference
        {
            get
            {
                var name = BaseConfig<T>.Config.FileName;
                var database = User.Database;
                
                return database.Collection("PlayersData")
                    .Document(UserId)
                    .Collection("Data")
                    .Document(name);
            }
        }
    }
}