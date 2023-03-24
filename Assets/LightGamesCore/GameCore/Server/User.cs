using System;
using System.Threading.Tasks;
using Core.ConfigModule;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using Firebase.Storage;
using Newtonsoft.Json;
using Random = UnityEngine.Random;

namespace Core.Server
{
    public class User : JsonBaseConfigData<User>
    {
        public const long MaxAllowedSize = 4 * 1024 * 1024;
        [JsonProperty] private string id = "undefined";
        [JsonProperty] private string nickName = "No Name";
        private static DocumentReference playersCountRef;

        private static string[] nickNames = new[]
        {
            "Captain Crunchwrap",
            "Sir Spam-a-lot",
            "Princess Pudding pop",
            "Count Quackula",
            "Duke of Deliciousness",
            "Lady Lollygag",
            "Baron Von Burrito",
            "The Great Gatsbyburger",
            "Dr. Doughnutstein",
            "The Burgermeister",
        };

        public static string Id
        {
            get => Config.id;
            set
            {
                var config = Config;
                config.id = value;
                config.nickName = nickNames[Random.Range(0, nickNames.Length)];
            }
        }
        
        public static string NickName
        {
            get => Config.nickName;
            set => Config.nickName = value;
        }
        
        public static long PlayersCount { get; private set; }
        
        public static bool ServerEnabled
        {
            get
            {
#if DEBUG
                return DebugData.Config.serverEnabled;
#else
                return true;
#endif
            }
        }
        
        public static FirebaseAuth Auth => FirebaseAuth.DefaultInstance;
        public static FirebaseFirestore Database => FirebaseFirestore.DefaultInstance;
        public static FirebaseStorage Storage => FirebaseStorage.DefaultInstance;

        protected override void OnLoading()
        {
            base.OnLoading();
            playersCountRef = Database.Collection("PlayersData").Document("TotalCount");
            OnAppPause.Subscribe(Dispose);
        }

        public static void Dispose()
        {
            FirebaseApp.DefaultInstance.Dispose();
            OnAppPause.UnSubscribe(Dispose);
        }

        public static void SignIn(Action onSuccess, Action onError = null)
        {
            var userId = Id;
            if (string.IsNullOrEmpty(userId))
            {
                var utc = DateTime.UtcNow;
                Id = $"Y{utc.Year}M{utc.Month}D{utc.Day}h{utc.Hour}m{utc.Minute}s{utc.Second}ms{utc.Millisecond}";
                SignInByEmail(true, onSuccess, onError);
            }
            else if(Auth.CurrentUser == null)
            {
                SignInByEmail(false, onSuccess, () =>
                {
                    SignInByEmail(true, onSuccess, onError);
                });
            }
            else
            {
                onSuccess.SafeInvoke();
            }
        }

        public static void GetPlayersCount(Action onSuccess, int increase = 0, Action onError = null)
        {
            long value = 0;
            Database.RunTransactionAsync(transaction =>
            {
                value = 0;
                return transaction.GetSnapshotAsync(playersCountRef).ContinueWithOnMainThread(snapshotTask =>
                {
                    if (snapshotTask.Result.TryGetValue("count", out value))
                    {
                        value += increase;
                        transaction.Update(playersCountRef, Data.Create("count", value));
                    }
                    else
                    {
                        value += increase;
                        transaction.Set(playersCountRef, Data.Create("count", value));
                    }
                });
            }).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    PlayersCount = value;
                    onSuccess.SafeInvoke();
                }
                else
                {
                    Burger.Log($"[{nameof(User)}] Failure Get Players Count: {task.Exception.Message}");
                    onError.SafeInvoke();
                }
            });
        }

        private static void OnCreated(Action onSuccess, Action onError)
        {
            GetPlayersCount(() =>
            {
                Leaderboards.Position = PlayersCount;
                Leaderboards.Rank = 0;
                Burger.Log($"[{nameof(User)}] Players Count Updated: {PlayersCount}");
                onSuccess.SafeInvoke();
            }, 1, onError);
        }

        private static void SignInByEmail(bool isNewUser, Action onSuccess, Action onError)
        {
            GetTask(isNewUser).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    if (isNewUser)
                    {
                        Burger.Log($"[{nameof(User)}] New user created! UserId: {Id}");
                        OnCreated(onSuccess, onError);
                    }
                    else
                    {
                        Burger.Log($"[{nameof(User)}] Sign In! UserId: {Id}");
                        onSuccess.SafeInvoke();
                    }
                }
                else
                {
                    Burger.Error($"[{nameof(User)}] {task.Exception.Message}");
                    onError.SafeInvoke();
                }
            });
        }

        private static Task<FirebaseUser> GetTask(bool isNewUser)
        {
            var email = $"{Id}@beatroyale.com";
            var password = Id;

            return isNewUser
                ? Auth.CreateUserWithEmailAndPasswordAsync(email, password)
                : Auth.SignInWithEmailAndPasswordAsync(email, password);
        }
    }
}