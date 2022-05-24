using UnityEngine;
using UnityEngine.SceneManagement;

public class SuperGameController : MonoBehaviour
{
    [SerializeField] private bool _killThemAll;
    [SerializeField] private bool _showStartMenu;
    [SerializeField] private LoseEndGameMenu menuController;
    [SerializeField] LevelFinishingScript levelFinisgingObject;

    void Start()
    {
        menuController.MenuType = Assets.Scripts.MenuStateEnum.StartGame;
        menuController.Show();
    }

    void Update()
    {
        if (levelFinisgingObject.LevelFinished)
        {
            SceneManager.LoadScene("Level2");
        }
        else
        {
            ShowEndGameMenu();
        }
    }

    private void ShowEndGameMenu()
    {
        var olaf = GameObject.Find("Hero-Olaf");
        var ulrich = GameObject.Find("Hero-Erik");
        var baealog = GameObject.Find("Hero-Baealog");

        if ((_killThemAll) || (olaf == null && ulrich == null && baealog == null))
        {
            menuController.MenuType = Assets.Scripts.MenuStateEnum.FailedGame;

            //DEBUG +
            if (_killThemAll)
            {
                menuController.OlafWasDead = true;
                menuController.UlrichWasDead = true;
                menuController.BaealogWasDead = true;
            }
            //DEBUG -
            else
            {
                menuController.OlafWasDead = olaf == null;
                menuController.UlrichWasDead = ulrich == null;
                menuController.BaealogWasDead = baealog == null;
            }
            menuController.Show();
        }
    }
}
