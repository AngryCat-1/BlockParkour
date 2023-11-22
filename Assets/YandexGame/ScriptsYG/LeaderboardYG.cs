using UnityEngine;
using UnityEngine.UI;
using UnityToolbag;

namespace YG
{
    public class LeaderboardYG : MonoBehaviour
    {
        [Tooltip("Техническое название соревновательной таблицы")]
        public string nameLB;
        [Tooltip("Максимальное кол-во получаемых игроков")]
        public int maxQuantityPlayers = 20;
        [Tooltip("Кол-во получения верхних топ игроков")]
        [Range(1, 10)]
        public int quantityTop = 3;
        [Tooltip("Кол-во получаемых записей возле пользователя")]
        [Range(1, 20)]
        public int quantityAround = 3;
        [Tooltip("Перетащите компонент Text для записи описания таблицы, если вы не выбрали продвинутую таблицу (advanced)")]
        public Text entriesText;
        [Tooltip("Продвинутая таблица. Поддерживает подгрузку авата и конвертацию рекордов в тип Time")]
        public bool advanced;
        public enum PlayerPhotoSize { nonePhoto, small, medium, large };
        [Tooltip("Размер подгружаемых изображений игроков. nonePhoto = не подгружать изображение")]
        [ConditionallyVisible(nameof(advanced))]
        public PlayerPhotoSize playerPhotoSize;
        [Tooltip("Конвертация полученных рекордов в Time тип")]
        [ConditionallyVisible(nameof(advanced))]
        public bool timeTypeConvert;

        YandexGame yg;
        string photoSize;

        void Start()
        {
            if (playerPhotoSize == PlayerPhotoSize.nonePhoto)
                photoSize = "nonePhoto";
            if (playerPhotoSize == PlayerPhotoSize.small)
                photoSize = "small";
            else if (playerPhotoSize == PlayerPhotoSize.medium)
                photoSize = "medium";
            else if (playerPhotoSize == PlayerPhotoSize.large)
                photoSize = "large";

            yg = GameObject.Find("YandexGame").GetComponent<YandexGame>();

            if(YandexGame.initializedLB) 
                _UpdateLB();
        }

        private void OnEnable() => YandexGame.UpdateLbEvent += OnUpdateLB;
        private void OnDisable() => YandexGame.UpdateLbEvent -= OnUpdateLB;

        void OnUpdateLB(string _name, string entriesLB, int[] rank, string[] photo, string[] playersName, int[] scorePlayers, bool auth)
        {
            if (_name == "initialized"){
                _UpdateLB();
            }

            else if (_name == nameLB)
            {
                string error = "...";

                if (entriesLB == "No data")
                {
                    if (auth)
                    {
                        switch (PlayerPrefs.GetString("Language"))
                        {
                            case "ru":
                                error = "Нет данных";
                                break;
                            case "tr":
                                error = "Veri yok";
                                break;
                            default:
                                error = "No data";
                                break;
                        }
                    }
                    else
                    {
                        switch (PlayerPrefs.GetString("Language"))
                        {
                            case "ru":
                                error = "Требуется Авторизация!";
                                break;
                            case "tr":
                                error = "Yetkilendirme gerekli!";
                                break;
                            default:
                                error = "Authorization is required!";
                                break;
                        }
                    }
                }

                if (!advanced)
                {
                    if (entriesLB == "No data") entriesText.text = error;
                    else entriesText.text = entriesLB;
                }
                    
                else
                {
                    GameObject sampleContainer = transform.GetComponentInChildren<GridLayoutGroup>().transform.GetChild(0).gameObject;

                    for (int i = 0; i < sampleContainer.transform.parent.childCount; i++)
                        if (i != 0) Destroy(sampleContainer.transform.parent.GetChild(i).gameObject);

                    if (entriesLB == "No data")
                    {
                        sampleContainer.transform.Find("Rank").GetComponentInChildren<Text>().text = "";
                        sampleContainer.transform.Find("Score").GetComponentInChildren<Text>().text = "";
                        sampleContainer.transform.Find("Name").GetComponentInChildren<Text>().text = error;
                    }
                    else
                    {
                        for (int i = 0; i < rank.Length; i++)
                        {
                            if (i != 0) sampleContainer = Instantiate(sampleContainer, sampleContainer.transform.parent);

                            sampleContainer.transform.Find("Rank").GetComponentInChildren<Text>().text = rank[i].ToString();
                            sampleContainer.transform.Find("Name").GetComponentInChildren<Text>().text = playersName[i];
                            if (!timeTypeConvert)
                                sampleContainer.transform.Find("Score").GetComponentInChildren<Text>().text = scorePlayers[i].ToString();
                            else sampleContainer.transform.Find("Score").GetComponentInChildren<Text>().text = scorePlayers[i].ToString("D4").Insert(2, ":");
                            if (photo[i] != "nonePhoto")
                                sampleContainer.transform.Find("Photo").GetComponentInChildren<ImageLoadYG>().Load(photo[i]);
                        }
                    }
                }
            }
        }

        public void _UpdateLB()
        {
            YandexGame.GetLeaderboard(nameLB, maxQuantityPlayers, quantityTop, quantityAround, photoSize);
        }

        public void _NewNameLB(string newName)
        {
            nameLB = newName;
            YandexGame.GetLeaderboard(newName, maxQuantityPlayers, quantityTop, quantityAround, photoSize);
        }

        public void _NewScore(int score)
        {
            YandexGame.NewLeaderboardScores(nameLB, score);
        }
    }
}

