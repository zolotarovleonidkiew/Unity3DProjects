using UnityEngine;

/// <summary>
/// Определяет, попал ли игрок ( тег Player) в зону видимости бота
/// Пример:
///https://www.youtube.com/watch?v=0IrZ3LDJoeM
/// </summary>
public class AI_BotSeePlayer : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private Camera _botCamera;

    void Start()
    {

    }

    void Update()
    {
        DetectPlayer();
    }

    private void DetectPlayer()
    {
        if (IsPlayerDetected(_botCamera, _player))
        {
            Debug.Log("I see player");
        }
    }

    private bool IsPlayerDetected(Camera c, GameObject target)
    {
        var planes = GeometryUtility.CalculateFrustumPlanes(c);
        var point = target.transform.position;

        foreach (var plane in planes)
        {
            if (plane.GetDistanceToPoint(point) < 0)
            {
                return false;
            }
        }

        return true;
    }
}
