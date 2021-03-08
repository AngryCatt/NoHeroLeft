using PlayFab;
using PlayFab.ClientModels;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace HeroLeft.Auth {
    public class PlayFabAuth : MonoBehaviour {
        [Header("Registration Fields")]
        public InputField NameField;
        public InputField EmailField;
        public InputField PassField;
        public InputField ConfirmPassField;

        [Header("Login Fields")]
        public InputField lgEmailField;
        public InputField lgPassField;

        [Header("Recover Fields")]
        public InputField rcEmailField;

        public ErrorText errorText;

        bool registered = false;

        public static string EncryptPass(string pass) {
            MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();

            byte[] bs = Encoding.UTF8.GetBytes(pass);
            bs = provider.ComputeHash(bs);

            StringBuilder stringBuilder = new StringBuilder();

            foreach (byte b in bs) {
                stringBuilder.Append(b.ToString("x2").ToLower());
            }
            return stringBuilder.ToString();
        }


        public void Register() {
            if (!ConfirmRegistration()) return;
            string nm = EmailField.text.Remove(EmailField.text.IndexOf('@'));
            if (nm.Length > 16) nm.Remove(16);
            var registerRequest = new RegisterPlayFabUserRequest {
                Email = EmailField.text,
                Password = EncryptPass(PassField.text),
                Username = nm,
                
            };
            GameManager.UserName = NameField.text;

            lgEmailField.text = EmailField.text;
            lgPassField.text = PassField.text;

            PlayFabClientAPI.RegisterPlayFabUser(registerRequest, RegisterSuccess, RegisterFailure);
            transform.GetChild(1).gameObject.SetActive(false);
        }

        private void RegisterSuccess(RegisterPlayFabUserResult result) {
            Debug.Log("Register success!");
            registered = true;

            GameManager.SetUserData();
            Login();
        }

        private void RegisterFailure(PlayFabError error) {
            Debug.Log("Register failed! " + error);
            errorText.SendError(error.ToString());
            transform.GetChild(1).gameObject.SetActive(true);
        }

        public void Login() {
            var loginRequest = new LoginWithEmailAddressRequest {
                Email = lgEmailField.text,
                Password = EncryptPass(lgPassField.text)
            };

            PlayFabClientAPI.LoginWithEmailAddress(loginRequest, LoginSuccess, LoginFailure);
            transform.GetChild(2).gameObject.SetActive(false);
        }

        private void LoginSuccess(LoginResult result) {
            Debug.Log("Login success!");
            AuthController.FileReWrite(lgEmailField.text, lgPassField.text, (System.DateTime)result.LastLoginTime, result.PlayFabId);
            gameObject.SetActive(false);
            if(registered)
                MenuController.menuController.StartTraining();
        }

        private void LoginFailure(PlayFabError error) {
            Debug.Log("Login failed! " + error);
            errorText.SendError("Неверный логин или пароль!");
            transform.GetChild(2).gameObject.SetActive(true);
        }

        public void RestorePassword() {
            PlayFabClientAPI.SendAccountRecoveryEmail(new SendAccountRecoveryEmailRequest {
                Email = rcEmailField.text,
                TitleId = "a4e25",
            }, result => {
                Debug.Log("Success");
            }, error => {
                Debug.Log("Restore Password error! " + error);
                errorText.SendError("Restore Password error!");
            });
        }

        public bool ConfirmRegistration() {

            if (EmailField.text.Length == 0 || !EmailField.text.Contains("@")) {
                errorText.SendError("Данной почты не существует!");
                return false;
            }
            if (NameField.IsActive() && NameField.text.Length < 3) {
                errorText.SendError("Имя пользователя не может быть короче 3-х символов!");
                return false;
            }
            if (PassField.text.Length < 6) {
                errorText.SendError("Пароль должен содержать более 6-ти символов!");
                return false;
            }
            if (ConfirmPassField.IsActive() && PassField.text != ConfirmPassField.text) {
                errorText.SendError("Пароли не совпадают!");
                return false;
            }

            return true;
        }
    }
}
