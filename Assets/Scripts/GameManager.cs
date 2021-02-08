using HeroLeft.Interfaces;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HeroLeft.BattleLogic;

namespace HeroLeft
{
    public class GameManager
    {
        public static string SelectedLanguage = "Russian";
        public const string HeroObjectPath = "Hero";

        public static readonly string[] DamageIndicator = new string[] {
            "Prefabs/DamageIndicator",
            "Prefabs/FistDamageIndicator",

        };

        public static readonly string HeroImages = "Sprites/Heroes/";

        public const string MissIndicator = "Prefabs/MissIndicator";
        public const string HealIndicator = "Prefabs/HealIndicator";

        public const float AttackReactionDistance = 200f;

        public static string UserName;
        public static SafeInt Money = 100;
        public static string PlayFabID;

        public static float DeltaXScreen { get { return 1280f / Screen.width; } }
        public static float DeltaYScreen { get { return 720f / Screen.height; } }

        public const float MissChansePerPosition = 10f;

        public static void SetUserData()
        {
            PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
            {
                Data = new Dictionary<string, string>() {
                {"Name", UserName },
        },
                Permission = UserDataPermission.Public,
            },
            null,
            error =>
            {
                Debug.Log(error.GenerateErrorReport());
            });
            PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
            {
                Data = new Dictionary<string, string>() {
                {"Money", Money.ToString()},
        },
                Permission = UserDataPermission.Private,
            },
            result => Debug.Log("Successfully updated user data"),
            error =>
            {
                Debug.Log(error.GenerateErrorReport());
            });
        }

        public static void GetUserData(string myPlayFabeId = null)
        {
            PlayFabClientAPI.GetUserData(new GetUserDataRequest()
            {
                PlayFabId = (myPlayFabeId == null) ? PlayFabID : myPlayFabeId,
                Keys = null
            }, result =>
            {
                Debug.Log("Got user data");

                GetVal(result, "Name", ref UserName);
                GetVal(result, "Money", ref Money);

            }, (error) =>
            {
                Debug.Log("Got error retrieving user data:");
                Debug.Log(error.GenerateErrorReport());
            });
        }

        public static void GetVal(GetUserDataResult res, string key, ref SafeInt val)
        {
            if (res.Data == null || !res.Data.ContainsKey(key))
            {
                Debug.Log("Have not " + key);
            }
            else
            {
                val = int.Parse(res.Data[key].Value);
            }
        }

        public static void GetVal(GetUserDataResult res, string key, ref string val)
        {
            if (res.Data == null || !res.Data.ContainsKey(key))
            {
                Debug.Log("Have not " + key);
            }
            else
            {
                val = res.Data[key].Value;
            }
        }
    }
}
