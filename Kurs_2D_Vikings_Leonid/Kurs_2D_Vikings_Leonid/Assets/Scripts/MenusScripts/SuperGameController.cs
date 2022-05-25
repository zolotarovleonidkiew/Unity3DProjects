using Assets.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Game controller
/// </summary>
public class SuperGameController : MonoBehaviour
{
    [SerializeField] private bool _killThemAll;
    [SerializeField] private bool _showStartMenu;
    [SerializeField] private LoseEndGameMenu menuController;
    [SerializeField] LevelFinishingScript levelFinisgingObject;

    [SerializeField] LevelsEnum currentLevel;

    /// <summary>
    /// De/Activate heroes on Menu
    /// </summary>
    private void DisableHeroes(bool undoDisable = false)
    {
        var olaf = GameObject.Find("Hero-Olaf");
        var ulrich = GameObject.Find("Hero-Erik");
        var baealog = GameObject.Find("Hero-Baealog");

        if (olaf != null && ulrich != null && baealog != null)
        {
            olaf.GetComponent<PlayerMovement>().AllowMovement = undoDisable;
            ulrich.GetComponent<PlayerMovement>().AllowMovement = undoDisable;
            baealog.GetComponent<PlayerMovement>().AllowMovement = undoDisable;
        }
    }

    void Start()
    {
        DisableHeroes();
        menuController.DisableHeroes = () => DisableHeroes(true);

        menuController.MenuType = MenuStateEnum.StartGame;
        menuController.Show();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //show menu pause
            DisableHeroes();

            menuController.MenuType = MenuStateEnum.PauseGame;
            menuController.Show();
        }

        if (levelFinisgingObject.LevelFinished)
        {
            if (currentLevel == LevelsEnum.Level1)
            {
                currentLevel = LevelsEnum.Level2;

                SceneManager.LoadScene(LevelsEnum.Level2.ToString());
            }
            if (currentLevel == LevelsEnum.Level2)
            {
                //PLAYER WON THIS NIGHTMARE :)
                menuController.MenuType = Assets.Scripts.MenuStateEnum.WinTheGame;
                menuController.Show();
            }
        }
        else if(menuController.MenuType == MenuStateEnum.FailedGame)
        {
            ShowEndGameMenu();
        }
    }

    private void ShowEndGameMenu()
    {
        if (menuController.MenuType != Assets.Scripts.MenuStateEnum.StartGame)
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
}
