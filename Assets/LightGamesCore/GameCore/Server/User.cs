using System;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using Firebase.Storage;
using UnityEngine;
using static Core.Server.CommonPlayerData;

namespace Core.Server
{
    public static class User
    {
        public static FirebaseAuth Auth => FirebaseAuth.DefaultInstance;
        public static FirebaseFirestore Database => FirebaseFirestore.DefaultInstance;
        public static FirebaseStorage Storage => FirebaseStorage.DefaultInstance;
        
        public static void SignIn(Action onSuccess, Action onError = null)
        {
            var userId = UserId;
            if (string.IsNullOrEmpty(userId))
            {
                var utc = DateTime.UtcNow;
                UserId = $"Y{utc.Year}M{utc.Month}D{utc.Day}h{utc.Hour}m{utc.Minute}s{utc.Second}ms{utc.Millisecond}";
                SignInByEmail(true, onSuccess, onError);
            }
            else if(Auth.CurrentUser == null)
            {
                SignInByEmail(false, onSuccess, ()=> { SignInByEmail(true, onSuccess, onError); });
            }
            else
            {
                onSuccess();
            }
        }

        private static void SignInByEmail(bool isNewUser, Action onSuccess, Action onError)
        {
            GetTask(isNewUser).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    Debug.Log($"[{nameof(User)}] Sign In. UserId: {UserId}");
                    onSuccess?.Invoke();
                }
                else
                {
                    onError?.Invoke();
                    Debug.LogError($"[{nameof(User)}] {task.Exception.Message}");
                }
            });
        }

        private static Task<FirebaseUser> GetTask(bool isNewUser)
        {
            var email = $"{UserId}@beatroyale.com";
            var password = UserId;

            /*if (isNewUser)
            {
                var playersCountRef = FirebaseFirestore.DefaultInstance.Collection("cities").Document("SF");
                FirebaseFirestore.DefaultInstance.RunTransactionAsync(transaction =>
                {
                    return transaction.GetSnapshotAsync(cityRef).ContinueWith((snapshotTask) =>
                    {
                        DocumentSnapshot snapshot = snapshotTask.Result;
                        long newPopulation = snapshot.GetValue<long>("Population") + 1;
                        Dictionary<string, object> updates = new Dictionary<string, object>
                        {
                            { "Population", newPopulation}
                        };
                        transaction.Update(cityRef, updates);
                    });
                });
            }*/

            return isNewUser
                ? Auth.CreateUserWithEmailAndPasswordAsync(email, password)
                : Auth.SignInWithEmailAndPasswordAsync(email, password);
        }
    }
}