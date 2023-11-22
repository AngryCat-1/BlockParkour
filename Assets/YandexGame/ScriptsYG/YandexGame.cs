using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.Events;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace YG
{
    public class YandexGame : MonoBehaviour
    {
        public InfoYG infoYG;
        [Space(10)]
        public UnityEvent ResolvedAuthorization;
        public UnityEvent RejectedAuthorization;
        [Space(30)]
        public UnityEvent OpenFullscreenAd;
        public UnityEvent CloseFullscreenAd;
        [Space(30)]
        public UnityEvent OpenVideoAd;
        public UnityEvent CloseVideoAd;
        public UnityEvent CheaterVideoAd;

        #region Data Fields
        public static bool startGame
        {
            get
            {
                return _startGame;
            }
        }
        public static bool auth
        {
            get
            {
                return _auth;
            }
        }
        public static bool initializedLB
        {
            get
            {
                return _initializedLB;
            }
        }
        public static string playerName
        {
            get
            {
                return _playerName;
            }
            set
            {
                _photoSize = value;
            }
        }
        public static string playerId
        {
            get
            {
                return _playerId;
            }
        }
        public static string playerPhoto
        {
            get
            {
                return _playerPhoto;
            }
            set
            {
                _photoSize = value;
            }
        }
        public static bool adBlock
        {
            get
            {
                return _adBlock;
            }
            set
            {
                _adBlock = value;
            }
        }
        public static string photoSize
        {
            get
            {
                return _photoSize;
            }
            set
            {
                _photoSize = value;
            }
        }

        static bool _startGame;
        static bool _auth;
        static bool _initializedLB;
        static string _playerName = "unauthorized";
        static string _playerId;
        static string _playerPhoto;
        static bool _adBlock;
        static string _photoSize;
        static bool _leaderboardEnable;
        static bool _debug;
        static bool _cloudSaves;
        public static JsonSaves savesData = new JsonSaves();
        public static JsonEnvironmentData EnvironmentData = new JsonEnvironmentData();
        #endregion Data Fields


        // Methods

        private void Awake()
        {
            transform.SetParent(null);
            gameObject.name = "YandexGame";

            _FullscreenShow();
        }
        void AdBlockFalseInvoke()
        {
            _adBlock = false;
        }
        static void Message(string message)
        {
            if (_debug) Debug.Log(message);
        }

        #region First Calls
        void FirstСalls()
        {
            if (!_startGame)
            {
                _debug = infoYG.debug;
                _cloudSaves = infoYG.cloudSaves;
                _leaderboardEnable = infoYG.leaderboardEnable;
                _startGame = true;

                if (infoYG.authorizationEnable)
                {
                    if (infoYG.playerPhotoSize == InfoYG.PlayerPhotoSize.small)
                        _photoSize = "small";
                    else if (infoYG.playerPhotoSize == InfoYG.PlayerPhotoSize.medium)
                        _photoSize = "medium";
                    else if (infoYG.playerPhotoSize == InfoYG.PlayerPhotoSize.large)
                        _photoSize = "large";

                    _AuthorizationCheck();

                    if (_leaderboardEnable)
                        _InitLeaderboard();
                }
                else
                {
                    PlayerDataEvent?.Invoke();
                    LoadLocal();
                }

                _RequestingEnvironmentData();

                if (infoYG.LocalizationEnable &&
                   infoYG.callingLanguageCheck == InfoYG.CallingLanguageCheck.EveryGameLaunch)
                    _LanguageRequest();
            }

            if (savesData.isFirstSession)
            {
                if (infoYG.LocalizationEnable && infoYG.callingLanguageCheck == InfoYG.CallingLanguageCheck.FirstLaunchOnly)
                    _LanguageRequest();
            }
            else Invoke("LanguageEventInvoke", 0.1f);
        }
        #endregion

        #region Language
        public delegate void SwitchLang(string lang);
        public static event SwitchLang SwitchLangEvent;

        public void _SwitchLanguage(string language)
        {
            savesData.language = language;
            SaveProgress();

            SwitchLangEvent?.Invoke(language);
        }

        public static void SwitchLanguage(string language)
        {
            savesData.language = language;
            SaveProgress();

            SwitchLangEvent?.Invoke(language);
        }

        void LanguageEventInvoke()
        {
            SwitchLangEvent?.Invoke(savesData.language);
        }
        #endregion Language

        #region Player Data
        public static Action PlayerDataEvent;
        public static Action LoadedSavesEvent;

        public void GetPlayerData() => PlayerDataEvent?.Invoke();

        public static void SaveLocal()
        {
            if (!Directory.Exists(Application.persistentDataPath + "/SavesYG"))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/SavesYG");
                Message("Save Local: Create New Directory");
            }
            else Message("Save Local");

            FileStream fs = new FileStream(Application.persistentDataPath + "/SavesYG/saveyg.svyg", FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(fs, savesData);
            fs.Close();
        }

        public static void LoadLocal()
        {
            if (_debug) Debug.Log("Load Local");

            if (File.Exists(Application.persistentDataPath + "/SavesYG/saveyg.svyg")) // если файл есть
            {
                FileStream fs = new FileStream(Application.persistentDataPath + "/SavesYG/saveyg.svyg", FileMode.Open);
                BinaryFormatter formatter = new BinaryFormatter();
                try // загрузка
                {
                    savesData = (JsonSaves)formatter.Deserialize(fs);
                    LoadedSavesEvent?.Invoke();
                }
                catch (System.Exception e) // если файл поломан
                {
                    Debug.Log(e.Message);
                    ResetSaveProgress(true);
                }
                finally
                {
                    fs.Close();
                }
            }
            else ResetSaveProgress(true);
        }

        public static Action onMessageLogInEvent;
        public static void ResetSaveProgress(bool massage)
        {
            Message("Reset Save Progress");

            savesData = new JsonSaves { isFirstSession = false };
            LoadedSavesEvent?.Invoke();

            if (!auth && massage)
                onMessageLogInEvent?.Invoke();
        }

        public static void ResetSaveProgress()
        {
            ResetSaveProgress(false);
        }

        public static void SaveProgress()
        {
            if (_auth && _cloudSaves)
                SaveCloud(false);
            else
                SaveLocal();
        }

        public static void LoadProgress()
        {
            if (_auth && _cloudSaves)
                LoadCloud();
            else
                LoadLocal();
        }
        #endregion Player Data        


        // Sending messages

        #region Init Leaderboard
        [DllImport("__Internal")]
        private static extern void InitLeaderboard();

        public void _InitLeaderboard()
        {
            Invoke("InitializedLB", 0.2f);
#if !UNITY_EDITOR
            InitLeaderboard();
#endif
#if UNITY_EDITOR
            Message("Initialization Leaderboards");
#endif
        }
        #endregion Init Leaderboard

        #region Authorization Check
        [DllImport("__Internal")]
        private static extern void AuthorizationCheck(string playerPhotoSize);

        public void _AuthorizationCheck()
        {
#if !UNITY_EDITOR
            AuthorizationCheck( _photoSize);
#endif
#if UNITY_EDITOR
            SetAuthorization(@"{""playerAuth""" + ": " + @"""resolved""," + @"""playerName""" + ": " + @"""Ivan"", " + @"""playerId""" + ": " + @"""tOpLpSh7i8QG8Voh/SuPbeS4NKTj1OxATCTKQF92H4c="", " + @"""playerPhoto""" + ": " + @"""https://avatars.mds.yandex.net/get-yapic/35885/sQA4bpZ5JEWQkyz2x15TzAIO2kg-1/islands-300""}");
#endif
        }
        #endregion Authorization Check

        #region Open Auth Dialog
        [DllImport("__Internal")]
        private static extern void OpenAuthDialog(string playerPhotoSize);

        public void _OpenAuthDialog()
        {
#if !UNITY_EDITOR
            OpenAuthDialog(_photoSize);
#endif
#if UNITY_EDITOR
            Message("Open Auth Dialog");
#endif
        }
        
        public static void AuthDialog()
        {
#if !UNITY_EDITOR
            OpenAuthDialog(_photoSize);
#endif
#if UNITY_EDITOR
            Message("Open Auth Dialog");
#endif
        }
        #endregion Open Auth Dialog

        #region Save end Load Cloud
        [DllImport("__Internal")]
        private static extern void SaveYG(string jsonData, bool flush);

        public static void SaveCloud(bool flush)
        {
#if !UNITY_EDITOR
            if (_auth && _cloudSaves)
                SaveYG(JsonUtility.ToJson(savesData), flush);
#endif
            Message("Save Cloud");
        }

        [DllImport("__Internal")]
        private static extern void LoadYG();

        public static void LoadCloud()
        {
#if !UNITY_EDITOR
            if (_auth && _cloudSaves)
                LoadYG();
#endif
            Message("Load Cloud");
        }
        #endregion Save end Load Cloud

        #region Fullscren Ad Show
        [DllImport("__Internal")]
        private static extern void FullscreenShow();

        public void _FullscreenShow()
        {
            if (timerShowAd >= 65)
            {
                timerShowAd = 0;
#if !UNITY_EDITOR
                FullscreenShow();
#endif
#if UNITY_EDITOR
                Message("Fullscren Ad");
                OpenFullscreen();
                Invoke("CloseFullscreen", 1);
#endif
            }
            else Message("The display of full-screen ads is blocked! It's still early.  (ru) Отображение полноэкранной рекламы заблокировано! Еще рано.");
        }
        #endregion Fullscren Ad Show

        #region Rewarded Video Show
        [DllImport("__Internal")]
        private static extern void RewardedShow(int id);

        static int tempID;
        public void _RewardedShow(int id)
        {
#if !UNITY_EDITOR
            if (infoYG.checkAdblock)
            {
                if (!adBlock)
                {
                    adBlock = true;
                    Invoke("AdBlockFalseInvoke", 10);
                    RewardedShow(id);
                }
            }
            else RewardedShow(id);
#endif
#if UNITY_EDITOR
            Message("Rewarded Ad");

            tempID = id;

            if (infoYG.checkAdblock)
            {
                Message("Cheater!");
                Invoke("TestCheater", 1);
            }
            else
            {
                OpenVideo(id);
                Invoke("TestCloseVideo", 1);
            }
#endif
        }
#if UNITY_EDITOR
        void TestCloseVideo() => CloseVideo(tempID);
        void TestCheater()
        {
            CheaterVideoAd.Invoke();
            CheaterVideoEvent?.Invoke();
        }
#endif
        #endregion Rewarded Video Show

        #region Language Request
        [DllImport("__Internal")]
        private static extern void LanguageRequest();

        public void _LanguageRequest()
        {
#if !UNITY_EDITOR
            LanguageRequest();
#endif
#if UNITY_EDITOR
            SetLanguage("ru");
#endif
        }
        #endregion Language Request

        #region Requesting Environment Data
        [DllImport("__Internal")]
        private static extern void RequestingEnvironmentData();

        public void _RequestingEnvironmentData()
        {
#if !UNITY_EDITOR
            RequestingEnvironmentData();
#endif
#if UNITY_EDITOR
            EnvirDataEvent?.Invoke();
#endif
            Message("Requesting Environment Data");
        }
        #endregion Requesting Environment Data

        #region URL
        public void _OnURL(string url)
        {
            Application.OpenURL("https://yandex." + EnvironmentData.domain + "/games/" + url);
            Message("URL");
        }

        public void _OnURL_ru(string url)
        {
            Application.OpenURL("https://yandex.ru/games/" + url);
            Message("URL.ru");
        }
#endregion URL

        #region Leaderboard
        [DllImport("__Internal")]
        private static extern void SetLeaderboardScores(string nameLB, int score);

        public static void NewLeaderboardScores(string nameLB, int score)
        {
#if !UNITY_EDITOR
        if (_leaderboardEnable) 
            SetLeaderboardScores(nameLB, score);
#endif
            if (_leaderboardEnable)
                Message("New Scores Leaderboard " + nameLB + ": " + score);
        }

        [DllImport("__Internal")]
        private static extern void GetLeaderboardScores(string nameLB, int maxQuantityPlayers, int quantityTop, int quantityAround, string photoSizeLB);

        public static void GetLeaderboard(string nameLB, int maxQuantityPlayers, int quantityTop, int quantityAround, string photoSizeLB)
        {
            int[] rank = new int[3];
            string[] photo = new string[3];
            string[] playersName = new string[3];
            int[] scorePlayers = new int[3];

#if !UNITY_EDITOR
            if (_leaderboardEnable)
            {
                GetLeaderboardScores(nameLB, maxQuantityPlayers, quantityTop, quantityAround, photoSizeLB);
            }
            else
            {
                rank = new int[1];
                photo = new string[1];
                playersName = new string[1];
                scorePlayers = new int[1];

                UpdateLbEvent?.Invoke(nameLB, "No data", rank, photo, playersName, scorePlayers, auth);
            }
#endif
#if UNITY_EDITOR
            if (_leaderboardEnable)
            {
                rank[0] = 1; rank[1] = 2; rank[2] = 3;
                photo[0] = "https://avatars.mds.yandex.net/get-yapic/35885/sQA4bpZ5JEWQkyz2x15TzAIO2kg-1/islands-300";
                photo[1] = photo[0]; photo[2] = photo[0];
                playersName[0] = "Player"; playersName[1] = "Ivan"; playersName[2] = "Maria";
                scorePlayers[0] = 23; scorePlayers[1] = 115; scorePlayers[2] = 1053;

                UpdateLbEvent?.Invoke(nameLB, $"Test LeaderBoard\nName: {nameLB}\n1. Player: 10\n2. Ivan: 15\n3. Maria: 23",
                    rank, photo, playersName, scorePlayers, true);
            }
            else
            {
                rank = new int[1];
                rank[0] = 0;
                playersName[0] = "No data";

                UpdateLbEvent?.Invoke(nameLB, "No data", rank, photo, playersName, scorePlayers, auth);
            }
            Message("Get Leaderboard - " + nameLB);
#endif
        }
        #endregion Leaderboard

        #region Review
        [DllImport("__Internal")]
        private static extern void Review();

        public void _Review()
        {
#if !UNITY_EDITOR
            if ( _auth || !infoYG.authorizationEnable)
              Review();
          else
              _OpenAuthDialog();
#endif
            Message("Review");
        }
        #endregion Review


        // Receiving messages

        #region Ads
        public void OpenFullscreen()
        {
            OpenFullscreenAd.Invoke();
        }

        public delegate void CloseFullAd();
        public static event CloseFullAd CloseFullAdEvent;
        public void CloseFullscreen()
        {
            CloseFullscreenAd.Invoke();
            CloseFullAdEvent?.Invoke();
        }

        public delegate void OpenRewAd(int id);
        public static event OpenRewAd OpenVideoEvent;

        public void OpenVideo(int id)
        {
            OpenVideoEvent?.Invoke(id);
            OpenVideoAd.Invoke();
        }

        public delegate void CloseRewAd(int id);
        public static event CloseRewAd CloseVideoEvent;

        public delegate void CheaterReward();
        public static event CheaterReward CheaterVideoEvent;

        public void CloseVideo(int id)
        {
            if (infoYG.checkAdblock && _adBlock)
            {
                CheaterVideoAd.Invoke();
                CheaterVideoEvent?.Invoke();

                CancelInvoke("spawnInvoke");
                _adBlock = false;
            }
            else
            {
                CloseVideoAd.Invoke();
                CloseVideoEvent?.Invoke(id);
            }
        }
        #endregion Ads

        #region Authorization
        JsonAuth jsonAuth = new JsonAuth();

        public void SetAuthorization(string data)
        {
            jsonAuth = JsonUtility.FromJson<JsonAuth>(data);

            if (jsonAuth.playerAuth.ToString() == "resolved")
            {
                ResolvedAuthorization.Invoke();
                _auth = true;
            }
            else if (jsonAuth.playerAuth.ToString() == "rejected")
            {
                RejectedAuthorization.Invoke();
                _auth = false;
            }

            _playerName = jsonAuth.playerName.ToString();
            _playerId = jsonAuth.playerId.ToString();
            _playerPhoto = jsonAuth.playerPhoto.ToString();

            Message("Authorization - " + jsonAuth.playerAuth.ToString());

            Invoke("GetPlayerData", 0.1f);
            LoadProgress();
        }
        #endregion Set Authorization

        #region Loading progress
        public void SetLoadSaves(string data)
        {
            data = data.Remove(0, 2);
            data = data.Remove(data.Length - 2, 2);
            data = data.Replace(@"\", "");

            savesData = JsonUtility.FromJson<JsonSaves>(data);
            Message("Load YG Complete");

            LoadedSavesEvent?.Invoke();
        }

        public void ResetSaveCloud()
        {
            Message("Reset Save Progress");
            savesData = new JsonSaves { isFirstSession = false };
            SaveProgress();
            LoadedSavesEvent?.Invoke();
        }
        #endregion Loading progress

        #region Language
        public void SetLanguage(string language)
        {
            string lang = "en";

            switch (language)
            {
                case "ru":
                    if (infoYG.languages.ru)
                        lang = language;
                    break;
                case "en":
                    if (infoYG.languages.en)
                        lang = language;
                    break;
                case "tr":
                    if (infoYG.languages.tr)
                        lang = language;
                    else lang = "ru";
                    break;
                case "az":
                    if (infoYG.languages.az)
                        lang = language;
                    else lang = "en";
                    break;
                case "be":
                    if (infoYG.languages.be)
                        lang = language;
                    else lang = "ru";
                    break;
                case "he":
                    if (infoYG.languages.he)
                        lang = language;
                    else lang = "en";
                    break;
                case "hy":
                    if (infoYG.languages.hy)
                        lang = language;
                    else lang = "en";
                    break;
                case "ka":
                    if (infoYG.languages.ka)
                        lang = language;
                    else lang = "en";
                    break;
                case "et":
                    if (infoYG.languages.et)
                        lang = language;
                    else lang = "en";
                    break;
                case "fr":
                    if (infoYG.languages.fr)
                        lang = language;
                    else lang = "en";
                    break;
                case "kk":
                    if (infoYG.languages.kk)
                        lang = language;
                    else lang = "ru";
                    break;
                case "ky":
                    if (infoYG.languages.ky)
                        lang = language;
                    else lang = "en";
                    break;
                case "lt":
                    if (infoYG.languages.lt)
                        lang = language;
                    else lang = "en";
                    break;
                case "lv":
                    if (infoYG.languages.lv)
                        lang = language;
                    else lang = "en";
                    break;
                case "ro":
                    if (infoYG.languages.ro)
                        lang = language;
                    else lang = "en";
                    break;
                case "tg":
                    if (infoYG.languages.tg)
                        lang = language;
                    else lang = "en";
                    break;
                case "tk":
                    if (infoYG.languages.tk)
                        lang = language;
                    else lang = "en";
                    break;
                case "uk":
                    if (infoYG.languages.uk)
                        lang = language;
                    else lang = "ru";
                    break;
                case "uz":
                    if (infoYG.languages.uz)
                        lang = language;
                    else lang = "ru";
                    break;
                default:
                    lang = "en";
                    break;
            }

            if (lang == "en" && !infoYG.languages.en)
                lang = "ru";
            else if (lang == "ru" && !infoYG.languages.ru)
                lang = "en";

            //if (infoYG.languageType == InfoYG.LanguageType.RuEndEn)
            //{
            //    if (language == "ru" || language == "be" || language == "kk" || language == "uk" || language == "uz")
            //        lang = "ru";
            //    else 
            //        lang = "en";
            //}
            //else if (infoYG.languageType == InfoYG.LanguageType.RuTrEn)
            //{
            //    if (language == "ru" || language == "be" || language == "kk" || language == "uk" || language == "uz")
            //        lang = "ru";
            //    else if (language == "tr")
            //        lang = "tr";
            //    else
            //        lang = "en";
            //}
            //else if (infoYG.languageType == InfoYG.LanguageType.All)
            //    lang = language;

            Message("Language Request: Lang - " + lang);

            savesData.language = lang;

            if (infoYG.callingLanguageCheck == InfoYG.CallingLanguageCheck.FirstLaunchOnly)
                SaveProgress();

            SwitchLangEvent?.Invoke(lang);
        }
        #endregion Language

        #region Environment Data
        public static Action EnvirDataEvent;

        public void SetEnvironmentData(string data)
        {
            EnvironmentData = JsonUtility.FromJson<JsonEnvironmentData>(data);
            EnvirDataEvent?.Invoke();

        }
        #endregion Environment Data

        #region Leaderboard
        public delegate void UpdateLB(
            string name,
            string description,
            int[] rank,
            string[] photo,
            string[] playersName,
            int[] scorePlayers,
            bool auth);
        public static event UpdateLB UpdateLbEvent;

        JsonLB jsonLB = new JsonLB();

        int[] rank;
        string[] photo;
        string[] playersName;
        int[] scorePlayers;

        public void LeaderboardEntries(string data)
        {
            jsonLB = JsonUtility.FromJson<JsonLB>(data);

            rank = jsonLB.rank;
            photo = jsonLB.photo;
            playersName = jsonLB.playersName;
            scorePlayers = jsonLB.scorePlayers;

            UpdateLbEvent?.Invoke(
                jsonLB.nameLB.ToString(),
                jsonLB.entries.ToString(),
                rank,
                photo,
                playersName,
                scorePlayers,
                _auth);
        }

        void InitializedLB(){
            UpdateLbEvent?.Invoke("initialized", "no data", rank, photo, playersName, scorePlayers, _auth);
                _initializedLB = true;
        }
        #endregion Leaderboard


        // The rest

        #region Update
        int delayFirstCalls;
        static float timerShowAd;
        private void Update()
        {
            // Таймер для обработки показа Fillscreen рекламы
            timerShowAd += Time.unscaledDeltaTime;

            // Задержка первых вызовов 
            if(delayFirstCalls < 20)
            {
                delayFirstCalls++;
                if (delayFirstCalls == 20)
                    FirstСalls();
            }
        }
        #endregion Update

        #region Json
        public class JsonAuth
        {
            public string playerAuth;
            public string playerName;
            public string playerId;
            public string playerPhoto;
        }

        public class JsonLB
        {
            public string nameLB;
            public string entries;
            public int[] rank;
            public string[] photo;
            public string[] playersName;
            public int[] scorePlayers;
        }

        public class JsonEnvironmentData
        {
            public string domain = "ru";
            public string deviceType = "desktop";
            public bool isMobile;
            public bool isDesktop;
            public bool isTablet;
            public bool isTV;
        }

        [System.Serializable]
        public class JsonSaves
        {
            public bool isFirstSession = true;
            public string language = "ru";

            // Ваши сохранения
            public int money = 1;
            public string newPlayerName = "Hello!";
            public bool[] openLevels = new bool[3];
        }
#endregion Json
    }
}