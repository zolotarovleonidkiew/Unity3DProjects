using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Скриптовая сцена - викинги валят с корабля
/// </summary>
public class ScriptScheneGoodBy : MonoBehaviour
{
    [SerializeField] LevelFinishingScript GameWinnerScript;

    private bool _vikingsPrepeared;

    private void PrepareVikings(GameObject olaf, GameObject ulrich, GameObject baealog)
    {
        //Destroy(olaf.GetComponent<Rigidbody2D>());
        //Destroy(olaf.GetComponent<Collider2D>());

        //Destroy(ulrich.GetComponent<Rigidbody2D>());
        //Destroy(ulrich.GetComponent<Collider2D>());

        //Destroy(baealog.GetComponent<Rigidbody2D>());
        //Destroy(baealog.GetComponent<Collider2D>());

        _vikingsPrepeared = true;
    }

    void FixedUpdate()
    {
        if (GameWinnerScript.LevelFinished)
        {
            var olaf = GameObject.Find("Hero-Olaf");
            var ulrich = GameObject.Find("Hero-Erik");
            var baealog = GameObject.Find("Hero-Baealog");

            if (!_vikingsPrepeared)
            {
                PrepareVikings(olaf, ulrich, baealog);
            }

            if (olaf != null)
            {
                if (olaf.transform.position.x < this.transform.position.x)
                {
                    olaf.transform.position += new Vector3(3, 0, 0);
                }
                else
                {
                    olaf.SetActive(false);
                }
            }

            if (ulrich != null)
            {
                if (ulrich.transform.position.x < this.transform.position.x)
                {
                    ulrich.transform.position += new Vector3(3, 0, 0);
                }
                else
                {
                    ulrich.SetActive(false);
                }
            }

            if (baealog != null)
            {
                if (baealog.transform.position.x < this.transform.position.x)
                {
                    baealog.transform.position += new Vector3(3, 0, 0);
                }
                else
                {
                    baealog.SetActive(false);
                }
            }

            if (olaf == null && ulrich == null && baealog == null)
            {
                var animator = GetComponent<Animator>();
                animator.SetBool("ShowLastScriptSchene", true);
            }
        }
    }

    public void DestroyAfterPlay()
    {
        Destroy(this.gameObject);
    }
}
