using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Стрела, которую запускает Баелог в пространство
/// 
/// Может попасть в пространстно/убить врага
/// </summary>
public class Arrow : MonoBehaviour
{
    /// <summary>
    /// Откуда вылетает стрела (позиция Баелога)
    /// </summary>
    public Vector3 StartPosition { get; set; }

    /// <summary>
    /// Куда летит стрела - влево/вправо в зависимотсри куда он смотрит, на одной высоте
    /// </summary>
    public Vector3 EndPosition { get; set; }

    /// <summary>
    /// Скорость стрелы
    /// </summary>
    public float Speed = 10;

    public Sprite ArrowSprite { get; set; }

    /*
     * 
     [?] Как определить границу экрана слева или справа? Viewport ?
     
     */

    public Arrow(Vector3 startPosition, MovementDirections direction)
    {
        StartPosition = startPosition;

        /*DEFINE EndPosition

        стрела летит тупо влева иил вправо, в зависимости от куда смотрит викинг

        */
        if (direction == MovementDirections.left)
        {
            EndPosition = new Vector3(-10, startPosition.y, startPosition.z);
        }
        else if (direction == MovementDirections.right)
        {
            EndPosition = new Vector3(10, startPosition.y, startPosition.z);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        /*
         * стрела летит
         */
    }

    /// <summary>
    /// EVENTS - стрела куда/то попала (преграда или враг)
    /// </summary>
    public void OnArrowShootSomething()
    {
        //стрела куда-то попала

        // КАК ???
    }
}

