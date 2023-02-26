using Firebase.Auth;
using Firebase.Firestore;

namespace Core.ConfigModule
{
    public class RemotePlayerData<T> : DatabaseRemoteConfig<RemotePlayerData<T>, T> where T : BaseConfig<T>, new()
    {
        protected override DocumentReference Reference
        {
            get
            {
                var name = BaseConfig<T>.Config.FileName;
                var storage = FirebaseFirestore.DefaultInstance;
                var userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
                
                return storage.Collection("PlayersData")
                    .Document(userId)
                    .Collection("Data")
                    .Document(name);
            }
        }
    }
}