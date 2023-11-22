using UnityEngine;
using UnityToolbag;

namespace YG
{
    [CreateAssetMenu(fileName = "YandexGameData", menuName = "YG Settings")]
    public class InfoYG : ScriptableObject
    {
        [Header("Player Data")]
        [Tooltip("Вкл/Выкл авторизацию")]
        public bool authorizationEnable;

        [ConditionallyVisible(nameof(authorizationEnable))]
        [Tooltip("Вкл/Выкл лидерборды")]
        public bool leaderboardEnable;

        [ConditionallyVisible(nameof(authorizationEnable))]
        public bool cloudSaves;

        public enum PlayerPhotoSize { small, medium, large };
        [ConditionallyVisible(nameof(authorizationEnable))]
        [Tooltip("Размер подкачанного изображения пользователя")]
        public PlayerPhotoSize playerPhotoSize;

        [Header("Ad")]
        [Space(10)]

        [Tooltip("Защита от накруток вознаграждения при использовании рекламы за вознаграждение. Не даёт награду пользователям с AdBlock и другими аналогичными расширениями браузера. Пользователям, которые закрывают рекламу раньше времени. Предотвращает открытие нескольких рекламных блоков и соответственно получения чрезмерной награды")]
        public bool checkAdblock = true;

        //public enum LanguageType { None, RuEndEn, RuTrEn, All };
        //[Header("Language Translation")]
        //[Space(10)]
        //[Tooltip("None - не записвать данные о языке\nRuEndEn - Русский и Английский яызки\nRuTrEn - Русский, Английский и Турецкий\nAll - Все языки")]
        //public LanguageType languageType;

        [Header("Language Translation")]
        [Space(10)]

        public bool LocalizationEnable;

        [Tooltip("Отображать параметры автоматической авторизации в инспекторе компонента LanguageYG")]
        [ConditionallyVisible(nameof(LocalizationEnable))]
        public bool autolocationInspector = true;

        public enum CallingLanguageCheck { FirstLaunchOnly, EveryGameLaunch, DoNotChangeLanguageStartup };
        [Tooltip("Менять язык игры в соответствии с языком браузера:\nFirstLaunchOnly - Только при первом запуске игры\nEveryGameLaunch - Каждый раз при запуске игры\nDoNotChangeLanguageStartup - Не менять язык при запуске игры")]
        [ConditionallyVisible(nameof(LocalizationEnable))]
        public CallingLanguageCheck callingLanguageCheck;

        [System.Serializable]
        public class Languages { public bool ru, en, tr, az, be, he, hy, ka, et, fr, kk, ky, lt, lv, ro, tg, tk, uk, uz; }
        [ConditionallyVisible(nameof(LocalizationEnable))]
        public Languages languages;

        [System.Serializable]
        public class Fonts { public Font defaultFont, ru, en, tr, az, be, he, hy, ka, et, fr, kk, ky, lt, lv, ro, tg, tk, uk, uz; }
        [ConditionallyVisible(nameof(LocalizationEnable))]
        public Fonts fonts;

        [Header("Other")]
        [Space(10)]

        public bool debug = true;
    }
}
