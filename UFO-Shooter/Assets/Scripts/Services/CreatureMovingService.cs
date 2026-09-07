using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Hero/aliend movements using Waypoints
/// </summary>
public class CreatureMovingService
{
    public HashSet<WayPoint> GetReachableWayPoints(
        IGroundHierarchy groundHierarchy,
        GroundHierarchyLevel startLevel,
        Vector2Int startCoordinates,
        int maxSteps)
    {
        var reachable = new HashSet<WayPoint>();

        if (groundHierarchy?.GroudLayers == null || maxSteps <= 0)
            return reachable;

        var nodes = CreateNodes(groundHierarchy);
        var startNode = FindStartNode(nodes, startLevel, startCoordinates);
        if (startNode == null)
            return reachable;

        var queue = new Queue<SearchNode>();
        var visited = new HashSet<WayPoint> { startNode.WayPoint };
        queue.Enqueue(new SearchNode { Node = startNode, Distance = 0 });

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (current.Distance >= maxSteps)
                continue;

            foreach (var next in GetNeighbors(current.Node, nodes))
            {
                if (!visited.Add(next.WayPoint))
                    continue;

                reachable.Add(next.WayPoint);
                queue.Enqueue(new SearchNode
                {
                    Node = next,
                    Distance = current.Distance + 1
                });
            }
        }

        return reachable;
    }

    /// <summary>
    /// Builds a shortest path through the WayPoint graph.
    /// The returned queue contains only destination points, not the starting point.
    /// </summary>
    public bool TryCreatePath(
        IGroundHierarchy groundHierarchy,
        GroundHierarchyLevel startLevel,
        Vector2Int startCoordinates,
        WayPoint target,
        int maxSteps,
        out Queue<Vector3> path)
    {
        path = new Queue<Vector3>();

        if (groundHierarchy?.GroudLayers == null || target == null || maxSteps <= 0)
            return false;

        var nodes = CreateNodes(groundHierarchy);
        if (!nodes.TryGetValue(target, out var targetNode))
            return false;

        var startNode = FindStartNode(nodes, startLevel, startCoordinates);

        if (startNode == null || startNode.WayPoint == target)
            return false;

        var queue = new Queue<SearchNode>();
        var visited = new HashSet<WayPoint>();
        var startSearchNode = new SearchNode { Node = startNode };
        queue.Enqueue(startSearchNode);
        visited.Add(startNode.WayPoint);

        SearchNode targetSearchNode = null;
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            int currentDistance = GetDistance(current);
            if (currentDistance >= maxSteps)
                continue;

            foreach (var next in GetNeighbors(current.Node, nodes))
            {
                if (!visited.Add(next.WayPoint))
                    continue;

                var nextSearchNode = new SearchNode
                {
                    Node = next,
                    Previous = current,
                    Distance = currentDistance + 1
                };

                if (next.WayPoint == targetNode.WayPoint)
                {
                    targetSearchNode = nextSearchNode;
                    break;
                }

                queue.Enqueue(nextSearchNode);
            }

            if (targetSearchNode != null)
                break;
        }

        if (targetSearchNode == null)
            return false;

        var reversedPath = new Stack<Vector3>();
        for (var current = targetSearchNode; current != null; current = current.Previous)
            reversedPath.Push(current.Node.WayPoint.transform.position);

        reversedPath.Pop();
        while (reversedPath.Count > 0)
            path.Enqueue(reversedPath.Pop());

        return path.Count > 0;
    }

    private WayPointNode FindStartNode(
        Dictionary<WayPoint, WayPointNode> nodes,
        GroundHierarchyLevel startLevel,
        Vector2Int startCoordinates)
    {
        foreach (var node in nodes.Values)
        {
            if (node.Layer.GroundLevel == startLevel && node.Coordinates == startCoordinates)
                return node;
        }

        return null;
    }

    private Dictionary<WayPoint, WayPointNode> CreateNodes(IGroundHierarchy groundHierarchy)
    {
        var result = new Dictionary<WayPoint, WayPointNode>();

        foreach (var layer in groundHierarchy.GroudLayers)
        {
            if (layer?.SmallBoxed == null)
                continue;

            for (int x = 0; x < layer.SmallBoxed.GetLength(0); x++)
            {
                for (int y = 0; y < layer.SmallBoxed.GetLength(1); y++)
                {
                    var smallBox = layer.SmallBoxed[x, y];
                    var wayPoint = smallBox == null ? null : smallBox.GetComponent<WayPoint>();
                    if (wayPoint == null || result.ContainsKey(wayPoint))
                        continue;

                    result.Add(wayPoint, new WayPointNode
                    {
                        WayPoint = wayPoint,
                        Layer = layer,
                        Coordinates = new Vector2Int(Mathf.RoundToInt(wayPoint.Corrds.x), Mathf.RoundToInt(wayPoint.Corrds.y))
                    });
                }
            }
        }

        return result;
    }

    private IEnumerable<WayPointNode> GetNeighbors(
        WayPointNode source,
        Dictionary<WayPoint, WayPointNode> nodes)
    {
        foreach (var direction in source.WayPoint.AvailableDirections)
        {
            var movement = direction.Value;
            if (movement == null || !movement.Allowed)
                continue;

            WayPointNode target = null;
            if (movement.ConnectionType != WayPointConnectionType.Default && movement.TargetWaypoint != null)
            {
                nodes.TryGetValue(movement.TargetWaypoint, out target);
            }
            else
            {
                var coordinates = GetNeighborCoordinates(source.Coordinates, direction.Key);
                target = FindNode(source.Layer, coordinates, nodes);
            }

            if (target != null)
                yield return target;
        }
    }

    private WayPointNode FindNode(
        GroundLayer layer,
        Vector2Int coordinates,
        Dictionary<WayPoint, WayPointNode> nodes)
    {
        foreach (var node in nodes.Values)
        {
            if (node.Layer == layer && node.Coordinates == coordinates)
                return node;
        }

        return null;
    }

    private Vector2Int GetNeighborCoordinates(Vector2Int coordinates, WayPointDirection direction)
    {
        switch (direction)
        {
            case WayPointDirection.Up:
                return coordinates + Vector2Int.up;
            case WayPointDirection.Down:
                return coordinates + Vector2Int.down;
            case WayPointDirection.Left:
                return coordinates + Vector2Int.left;
            case WayPointDirection.Right:
                return coordinates + Vector2Int.right;
            default:
                return coordinates;
        }
    }

    private int GetDistance(SearchNode node)
    {
        return node.Distance;
    }

    private sealed class WayPointNode
    {
        public WayPoint WayPoint;
        public GroundLayer Layer;
        public Vector2Int Coordinates;
    }

    private sealed class SearchNode
    {
        public WayPointNode Node;
        public SearchNode Previous;
        public int Distance;
    }
}