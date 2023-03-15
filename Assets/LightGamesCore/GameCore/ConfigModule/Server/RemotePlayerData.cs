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
                
                return storage.Collection("PlayersData")
                    .Document(UserId)
                    .Collection("Data")
                    .Document(name);
            }
        }
    }
}