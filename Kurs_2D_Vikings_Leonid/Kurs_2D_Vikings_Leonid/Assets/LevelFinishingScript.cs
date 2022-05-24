using UnityEngine;

/// <summary>
/// ѕроверка выполнени€ услови€ завершени€ уровна€ - все 3 викинда должнвы потревожить коллайдер
/// </summary>
public class LevelFinishingScript : MonoBehaviour
{
    [SerializeField] public bool UlrichReachedFlag = false;
    [SerializeField] public bool BaelogReachedFlag = false;
    [SerializeField] public bool OlafReachedFlag = false;

    public bool LevelFinished => _levelFinished;
    private bool _levelFinished;

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
            else if (vikingName == "Baealog")
            {
                BaelogReachedFlag = true;
            }

            //Exit level
            if (UlrichReachedFlag && BaelogReachedFlag && OlafReachedFlag)
            {
                _levelFinished = true;
            }

        }
        else
        {
            Debug.LogError("LevelFinishingScript=>[TriggerEnter]=>controller is null");
        }
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
