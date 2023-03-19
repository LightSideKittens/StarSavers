using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.ConfigModule;
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
        [JsonProperty] private string id;
        [JsonProperty] private string nickName;
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
        
        public static FirebaseAuth Auth => FirebaseAuth.DefaultInstance;
        public static FirebaseFirestore Database => FirebaseFirestore.DefaultInstance;
        public static FirebaseStorage Storage => FirebaseStorage.DefaultInstance;

        protected override void OnLoading()
        {
            base.OnLoading();
            playersCountRef = Database.Collection("PlayersData").Document("TotalCount");
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

        public static void GetPlayersCount(Action<Transaction, long> onSuccess, Action onError = null)
        { 
            Database.RunTransactionAsync(transaction =>
            {
                return transaction.GetSnapshotAsync(playersCountRef).ContinueWithOnMainThread(snapshotTask =>
                {
                    if (snapshotTask.IsCompletedSuccessfully)
                    {
                        if (snapshotTask.Result.TryGetValue<long>("count", out var value))
                        {
                            onSuccess.SafeInvoke(transaction, value);
                        }
                        else
                        {
                            transaction.Set(playersCountRef, Data.Create("count", value));
                            onSuccess.SafeInvoke(transaction, value);
                        }
                    }
                    else
                    {
                        Burger.Log($"[{nameof(User)}] Failure Get Players Count: {snapshotTask.Exception.Message}");
                        onError.SafeInvoke();
                    }
                });
            });
        }

        private static void OnCreated(Action onSuccess, Action onError)
        {
            GetPlayersCount((transaction, count) =>
            {
                long newValue = count + 1;
                transaction.Set(playersCountRef, Data.Create("count", newValue));
                
                Leaderboards.Position = newValue;
                Leaderboards.Rank = 0;
                Burger.Log($"[{nameof(User)}] Players Count Updated: {newValue}");
                onSuccess.SafeInvoke();
            }, onError);
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