using UnityEngine;
using UnityEngine.UI;
using YG;

public class SaverTest : MonoBehaviour
{
    [SerializeField] InputField integerText;
    [SerializeField] InputField stringifyText;
    [SerializeField] Text systemSavesText;
    [SerializeField] Toggle[] booleanArrayToggle;

    private void OnEnable() => YandexGame.LoadedSavesEvent += GetLoad;
    private void OnDisable() => YandexGame.LoadedSavesEvent -= GetLoad;

    private void Start()
    {
        GetLoad();
    }

    public void Save()
    {
        YandexGame.savesData.money = int.Parse(integerText.text);
        YandexGame.savesData.newPlayerName = stringifyText.text;

        for (int i = 0; i < booleanArrayToggle.Length; i++)
            YandexGame.savesData.openLevels[i] = booleanArrayToggle[i].isOn;

        YandexGame.SaveProgress();
    }

    public void Load() => YandexGame.LoadProgress();

    public void GetLoad()
    {
        integerText.text = string.Empty;
        stringifyText.text = string.Empty;

        integerText.placeholder.GetComponent<Text>().text = YandexGame.savesData.money.ToString();
        stringifyText.placeholder.GetComponent<Text>().text = YandexGame.savesData.newPlayerName;

        for (int i = 0; i < booleanArrayToggle.Length; i++)
            booleanArrayToggle[i].isOn = YandexGame.savesData.openLevels[i];

        systemSavesText.text = $"Language - {YandexGame.savesData.language}\n" +
        $"First Session - {YandexGame.savesData.isFirstSession}\n";
    }
}
