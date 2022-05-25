using Assets.Scripts.Banana_scripts;
using System;
using UnityEngine;

/// <summary>
/// Switch controller
/// 
/// Переключатель на стене
/// </summary>
public class SwitchHandler : MonoBehaviour
{
    [SerializeField] private Sprite _switchOff;
    [SerializeField] private Sprite _switchOn;

    /// <summary>
    /// Начальное а потом и текущее положение переключателя
    /// </summary>
    [SerializeField] private bool _startPositionOff;

    /// <summary>
    /// Автовозврат в положение OFF через {_autoReturnInSecons}
    /// </summary>
    [SerializeField] private bool _enableAutoreturnToPositionOff;
    [SerializeField] private int _autoReturnInSecons;

    [SerializeField] private GameObject _targetToDeactivate;

    /// <summary>
    /// Если вклюсчено - то пытается сгенерировать бомбу в месте child(0)
    /// </summary>
    [SerializeField] private bool _generateNewBomb;

    /// <summary>
    /// Спрайт взрыва (если _generateNewBomb вкл)
    /// </summary>
    [SerializeField] private Sprite _explotionSprite;
    /// <summary>
    /// Спрайт бомбы перед взрывом (если _generateNewBomb вкл)
    /// </summary>
    [SerializeField] private Sprite _bombHasBeenPlanted;

    public bool UserActivatedSwitch = false;

    private SpriteRenderer _spriteRenderer;

    private DateTime _dateSwitchChangedToOn;

    void Start()
    {
        _spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (_enableAutoreturnToPositionOff)
        {
            if (DateTime.Now >= _dateSwitchChangedToOn.AddSeconds(_autoReturnInSecons))
            {
                _targetToDeactivate.SetActive(true);
                _spriteRenderer.sprite = _switchOff;

                _startPositionOff = true;
            }
        }

        if (UserActivatedSwitch)
        {
            if (_generateNewBomb)
            {
                GenerateNewBomb();
            }
            else
            {
                StartDeActivationProcess();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var pmController = collision.GetComponent<PlayerMovement>();

        if (pmController != null)
        {
            pmController.PlayerCanActivateSwitch = true;
            pmController.GameObjectSwitchReference = this;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var pmController = collision.GetComponent<PlayerMovement>();

        if (pmController != null)
        {
            pmController.PlayerCanActivateSwitch = false;
            pmController.GameObjectSwitchReference = null;
        }
    }

    private void StartDeActivationProcess()
    {
        if (_startPositionOff)
        {
            _targetToDeactivate.SetActive(false);
            _dateSwitchChangedToOn = DateTime.Now;

            _spriteRenderer.sprite = _switchOn;

            _startPositionOff = false;
        }
        else
        {
            _targetToDeactivate.SetActive(true);
            _spriteRenderer.sprite = _switchOff;

            _startPositionOff = true;
        }
        UserActivatedSwitch = false;
    }

    private void GenerateNewBomb()
    {
        var bombGenerator = new BombPlanting();

        bombGenerator.Plant(
            transform.GetChild(0).position, _explotionSprite, _bombHasBeenPlanted);


        UserActivatedSwitch = false;
    }
}