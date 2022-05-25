using UnityEngine;

/// <summary>
/// Проверка выполнения условия завершения уровная - все 3 викинга должнвы потревожить коллайдер
/// </summary>
public class LevelFinishingScript : MonoBehaviour
{
    [SerializeField] public bool UlrichReachedFlag = false;
    [SerializeField] public bool BaelogReachedFlag = false;
    [SerializeField] public bool OlafReachedFlag = false;

    /// <summary>
    /// Флад для GameController
    /// </summary>
    public bool LevelFinished => _levelFinished;
    private bool _levelFinished;

    /// <summary>
    /// Игрок зашел в зону выхода
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
    }

    /// <summary>
    /// Игрок вышел из зоны выхода
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
    }
}
