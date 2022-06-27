using System.Collections;
using UnityEngine;

/// <summary>
/// Определяет, попал ли предмет прицеливания в объектив камеры смотрящего
/// </summary>
public class AI_BotSeePlayer : MonoBehaviour
{
    [SerializeField] public GameObject target;

    [SerializeField] public float radius;

    [Range(0, 360)]
    [SerializeField] public float angle;

    public LayerMask targetMask;
    public LayerMask obstructionMask;

    public bool canSeePlayer { get; private set; }

    public bool TARGET_AIMED { get; private set; }

    private void Start()
    {
        TARGET_AIMED = false;
        StartCoroutine(FOVRoutine());
    }

    private void Update()
    {
        if (canSeePlayer)
        {
            Debug.Log("Hello there");
            TARGET_AIMED = true;

            Debug.LogWarning("TARGET_AIMED = true");
        }
        else
        {
            TARGET_AIMED = false;
            Debug.LogWarning("TARGET_AIMED = false");
        }
        
    }

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                    canSeePlayer = true;
                else
                    canSeePlayer = false;
            }
            else
                canSeePlayer = false;
        }
        else if (canSeePlayer)
            canSeePlayer = false;
    }
}


// VERSION 2 (old)
/*
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
 */