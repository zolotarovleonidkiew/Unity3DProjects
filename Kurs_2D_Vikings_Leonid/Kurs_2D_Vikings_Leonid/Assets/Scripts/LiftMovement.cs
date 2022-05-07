using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Лифт курсирует между точками А и Б по нажатию на кнопку "Е".
/// Если кто-то упал в шахту лифта, то ему труба, когда лифт раздавит его
/// </summary>
public class LiftMovement : MonoBehaviour
{
    [SerializeField] private float _speed = 10f;
    [SerializeField] public bool _playerLaunchedLiftMovement; //уже нажал кнопку лифтаи должен поехать вместе с ним

    private bool _liftIsMovingUp = false;
    private bool _liftIsMovingDown = false;
    /// <summary>
    /// Лифт может изменить направление движения только в начальной и конечной точке
    /// </summary>
    public bool LiftCanStartMoving
    {
        get
        {
            return _liftOnStartPoint || _liftCanReturnToStartPoint;
        }
    }

    private bool _liftOnStartPoint = true;  //лифт на пункте А и готов ехать
    private bool _liftCanReturnToStartPoint = false; //лифт на пункте Б и готов ехать

    //промежуточные этапы пути туда и обратно
    bool pointA_reached;
    bool pointB_reached;
    bool pointC_reached;

    /// <summary>
    /// Игроки, которые зашли на платформу
    /// </summary>
    List<GameObject> playersOnThePlatform;

    void Start()
    {
        playersOnThePlatform = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        if (_liftIsMovingUp)
        {
            MovingUp();
        }
        if (_liftIsMovingDown)
        {
            MovingDown();
        }
        else
        {
            if (LiftCanStartMoving && _playerLaunchedLiftMovement)
            {
                if (_liftOnStartPoint)
                {
                    _liftIsMovingUp = true;
                    //move to point B 

                    //1. Y = -25.7
                    //2. X = 125.8
                    //3. Y = -12.28
                    MovingUp();
                }
                else
                {
                    _liftIsMovingDown = true;

                    //move to  Start point
                    MovingDown();
                }
            }
        }
    }

    /// <summary>
    /// Вверх
    /// </summary>
    private void MovingUp()
    {
        var currentPosition = transform.position;

        _liftOnStartPoint = false;

        if (!pointA_reached)
        {
            if (currentPosition.y <= -25.7)
            {
                //var y = currentPosition.y + 1;

                transform.position += Vector3.up * _speed * Time.deltaTime;
                //(new Vector3(0, y, 0)) * _speed * Time.deltaTime;

                MovePlayersOnThePlatform(Vector3.up);
            }
            else
            {
                pointA_reached = true;
            }
        }
        if (pointA_reached && !pointB_reached)
        {
            if (currentPosition.x >= 125.92)//221.6729)
            {
                //var x = currentPosition.x + 1;

                transform.position += Vector3.left * _speed * Time.deltaTime;
                //(new Vector3(x, 0, 0)) * _speed * Time.deltaTime;

                MovePlayersOnThePlatform(Vector3.left);
            }
            else
            {
                pointB_reached = true;
            }
        }
        if (pointB_reached && !pointC_reached)
        {
            if (currentPosition.y <= -12)//-73.3013)
            {
                //var y = currentPosition.y + 1;

                transform.position += Vector3.up * _speed * Time.deltaTime;
                //(new Vector3(0, y, 0)) * _speed * Time.deltaTime;

                MovePlayersOnThePlatform(Vector3.up);
            }
            else
            {
                pointA_reached = false;
                pointB_reached = false;
                pointC_reached = false;

                _liftCanReturnToStartPoint = true;
                _playerLaunchedLiftMovement = false;

                _liftIsMovingUp = false;
            }
        }
    }

    /// <summary>
    /// Вниз
    /// </summary>
    private void MovingDown()
    {
        var currentPosition = transform.position;

        _liftCanReturnToStartPoint = false;

        if (!pointA_reached)
        {
            if (currentPosition.y >= -25.7)
            {
                transform.position += Vector3.down * _speed * Time.deltaTime;
                
                MovePlayersOnThePlatform(Vector3.down);
            }
            else
            {
                pointA_reached = true;
            }
        }
        if (pointA_reached && !pointB_reached)
        {
            if (currentPosition.x <= 221.6729)//221.6729)
            {
                //var x = currentPosition.x + 1;

                transform.position += Vector3.right * _speed * Time.deltaTime;
                //(new Vector3(x, 0, 0)) * _speed * Time.deltaTime;

                MovePlayersOnThePlatform(Vector3.right);
            }
            else
            {
                pointB_reached = true;
            }
        }
        if (pointB_reached && !pointC_reached)
        {
            if (currentPosition.y >= -73.3013)//-73.3013)
            {
                //var y = currentPosition.y + 1;

                transform.position += Vector3.down * _speed * Time.deltaTime;
                //(new Vector3(0, y, 0)) * _speed * Time.deltaTime;

                MovePlayersOnThePlatform(Vector3.down);
            }
            else
            {
                pointA_reached = false;
                pointB_reached = false;
                pointC_reached = false;

                _liftCanReturnToStartPoint = false;
                _playerLaunchedLiftMovement = false;
                _liftOnStartPoint = true;

                _liftIsMovingDown = false;
            }
        }
    }

    /// <summary>
    /// Player зашел на платформу
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //здесь - коллекция плееров, которые на платформе, зашли и вышли учесть

        if (collision.gameObject.tag == "Player")
        {
            var player = collision.gameObject;

            if (player != null)
            {
                playersOnThePlatform.Add(player);

                /*
                 * утснвоить переменные у плеера:
                 * player.playerOnLiftPlatforfmFlag = true;
                 * 
                 */

                var pmControl = player.GetComponent<PlayerMovement>();
                pmControl.playerOnLiftPlatforfmFlag = true;
                pmControl.GameObjectLiftReference = this;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            var player = collision.gameObject;

            if (player != null)
            {
                var pmControl = player.GetComponent<PlayerMovement>();
                pmControl.playerOnLiftPlatforfmFlag = false;
                pmControl.GameObjectLiftReference = null;

                playersOnThePlatform.Remove(player);
            }
        }
    }

    private void MovePlayersOnThePlatform(Vector3 v)
    {
        foreach (var pl in playersOnThePlatform)
        {
            pl.transform.position += v * _speed * Time.deltaTime; 
        }
    }
}