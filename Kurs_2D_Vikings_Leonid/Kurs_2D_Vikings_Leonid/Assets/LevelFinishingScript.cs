using UnityEngine;

/// <summary>
/// �������� ���������� ������� ���������� ������� - ��� 3 ������� ������� ����������� ���������
/// </summary>
public class LevelFinishingScript : MonoBehaviour
{
    [SerializeField] public bool UlrichReachedFlag = false;
    [SerializeField] public bool BaelogReachedFlag = false;
    [SerializeField] public bool OlafReachedFlag = false;

    /// <summary>
    /// ���� ��� GameController
    /// </summary>
    public bool LevelFinished => _levelFinished;
    private bool _levelFinished;

    /// <summary>
    /// ����� ����� � ���� ������
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
    /// ����� ����� �� ���� ������
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
