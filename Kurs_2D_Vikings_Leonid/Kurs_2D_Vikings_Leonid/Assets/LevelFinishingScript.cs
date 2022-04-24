using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ѕроверка выполнени€ услови€ завершени€ уровна€ - все 3 викинда должнвы потревожить коллайдер
/// </summary>
public class LevelFinishingScript : MonoBehaviour
{

    [SerializeField] public bool UlrichReachedFlag = false;
    [SerializeField] public bool BaelogReachedFlag = false;
    [SerializeField] public bool OlafReachedFlag = false;

    /// <summary>
    /// »грок зашел в зону выхода
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var controller = collision.GetComponent<CharacterController2D>();

        if (controller != null)
        {
            var vikingName = controller.VikingName;

            if (vikingName == "Ulrick")
            {
                UlrichReachedFlag = true;
            }
            else if (vikingName == "Olaf")
            {
                OlafReachedFlag = true;
            }
            else if (vikingName == "Baelog")
            {
                BaelogReachedFlag = true;
            }

            if (UlrichReachedFlag && BaelogReachedFlag && OlafReachedFlag)
            {
                //Load New level !!!
                LoadNewLevel();
            }

        }
        else
        {
            Debug.LogError("LevelFinishingScript=>[TriggerEnter]=>controller is null");
        }
    }


    private void LoadNewLevel()
    {
        SceneManager.LoadScene("Level2");
    }

    /// <summary>
    /// »грок вышел из зоны выхода
    /// </summary>
    private void OnTriggerExit2D(Collider2D collision)
    {
        var controller = collision.GetComponent<CharacterController2D>();

        if (controller != null)
        {
            var vikingName = controller.VikingName;

            if (vikingName == "Ulrick")
            {
                UlrichReachedFlag = false;
            }
            else if (vikingName == "Olaf")
            {
                OlafReachedFlag = false;
            }
            else if (vikingName == "Baelog")
            {
                BaelogReachedFlag = false;
            }
        }
        else
        {
            Debug.LogError("LevelFinishingScript=>[TriggerExit]=>controller is null");
        }
    }
}
