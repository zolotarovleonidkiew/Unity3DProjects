using UnityEngine;

public interface IJoystickDetector
{
    bool IsMoved { get; }
    Vector2 Direction { get; }
}
