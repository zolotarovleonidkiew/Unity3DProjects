using UnityEngine;

/// <summary>
/// Script schene - vikings leave with spaceship
/// 
/// Скриптовая сцена - викинги валят с корабля
/// </summary>
public class ScriptScheneGoodBy : MonoBehaviour
{
    [SerializeField] LevelFinishingScript GameWinnerScript;

    void FixedUpdate()
    {
        if (GameWinnerScript.LevelFinished)
        {
            var olaf = GameObject.Find("Hero-Olaf");
            var ulrich = GameObject.Find("Hero-Erik");
            var baealog = GameObject.Find("Hero-Baealog");

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
