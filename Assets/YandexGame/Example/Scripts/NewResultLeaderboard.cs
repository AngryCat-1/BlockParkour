using UnityEngine;
using UnityEngine.UI;
using YG;

public class NewResultLeaderboard : MonoBehaviour
{
    [SerializeField] LeaderboardYG leaderboardYG;
    [SerializeField] InputField nameLbInputField;
    [SerializeField] InputField scoreLbInputField;

    public void _NewName()
    {
        leaderboardYG._NewNameLB(nameLbInputField.text);
    }

    public void _NewScore()
    {
        leaderboardYG._NewScore(int.Parse(scoreLbInputField.text));
    }
}
