using UnityEngine;
using UnityEngine.UI;
using YG;

public class GetPlayerData : MonoBehaviour
{
    [SerializeField] ImageLoadYG imageLoad;
    [SerializeField] Text textPlayerData;
    [SerializeField] Text textEnvirData;

    private void OnEnable()
    {
        YandexGame.PlayerDataEvent += PlayerData;
        YandexGame.EnvirDataEvent += EnvirData;
    }
    private void OnDisable()
    {
        YandexGame.PlayerDataEvent -= PlayerData;
        YandexGame.EnvirDataEvent -= EnvirData;
    }

    private void Start()
    {
        if (YandexGame.startGame)
        {
            PlayerData();
            EnvirData();
        }
    }

    void PlayerData()
    {
        textPlayerData.text = "playerName - " + YandexGame.playerName +
            "\n\nplayerId - " + YandexGame.playerId +
            "\n\nauth - " + YandexGame.auth +
            "\nstartGame - " + YandexGame.startGame +
            "\nadBlock - " + YandexGame.adBlock +
            "\ninitializedLB - " + YandexGame.initializedLB +
            "\nphotoSize - " + YandexGame.photoSize;

        if (imageLoad != null && YandexGame.auth)
            imageLoad.Load(YandexGame.playerPhoto);
    }

    void EnvirData()
    {
        textEnvirData.text = "domain - " + YandexGame.EnvironmentData.domain +
            "\ndeviceType - " + YandexGame.EnvironmentData.deviceType +
            "\nisMobile - " + YandexGame.EnvironmentData.isMobile +
            "\nisDesktop - " + YandexGame.EnvironmentData.isDesktop +
            "\nisTablet - " + YandexGame.EnvironmentData.isTablet +
            "\nisTV - " + YandexGame.EnvironmentData.isTV;
    }
}
