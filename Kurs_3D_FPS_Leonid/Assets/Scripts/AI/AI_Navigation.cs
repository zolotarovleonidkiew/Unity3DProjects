using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Navigation : MonoBehaviour
{
    //waypoint routes
    [SerializeField] private GameObject _patrolRouteWP;
    [SerializeField] private GameObject _pickWeaponRouteWP;
    [SerializeField] private GameObject _healRouteWP;

    [SerializeField] private GameObject _player;
    [SerializeField] private bool _botSeePlayer = false;
    [SerializeField] private AI_state AI_state;

    /// <summary>
    /// Если да, до вызов CheckUpdateCurrentStrategy()
    /// </summary>
    public bool NeedCheckStrategy = false;

    //misc
    [SerializeField] private float _minDistanceToWayPoint = 2f;

    private InventoryController _inventory;
    private HeroController _health;

    /// <summary>
    /// Коллекция вейпоинтов маршрута
    /// </summary>
    private BotRoutes _patrolRoute;
    private BotRoutes _pickWeaponRoute;
    private BotRoutes _healRoute;

    private int? _lastWPPatrolIndex = null;
    private int? _lastWPPickWeaponIndex = null;
    private int? _lastWPHealIndex = null;

    public float _speed = 5;
    public float _rotationSpeed = 5;

    private NavMeshAgent _agent;

    private AI_BotSeePlayer _aimController;
    private WeaponShoot _weaponController;

    void Start()
    {
        _patrolRoute = new BotRoutes(_patrolRouteWP);
        _pickWeaponRoute = new BotRoutes(_pickWeaponRouteWP);
        _healRoute = new BotRoutes(_healRouteWP);

        _agent = GetComponent<NavMeshAgent>();
        _inventory = GetComponent<InventoryController>();
        _health = GetComponent<HeroController>();
        _aimController = GetComponent<AI_BotSeePlayer>();
        _weaponController = GetComponent<WeaponShoot>();

        //обновляем стратегию
        CheckUpdateCurrentStrategy();
    }

    void Update()
    {
        Debug.DrawLine(
          transform.position,
          _player.transform.position,
          Color.yellow);

        //Update strategy
        if (NeedCheckStrategy)
        {
            CheckUpdateCurrentStrategy();

            NeedCheckStrategy = false;
        }

        //БОТ УВИДЕЛ ИГРОКА
        if (_aimController.TARGET_AIMED)
        {
            _weaponController.TryShoot();

            Debug.LogError("AI SHOOTED");
        }
    }

    private void FixedUpdate()
    {
        Waypoint nextWP = default;

        if (AI_state == AI_state.Chase)
        {
            //преследование - это  TODO
            // а пока только стреляем в игрока
        }
        else
        {
            if (AI_state == AI_state.Heal)
            {
                nextWP = GetNextWaypoint(_healRoute, _lastWPHealIndex);
            }
            else if (AI_state == AI_state.PickWeapon)
            {
                nextWP = GetNextWaypoint(_pickWeaponRoute, _lastWPPickWeaponIndex);
            }
            else if (AI_state == AI_state.Patrol)
            {
                if (_patrolRoute == null)
                {
                    _patrolRoute = new BotRoutes(_patrolRouteWP);
                }
                nextWP = GetNextWaypoint(_patrolRoute, _lastWPPatrolIndex);
            }

            MoveToNextWaypoint(nextWP);

            var distance = Vector3.Distance(transform.position, nextWP.WaypointObject.transform.position);
            if (distance <= _minDistanceToWayPoint)
            {
                nextWP.SetWaypointReached(true);

                _lastWPPatrolIndex = nextWP.Index;
            }
        }
    }

    #region Waypoint and routes walkthrough

    /// <summary>
    /// Двигаться к следующему вейпоинту
    /// </summary>
    private void MoveToNextWaypoint(Waypoint nextWP)
    {
        Debug.DrawLine(transform.position, nextWP.WaypointObject.transform.position, Color.red);

        var direction = nextWP.WaypointObject.transform.position - transform.position;
        direction = direction.normalized;

        _agent.destination += direction * _speed * Time.deltaTime;

        var newRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * _rotationSpeed);

        //   Debug.Log($"MoveToNextWaypoint. direction: {direction}");
    }

    /// <summary>
    /// Получить следуюбщийендпоинт из маршрута
    /// </summary>
    /// <param name="route">Роут, зависит от AI_state</param>
    /// <param name="_lastWPIndex">Индпекс текущего/последнего посещенного вейпоинта</param>
    private Waypoint GetNextWaypoint(BotRoutes route, int? _lastWPIndex)
    {
        int nextWeypointIndex = _lastWPIndex == null ? 0 : _lastWPIndex.Value + 1;

        if (nextWeypointIndex >= route.WalkRoute.Count)
        {
            nextWeypointIndex = 0;
        }

        return route.WalkRoute.First(wp => wp.Index == nextWeypointIndex);
    }

    /// <summary>
    /// Получить ближайший ендпоинт из маршрута
    /// </summary>
    //private Waypoint GetNearestWaypointByAIState(AI_state state)
    //{
    //    var route = state == AI_state.PickWeapon ? _pickWeaponRoute : _healRoute;

    //    var distances = new Dictionary<float, Waypoint>();

    //    foreach (Waypoint wp in route.WalkRoute)
    //    {
    //        distances.Add(Vector3.Distance(transform.position, wp.WaypointObject.transform.position), wp);
    //    }

    //    var minDistance = distances.Min(x=>x.Key);

    //    return distances[minDistance];
    //}
    #endregion


    #region AI strategy

    /// <summary>
    /// Проверяем соответсвие выбранной стратегии (AI_state) текущему положению вещей
    /// </summary>
    private void CheckUpdateCurrentStrategy()
    {
        if (_health.Health > 25 && _inventory.BotReadyToKill() && _botSeePlayer)
        {
            AI_state = AI_state.Chase;
        }
        else if (_health.Health < 25)
        {
            AI_state = AI_state.Heal;
        }
        else if (_inventory.BotReadyToKill())
        {
            AI_state = AI_state.Patrol;
        }
        else if (_inventory.BotReadyToKill() == false)
        {
            AI_state = AI_state.PickWeapon;
        }

        Debug.Log($"Хорошо подумав, я решил что моя стратегия - {AI_state}");
    }

    #endregion
}
