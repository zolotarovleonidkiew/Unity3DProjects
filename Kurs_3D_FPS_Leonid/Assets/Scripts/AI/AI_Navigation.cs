using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Navigation : MonoBehaviour
{
    [SerializeField] private GameObject _waypointsCollection;
    [SerializeField] private float _distance = 0.1f;
    [SerializeField] private GameObject _player;

    [SerializeField] private bool _moveToPlayer = false;

    /// <summary>
    /// Коллекция вейпоинтов маршрута
    /// </summary>
    private BotRoutes _botRoutes;

    private int? _lastWPIndex = null;

    public float _speed = 5;
    public float _rotationSpeed = 5;

    private NavMeshAgent _agent;

    void Start()
    {
        _botRoutes = new BotRoutes(_waypointsCollection);
        _agent = GetComponent<NavMeshAgent>();

    }

    void Update()
    {
        Debug.DrawLine(
          transform.position,
          _player.transform.position,
          Color.yellow);
    }

    private void FixedUpdate()
    {
        if (_moveToPlayer)
        {
            var go = new GameObject();
            var transform = go.GetComponent<Transform>();
            transform.position = new Vector3(
                _player.transform.position.x,
                _player.transform.position.y,
                _player.transform.position.z);

            var w = new Waypoint(0, go);

            MoveToNextWaypoint(w);
        }
        else
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
    }

    /// <summary>
    /// Двигатсья к следующему вейпоинту
    /// </summary>
    private void MoveToNextWaypoint(Waypoint nextWP)
    {
        Debug.DrawLine(transform.position, nextWP.WaypointObject.transform.position, Color.red);

        var direction = nextWP.WaypointObject.transform.position - transform.position;
        direction = direction.normalized;

       // transform.position += direction * _speed * Time.deltaTime;

        _agent.destination += direction * _speed * Time.deltaTime;

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
