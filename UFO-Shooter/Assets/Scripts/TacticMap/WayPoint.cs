using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Представляет текущий Small-Box и показывает,
/// в каких направлениях из него может двигаться герой.
/// </summary>
public class WayPoint : MonoBehaviour
{
    /// <summary>
    /// Test-only: true => WayPoint уже обработан и не нужно его обрабатывать повторно
    /// </summary>
    public bool IsProcessed;

    /// <summary>
    /// Координаты Small-Box на TacticMap.
    /// </summary>
    [SerializeField] private Vector2 _corrds;
    [SerializeField] private bool _coordsInitialized;
    
    /// <summary>
    /// Координаты Small-Box на TacticMap.
    /// После установки изменить их нельзя.
    /// </summary>    
    public Vector2 Corrds
    {
        get => _corrds;

        private set
        {
            if (!_coordsInitialized)
            {
                _corrds = value;
                _coordsInitialized = true;
            }
        }
    }

    [SerializeField]
    private List<WayPointDirectionEntry> availableDirections = new()
    {
        new(WayPointDirection.Up, new WayPointMovement(true, WayPointConnectionType.Default)),
        new(WayPointDirection.Down, new WayPointMovement(true, WayPointConnectionType.Default)),
        new(WayPointDirection.Left, new WayPointMovement(true, WayPointConnectionType.Default)),
        new(WayPointDirection.Right, new WayPointMovement(true, WayPointConnectionType.Default))
    };

    public Dictionary<WayPointDirection, WayPointMovement> AvailableDirections { get; private set; } = new();

    private void Awake()
    {
        RebuildAvailableDirections();
    }

    private void OnValidate()
    {
        RebuildAvailableDirections();
    }

    public bool AllDirectionsAllowed
    {
        get
        {
            foreach (var direction in AvailableDirections.Values)
            {
                if (!direction.Allowed)
                {
                    return false;
                }
            }
            return true;
        }
    }

    /// <summary>
    /// Создает WayPoint со всеми доступными направлениями.
    /// </summary>
    public void CreateDefaultWayPoint(int x, int y)
    {
        Corrds = new Vector2(x, y);
    }

    /// <summary>
    /// Изменить доступность движения в указанном направлении.
    /// </summary>
    public bool SetDirectionAllowed(WayPointDirection direction, bool allowed)
    {
        if (!AvailableDirections.TryGetValue(direction, out var movement) || movement == null)
        {
            return false;
        }

        movement.Allowed = allowed;
        SyncAvailableDirectionsList();
        return true;
    }


    /// <summary>
    /// Создает WayPoint с указанными доступными направлениями.
    /// </summary>
    public void CreateWayPoint(
        int x,
        int y,
        Dictionary<WayPointDirection, WayPointMovement> availableDirections)
    {
        Corrds = new Vector2(x, y);
        AvailableDirections = new Dictionary<WayPointDirection, WayPointMovement>(availableDirections);
        SyncAvailableDirectionsList();
    }

    private void RebuildAvailableDirections()
    {
        AvailableDirections.Clear();

        foreach (var entry in availableDirections)
        {
            if (entry != null && entry.Movement != null)
            {
                AvailableDirections[entry.Direction] = entry.Movement;
            }
        }
    }

    private void SyncAvailableDirectionsList()
    {
        availableDirections.Clear();

        foreach (var direction in AvailableDirections)
        {
            availableDirections.Add(new WayPointDirectionEntry(direction.Key, direction.Value));
        }
    }
}

[Serializable]
public class WayPointDirectionEntry
{
    public WayPointDirection Direction;
    public WayPointMovement Movement;

    public WayPointDirectionEntry()
    {
    }

    public WayPointDirectionEntry(WayPointDirection direction, WayPointMovement movement)
    {
        Direction = direction;
        Movement = movement;
    }
}


/// <summary>
/// Доступные направления движения из Small-Box.
/// </summary>
[Serializable]
public enum WayPointDirection
{
    Up = 1,
    Down = 2,
    Left = 3,
    Right = 4
}

/// <summary>
/// Тип соединения между WayPoint'ами. (как герой добирается до следующей точки)
/// </summary>
[Serializable]
public enum WayPointConnectionType
{
    Default = 0, //прямо вперед идет
    Rooftop = 1, //залазит на крышу по лестнице
    Teleport = 2 //телепортиурется (заходит в комнату и пр.)
}

[Serializable]
public class WayPointMovement
{
    /// <summary>
    /// True => есть доступ к следующему WayPoint'у в этом направлении, false => нет доступа
    /// </summary>
    public bool Allowed;

    /// <summary>
    /// Тип соединения между WayPoint'ами. (как герой добирается до следующей точки)
    /// </summary>
    public WayPointConnectionType ConnectionType;

    /// <summary>
    /// Ссылка на целевой WayPoint, куда можно попасть из текущего WayPoint'а в этом направлении - 
    /// только при WayPointConnectionType.Rooftop или Teleport
    /// </summary>
    public WayPoint TargetWaypoint = null;

    public WayPointMovement()
    {
        Allowed = true;
        ConnectionType = WayPointConnectionType.Default;
    }

    public WayPointMovement(bool allowed)
    {
        Allowed = allowed;
        ConnectionType = WayPointConnectionType.Default;
    }

    public WayPointMovement(bool allowed, WayPointConnectionType connectionType)
    {
        Allowed = allowed;
        ConnectionType = connectionType;
    }

    public WayPointMovement(WayPoint targetWaypoint, WayPointConnectionType connectionType = WayPointConnectionType.Rooftop)
    {
        Allowed = true;
        ConnectionType = connectionType;
        TargetWaypoint = targetWaypoint;
    }
}