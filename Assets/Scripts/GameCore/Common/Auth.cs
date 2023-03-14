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
            var userId = CommonPlayerData.UserId;
            if (string.IsNullOrEmpty(userId))
            {
                FirebaseAuth.DefaultInstance.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
                {
                    if (task.IsCompletedSuccessfully)
                    {
                        userId = task.Result.UserId;
                        CommonPlayerData.UserId = userId;
                        Debug.Log($"[{nameof(Auth)}] Created New Account. UserId: {userId}");
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
                    Debug.Log($"[{nameof(Auth)}] Sign In. UserId: {CommonPlayerData.UserId}");
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