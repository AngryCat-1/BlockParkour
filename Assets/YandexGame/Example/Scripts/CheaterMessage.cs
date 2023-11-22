using UnityEngine;
using YG;

public class CheaterMessage : MonoBehaviour
{
    [SerializeField] GameObject notificationAdBlockPrefab;

    private void OnEnable() => YandexGame.CheaterVideoEvent += CheaterReward;
    private void OnDisable() => YandexGame.CheaterVideoEvent -= CheaterReward;

    void CheaterReward()
    {
        if (notificationAdBlockPrefab)
            Instantiate(notificationAdBlockPrefab);
    }
}
