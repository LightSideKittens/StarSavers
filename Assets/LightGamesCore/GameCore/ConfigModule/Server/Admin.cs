#if UNITY_EDITOR
using System;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using UnityEditor;
using UnityEngine;

namespace Core.ConfigModule
{
    [InitializeOnLoad]
    public static class Admin
    {
        public static FirebaseFirestore Firestore { get; }
        public static FirebaseApp App { get; }
        private static readonly FirebaseAuth auth;
        private static bool isSingedIn;
        
        static Admin()
        {
            App = FirebaseApp.Create(FirebaseApp.DefaultInstance.Options, "FIREBASE_EDITOR");
            Firestore = FirebaseFirestore.GetInstance(App);
            auth = FirebaseAuth.GetAuth(App);
        }
        
        public static void SignIn(Action onSuccess, Action onError = null)
        {
            if (isSingedIn)
            {
                onSuccess();
                return;
            }
            
            auth.SignInWithEmailAndPasswordAsync("firebase.admin@beatroyale.com", "firebaseadminbeatroyale")
                .ContinueWithOnMainThread(task =>
                {
                    if (task.IsCompletedSuccessfully)
                    {
                        isSingedIn = true;
                        onSuccess();
                    }
                    else
                    {
                        onError?.Invoke();
                        Debug.LogError($"Failure Sign In. Error: {task.Exception.Message}");
                    }
                });
        }
    }
}
#endif