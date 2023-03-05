using System;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;

namespace BeatRoyale
{
    public static class Auth
    {
        public static void SignIn(Action onSuccess, Action onError = null)
        {
            if (string.IsNullOrEmpty(CommonPlayerData.UserId))
            {
                FirebaseAuth.DefaultInstance.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
                {
                    if (task.IsCompletedSuccessfully)
                    {
                        CommonPlayerData.UserId = task.Result.UserId;
                        SignInByEmail(true, onSuccess, onError);
                    }
                    else
                    {
                        onError?.Invoke();
                        Debug.LogError($"[{nameof(Auth)}] {task.Exception.Message}");
                    }
                });
            }
            else
            {
                SignInByEmail(false, onSuccess, onError);
            }
        }

        private static void SignInByEmail(bool isNewUser, Action onSuccess, Action onError)
        {
            GetTask(isNewUser).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    onSuccess?.Invoke();
                }
                else
                {
                    onError?.Invoke();
                    Debug.LogError($"[{nameof(Auth)}] {task.Exception.Message}");
                }
            });
        }

        private static Task<FirebaseUser> GetTask(bool isNewUser)
        {
            var email = $"{CommonPlayerData.UserId}@beatroyale.com";
            var password = CommonPlayerData.UserId;
            
            return isNewUser
                ? FirebaseAuth.DefaultInstance.CreateUserWithEmailAndPasswordAsync(email, password)
                : FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(email, password);
        }
    }
}