using UnityEngine;

public class SuperGameController : MonoBehaviour
{
    [SerializeField] private LoseEndGameMenu loseMenu;

    bool formDisplayed;

    void Start()
    {
        loseMenu.Hide();
    }

    void Update()
    {
        if (!formDisplayed)
        {
            var olaf = GameObject.Find("Hero-Olaf");
            var ulrich = GameObject.Find("Hero-Erik");
            var baealog = GameObject.Find("Hero-Baealog");

            if (olaf == null && ulrich == null && baealog == null)
            {
                loseMenu.Show();
                formDisplayed = true;
            }
        }
    }
}
