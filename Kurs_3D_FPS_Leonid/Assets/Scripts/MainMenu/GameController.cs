using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private HeroController playerHealthController;
    [SerializeField] private HeroController targetHealthController;

    [SerializeField] private MainMenu mainMenu;
    [SerializeField] private HUDMenu hudMenu;

    List<GameObject> GreenTeam;
    List<GameObject> RedTeam;

    void Start()
    {
        //TODO : uncomment 
        //  mainMenu.Show();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerHealthController != null && targetHealthController != null)
        {

            //Это должно работать через события, но я не понял ещё как :(
            if (playerHealthController.IsDead || targetHealthController.IsDead)
            {
                ProcessEndGame();
            }

        }

    }

    private void ProcessEndGame()
    {
        if (playerHealthController.IsDead)
        {
            hudMenu.UpdateTemporaryMessage("YOU DEAD :(");

            //switch-off player controls
            var pm = playerHealthController.GetComponent<PlayerMovement>();
            pm.enabled = false;
            var w = playerHealthController.GetComponent<WeaponShoot>();
            w.enabled = false;
        }
        else if (targetHealthController.IsDead)
        {
            hudMenu.UpdateTemporaryMessage("YOU WON !!!");

            Destroy(targetHealthController.gameObject);
        }
    }
}
