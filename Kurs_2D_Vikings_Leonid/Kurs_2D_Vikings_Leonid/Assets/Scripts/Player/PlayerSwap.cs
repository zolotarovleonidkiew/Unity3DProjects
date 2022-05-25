using System.Linq;
using UnityEngine;

/// <summary>
/// Change active viking;
/// Swap player by [TAB] or atomatically after viking's death
/// </summary>
public class PlayerSwap : MonoBehaviour
{
    [SerializeField] private GameObject[] _vikings;

    [SerializeField] int selectedUserId = 0;
    PlayerMovement pmUlrick;
    PlayerMovement pmOlaf;
    PlayerMovement pmBaelog;

    CharacterController2D cnUlrick;
    CharacterController2D cnOlaf;
    CharacterController2D cnBaelog;

    void Start()
    {
        pmUlrick = _vikings[1].GetComponent<PlayerMovement>();
        pmOlaf = _vikings[0].GetComponent<PlayerMovement>();
        pmBaelog = _vikings[2].GetComponent<PlayerMovement>();

        cnUlrick = _vikings[1].GetComponent<CharacterController2D>();
        cnOlaf = _vikings[0].GetComponent<CharacterController2D>();
        cnBaelog = _vikings[2].GetComponent<CharacterController2D>();

        ModifyAcceptance(0);
    }

    private int GetNextActiveVikingIndex(int currentIndex)
    {
        var inextI = currentIndex + 1;

        if (inextI > _vikings.Length - 1)
        {
            inextI = 0;
        }

        if (_vikings[inextI] == null)
        {
           return GetNextActiveVikingIndex(inextI);
        }
        else
        {
            selectedUserId = inextI;
            return inextI;
        }
    }

    void Update()
    {
        if (_vikings.Count(x => x == null) == 3)
        {
             return;
        }

        if (_vikings[selectedUserId] == null)
        {
            selectedUserId = GetNextActiveVikingIndex(selectedUserId);

            ModifyAcceptance(selectedUserId);
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            selectedUserId = GetNextActiveVikingIndex(selectedUserId);

            ModifyAcceptance(selectedUserId);
        }

    }

    private void ModifyAcceptance(int userIndex)
    {
        if (selectedUserId == 0) //olaf
        {
            Debug.Log("Olaf control activated");
            ActivateOlafControls();
            ActivateBaealogControls(deactkivate: true);
            ActivateUlrichControls(deactkivate: true);
        }
        else if (selectedUserId == 1) //ulrich
        {
            Debug.Log("Ulrich control activated");
            ActivateOlafControls(deactkivate: true);
            ActivateBaealogControls(deactkivate: true);
            ActivateUlrichControls();
        }
        else if (selectedUserId == 2) //baelog
        {
            Debug.Log("Baealog control activated");
            ActivateOlafControls(deactkivate: true);
            ActivateBaealogControls();
            ActivateUlrichControls(deactkivate: true);
        }

    }

    private void ActivateUlrichControls(bool deactkivate = false)
    {
        if (deactkivate)
        {
            if (pmUlrick != null)
            {
                pmUlrick.enabled = false;
            }
            if (pmUlrick != null)
            {
                pmUlrick.enabled = false;
            }
        }
        else
        {
            if (pmUlrick != null)
            {
                pmUlrick.enabled = true;
            }
            if (pmUlrick != null)
            {
                pmUlrick.enabled = true;
            }
        }
    }

    private void ActivateOlafControls(bool deactkivate = false)
    {
        if (deactkivate)
        {
            if (pmOlaf != null)
            {
                pmOlaf.enabled = false;
            }
            if (cnOlaf != null)
            {
                cnOlaf.enabled = false;
            }
        }
        else
        {
            if (pmOlaf != null)
            {
                pmOlaf.enabled = true;
            }
            if (cnOlaf != null)
            {
                cnOlaf.enabled = true;
            }
        }
    }

    private void ActivateBaealogControls(bool deactkivate = false)
    {
        if (deactkivate)
        {
            if (pmBaelog != null)
            {
                pmBaelog.enabled = false;
            }
            if (cnBaelog != null)
            {
                cnBaelog.enabled = false;
            }
        }
        else
        {
            if (pmBaelog != null)
            {
                pmBaelog.enabled = true;
            }
            if (cnBaelog != null)
            {
                cnBaelog.enabled = true;
            }
        }
    }
}
