using UnityEngine;
using UnityEngine.SceneManagement;

public class SuperGameController : MonoBehaviour
{
    [SerializeField] private LoseEndGameMenu loseMenu;
    [SerializeField] LevelFinishingScript levelFinisgingObject;

    void Start()
    {
        loseMenu.Hide();
    }

    void Update()
    {
        if (levelFinisgingObject.LevelFinished)
        {
            SceneManager.LoadScene("Level2");
        }
        else
        {
            var olaf = GameObject.Find("Hero-Olaf");
            var ulrich = GameObject.Find("Hero-Erik");
            var baealog = GameObject.Find("Hero-Baealog");

            if (olaf == null && ulrich == null && baealog == null)
            {
                loseMenu.Show();
            }
        }
    }
}
