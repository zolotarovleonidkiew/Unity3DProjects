using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Health Controller
/// </summary>
public class HeroController : MonoBehaviour
{
    [SerializeField] private HUDMenu _hudMenu;

    /// <summary>
    /// Health
    /// </summary>
    public int Health
    {
        get { return _health; }
        set
        {
            _health = value;

            if (_health < 0)
            {
                _health = 0;
            }
            else if (_health > 100)
            {
                _health = 100;
            }
            //if (_health - value >= 100)
            //{
            //    _health = 100;
            //}
            //else if (_health - value <= 0)
            //{
            //    _health = 0;
            //    //  OnDie.Invoke(); //уведомить gameController через событие, что игроку крышка
            //}
            //else
            //{
            //    _health -= value;
            //}
        }
    }
    private int _health;

    /// <summary>
    /// Is Dead
    /// </summary>
    public bool IsDead => _health == 0;

    #region Events
    public delegate void GetDamage(int damage, bool updateHUD);
    public delegate void Die();
    public delegate void Shoot();


    public event GetDamage OnTakeDamage;
    //public event Die OnDie;
    //public event Shoot OnShoot;

    #endregion

    void Start()
    {
        //Start health = 100;
        _health = 50;

        OnTakeDamage += OnTakeDamageHandler;
    }

    void Update()
    {

    }

    public void OnTakeDamageHandler(int damage, bool updateHUD)
    {
        Health -= damage;

        //Update HUD
        if (updateHUD)
        {
            _hudMenu.UpdateHP(_health);
        }

    }
}
