using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TO DO
/// </summary>
public class PlayerSwap : MonoBehaviour
{
    [SerializeField] private GameObject Olaf;
    [SerializeField] private GameObject Ulrick;
    [SerializeField] private GameObject Baelog;

    //game logic
    public Character[] PlayerCharacters; // list of characters

    int selectedUserId = 0;
    PlayerMovement pmUlrick;
    PlayerMovement pmOlaf;
    PlayerMovement pmBaelog;

    CharacterController2D cnUlrick;
    CharacterController2D cnOlaf;
    CharacterController2D cnBaelog;

    void Start()
    {
        pmUlrick = Ulrick.GetComponent<PlayerMovement>();
        pmOlaf = Olaf.GetComponent<PlayerMovement>();
        pmBaelog = Baelog.GetComponent<PlayerMovement>();

        cnUlrick = Ulrick.GetComponent<CharacterController2D>();
        cnOlaf = Olaf.GetComponent<CharacterController2D>();
        cnBaelog = Baelog.GetComponent<CharacterController2D>();

        ModifyAcceptance(0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            selectedUserId =
            selectedUserId + 1 > 2 ?
            selectedUserId = 0 :
            selectedUserId + 1;

            ModifyAcceptance(selectedUserId);
        }
    }

    private void ModifyAcceptance(int userIndex)
    {
        if (selectedUserId == 0) //olaf
        {
            pmOlaf.enabled = true;
            cnOlaf.enabled = true;

            pmUlrick.enabled = false;
            cnUlrick.enabled = false;

            pmBaelog.enabled = false;
            cnBaelog.enabled = false;
        }
        else if (selectedUserId == 1) //ulrich
        {
            pmUlrick.enabled = true;
            cnUlrick.enabled = true;

            pmOlaf.enabled = false;
            cnOlaf.enabled = false;

            pmBaelog.enabled = false;
            cnBaelog.enabled = false;
        }
        else if (selectedUserId == 2) //baelog
        {
            pmBaelog.enabled = true;
            cnBaelog.enabled = true;

            pmOlaf.enabled = false;
            cnOlaf.enabled = false;

            pmUlrick.enabled = false;
            cnUlrick.enabled = false;
        }

    }
}
