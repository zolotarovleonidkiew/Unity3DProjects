using System.Collections.Generic;
using UnityEngine;

public class ObstructionManager : MonoBehaviour
{
    [SerializeField]
    private Camera targetCamera;

    [SerializeField]
    private Transform target;

    private ObstructionObject currentObstruction;

    /// <summary>
    /// Обстракція навколишщого середовища для активного героя. Викликається із GameController, коли активний герой змінюється.
    /// </summary>   
    public void SetActiveHeroObstructionTarget(Transform target)
    {
        if (this.target == target)
            return;

        // Убираем старое препятствие
        if (currentObstruction != null)
        {
            currentObstruction.SetFade(1f);
            currentObstruction = null;
        }

        this.target = target;
    }

    private void Update()
    {
        if (targetCamera == null || target == null)
            return;

        CheckObstruction();
    }

    private void CheckObstruction()
    {
        Collider targetCollider = target.GetComponentInChildren<Collider>();

        if (targetCollider == null)
        {
            SetObstruction(null);
            return;
        }

        Bounds bounds = targetCollider.bounds;

        Vector3[] targetPoints =
        {
            // Верх
            new Vector3(bounds.center.x, bounds.max.y, bounds.center.z),

            // Центр
            bounds.center,

            // Низ
            new Vector3(bounds.center.x, bounds.min.y, bounds.center.z),

            // Левая сторона
            new Vector3(bounds.min.x, bounds.center.y, bounds.center.z),

            // Правая сторона
            new Vector3(bounds.max.x, bounds.center.y, bounds.center.z)
        };

        ObstructionObject detectedObstruction = null;

        Dictionary<ObstructionObject, int> obstructionHits = new();

        foreach (Vector3 point in targetPoints)
        {
            if (Physics.Linecast(
                    targetCamera.transform.position,
                    point,
                    out RaycastHit hit))
            {
                ObstructionObject obstruction =
                    hit.collider.GetComponentInParent<ObstructionObject>();

                if (obstruction != null)
                {
                    if (!obstructionHits.ContainsKey(obstruction))
                    {
                        obstructionHits[obstruction] = 0;
                    }

                    obstructionHits[obstruction]++;
                }
            }
        }

        ObstructionObject bestObstruction = null;
        int bestHits = 0;

        foreach (var pair in obstructionHits)
        {
            if (pair.Value > bestHits)
            {
                bestHits = pair.Value;
                bestObstruction = pair.Key;
            }
        }

        if (bestHits >= 2)
        {
            SetObstruction(bestObstruction);
        }
        else
        {
            SetObstruction(null);
        }

        //// Объект считается препятствием,
        //// если хотя бы 2 луча попали в него.
        //if (hits >= 2)
        //{
        //    SetObstruction(detectedObstruction);
        //}
        //else
        //{
        //    SetObstruction(null);
        //}
    }

    private void SetObstruction(ObstructionObject obstruction)
    {
        if (currentObstruction == obstruction)
            return;

        if (currentObstruction != null)
        {
            currentObstruction.SetFade(1f);
        }

        currentObstruction = obstruction;

        if (currentObstruction != null)
        {
            currentObstruction.SetFade(0.5f); // желаемый fade !!!
        } 
    }
}