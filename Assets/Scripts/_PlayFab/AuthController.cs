using PlayFab;
using PlayFab.ClientModels;
using System;
using System.IO;
using System.Net.NetworkInformation;
using System.Threading;
using UnityEngine;

namespace HeroLeft.Auth {

    public class AuthController : MonoBehaviour {

        public bool EnterWithOutCheck = false;

        public static bool HaveAuthorized = false;
        public static bool HaveInternetConnection = false;

        public static string Password;
        public static string Email;

        public static string infoFilePath = "info.nhl";
        public static DateTime startTime;

        public string MenuSceanName;
        public GameObject InternetErrorPanel;

        public void Start() {
            InternetErrorPanel.SetActive(false);
            HaveAuthorized = CheckInfoFile();

            if (CheckServer("www.google.com") || EnterWithOutCheck) {
                HaveInternetConnection = true;
            }

            if (HaveInternetConnection && HaveAuthorized) {
                ParseAuthInfo();
                AutoLogin(Email, Password);
            } else if (!HaveAuthorized && !HaveInternetConnection) {
                InternetErrorPanel.SetActive(true);
                Debug.Log("ПОДКЛЮЧИТЕСЬ К ИНЕТУ");
            } else {
                UnityEngine.SceneManagement.SceneManager.LoadScene(MenuSceanName);
            }

        }

        public void RightEnter()
        {
            EnterWithOutCheck = true;
            Start();
        }

        public bool CheckInfoFile() {
            if (!File.Exists(Application.persistentDataPath + "/" + infoFilePath)) {
                File.Create(Application.persistentDataPath + "/" + infoFilePath);
                return false;
            }
            string info = File.ReadAllText(Application.persistentDataPath + "/" + infoFilePath);
            if (info.Length == 0) return false;
            return true;
        }


        public static void FileReWrite(string Email, string Password, DateTime startTime, string id) {
            if (!File.Exists(Application.persistentDataPath + "/" + infoFilePath)) throw new Exception("Файл сохранения не найден");
            File.WriteAllLines(Application.persistentDataPath + "/" + infoFilePath, new string[] { Email, Password, startTime.ToString() });
            GameManager.PlayFabID = id;
            HaveAuthorized = true;
            GameManager.GetUserData();
        }
        public static void FileReWrite(string Email, string Password, string startTime, string id) {
            if (!File.Exists(Application.persistentDataPath + "/" + infoFilePath)) throw new Exception("Файл сохранения не найден");
            File.WriteAllLines(Application.persistentDataPath + "/" + infoFilePath, new string[] { Email, Password, startTime });
            GameManager.PlayFabID = id;
            HaveAuthorized = true;
            GameManager.GetUserData();
        }

        public static void ParseAuthInfo() {
            try {
                string[] info = File.ReadAllLines(Application.persistentDataPath + "/" + infoFilePath);

                Email = info[0];
                Password = info[1];
                if(info[2].Length > 0)
                startTime = DateTime.Parse(info[2]);
            } catch (Exception e) {
                Debug.LogError(e);
                File.Delete(Application.persistentDataPath + "/" + infoFilePath);
                Application.Quit();
            }
        }

        public static bool CheckServer(string url) {
            try {
                System.Net.NetworkInformation.Ping p = new System.Net.NetworkInformation.Ping();
                PingReply pr = p.Send(url);
                IPStatus status = pr.Status;
                if (status == IPStatus.Success) {
                    return true;
                }
            } catch {
                return false;
            }

            return false;
        }


        public void AutoLogin(string mail, string password) {
            var loginRequest = new LoginWithEmailAddressRequest {
                Email = mail,
                Password = PlayFabAuth.EncryptPass(password)
            };

            PlayFabClientAPI.LoginWithEmailAddress(loginRequest, LoginSuccess, LoginFailure);
        }

        private void LoginSuccess(LoginResult result) {
            try {
                Debug.Log("Login success!");
                DateTime startTime = (DateTime)result.LastLoginTime;
                FileReWrite(Email, Password, startTime, result.PlayFabId);
            } catch(Exception e) {
                File.Delete(Application.persistentDataPath + "/" + infoFilePath);
            }
            UnityEngine.SceneManagement.SceneManager.LoadScene(MenuSceanName);
        }

        private void LoginFailure(PlayFabError error) {
            HaveAuthorized = false;
            Debug.Log("Login failed! " + error);
            File.Delete(Application.persistentDataPath + "/" + infoFilePath);
            UnityEngine.SceneManagement.SceneManager.LoadScene(MenuSceanName);
        }
    }
}
