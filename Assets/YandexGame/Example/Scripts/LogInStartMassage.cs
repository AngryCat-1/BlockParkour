using UnityEngine;
using YG;

public class LogInStartMassage : MonoBehaviour
{
    [SerializeField] GameObject logInMassagePrefab;

    private void OnEnable() => YandexGame.onMessageLogInEvent += Massage;
    private void OnDisable() => YandexGame.onMessageLogInEvent -= Massage;

    void Massage()
    {
        if (logInMassagePrefab)
            Instantiate(logInMassagePrefab);
    }
}
