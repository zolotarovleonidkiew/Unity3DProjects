using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControllerScript : MonoBehaviour
{
    [SerializeField] DialogWindowScript dialogWindowScript;

    void Start()
    {

    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //gameObject.SetActive(true);
            dialogWindowScript.Show();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            // gameObject.SetActive(false);
            dialogWindowScript.Hide();
        }
    }
}
