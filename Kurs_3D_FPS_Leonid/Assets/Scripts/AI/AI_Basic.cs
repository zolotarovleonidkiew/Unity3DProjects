using System.Linq;
using UnityEngine;

/// <summary>
/// TEST PURPOSE ONLY
/// </summary>
public class AI_Basic : MonoBehaviour
{
    [SerializeField] private GameObject _waypointsCollection;

    [SerializeField] private float _distance = 0.1f;

    [SerializeField] private GameObject _player;

    /// <summary>
    /// Коллекция вейпоинтов маршрута
    /// </summary>
    private BotRoutes _botRoutes;

    private int? _lastWPIndex = null;

    public float _speed = 5;
    public float _rotationSpeed = 5;

    /// <summary>
    /// Расстояние от улитки до спринтера
    /// </summary>
   // public float k = 8000;

    private void Awake()
    {
        _botRoutes = new BotRoutes(_waypointsCollection);
        //_rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        Debug.DrawLine(
            transform.position,
            _player.transform.position,
            Color.yellow);
    }

    private void FixedUpdate()
    {
        var nextWP = GetNextWaypoint();

        MoveToNextWaypoint(nextWP);

        var distance = Vector3.Distance(transform.position, nextWP.WaypointObject.transform.position);
        if (distance <= _distance)
        {
            nextWP.SetWaypointReached(true);

            _lastWPIndex = nextWP.Index;
        }
    }

    /// <summary>
    /// Двигатсья к следующему вейпоинту
    /// </summary>
    private void MoveToNextWaypoint(Waypoint nextWP)
    {
        Debug.DrawLine(transform.position, nextWP.WaypointObject.transform.position, Color.red);

        var direction = nextWP.WaypointObject.transform.position - transform.position;
        direction = direction.normalized;

        transform.position += direction * _speed * Time.deltaTime;

        var newRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * _rotationSpeed);

     //   Debug.Log($"MoveToNextWaypoint. direction: {direction}");
    }

    private Waypoint GetNextWaypoint()
    {
        int nextWeypointIndex = _lastWPIndex == null ? 0 : _lastWPIndex.Value + 1;

        if (nextWeypointIndex >= _botRoutes.WalkRoute.Count)
        {
            nextWeypointIndex = 0;
        }

        return _botRoutes.WalkRoute.First(wp => wp.Index == nextWeypointIndex);
    }
}
