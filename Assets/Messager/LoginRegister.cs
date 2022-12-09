using Core.Extensions.Unity;
using Firebase.Auth;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BeatRoyale
{
    public class LoginRegister : MonoBehaviour
    {
        [SerializeField] private Messager messager;
        [SerializeField] private TMP_InputField loginField;
        [SerializeField] private TMP_InputField passwordField;
        [SerializeField] private TMP_InputField confirmPasswordField;
        
        [SerializeField] private Button actionButton;
        [SerializeField] private bool isLogin = true;

        private void Start()
        {
            actionButton.interactable = false;
            
            if (isLogin)
            {
                actionButton.AddListener(Login);
                loginField.onValueChanged.AddListener(CheckLoginPossible);
                passwordField.onValueChanged.AddListener(CheckLoginPossible);
            }
            else
            {
                actionButton.AddListener(Register);
                loginField.onValueChanged.AddListener(CheckRegisterPossible);
                passwordField.onValueChanged.AddListener(CheckRegisterPossible);
                confirmPasswordField.onValueChanged.AddListener(CheckRegisterPossible);
            }
        }

        private async void Login()
        {
            var auth = FirebaseAuth.DefaultInstance;
            var request = auth.SignInWithEmailAndPasswordAsync(loginField.text, passwordField.text);
            var user = await request;
            
            if (request.Exception != null)
            {
                Debug.Log($"[Firebase Auth] Exception: {request.Exception}");
            }
            else
            {
                Debug.Log($"[Firebase Auth] Success Auth! Email: {user.Email}");
                messager.OnLogin(user.UserId);
                gameObject.SetActive(false);
            }
        }
        
        private async void Register()
        {
            var auth = FirebaseAuth.DefaultInstance;
            var request = auth.CreateUserWithEmailAndPasswordAsync(loginField.text, passwordField.text);
            var user = await request;
            
            if (request.Exception != null)
            {
                Debug.Log($"[Firebase Auth] Exception: {request.Exception}");
            }
            else
            {
                Debug.Log($"[Firebase Auth] Success Auth! Email: {user.Email}");
                messager.OnLogin(user.UserId);
                gameObject.SetActive(false);
            }
        }
        
        private void CheckLoginPossible(string s)
        {
            actionButton.interactable =
                string.IsNullOrEmpty(loginField.text) == false
                && string.IsNullOrEmpty(passwordField.text) == false;
        }

        private void CheckRegisterPossible(string s)
        {
            actionButton.interactable = 
                string.IsNullOrEmpty(loginField.text) == false 
                && string.IsNullOrEmpty(confirmPasswordField.text) == false
                && string.IsNullOrEmpty(passwordField.text) == false
                && confirmPasswordField.text == passwordField.text;
        }
    }
}
