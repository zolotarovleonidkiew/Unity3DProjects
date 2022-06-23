using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Health Controller
/// </summary>
public class HeroController : MonoBehaviour
{
    /// <summary>
    /// Health
    /// </summary>
    public int Health
    {
        get { return _health; }
        set 
        { 
            if (_health - value >= 100)
            {
                _health = 100;
            }
            else if (_health - value <= 0)
            {
                _health = 0;
                OnDie.Invoke();
            }
            else
            {
                _health -= value;
            }
        }
    }
    private int _health;

    /// <summary>
    /// Is Dead
    /// </summary>
    public bool IsDead => _health == 0;

    #region Events
    public delegate void GetDamage(int damage);
    public delegate void Die();
    public delegate void Shoot();
    

    public event GetDamage OnTakeDamage;
    public event Die OnDie;
    public event Shoot OnShoot;
    
    #endregion

    void Start()
    {
        OnTakeDamage += OnTakeDamageHandler;
        OnDie += OnDieHandler;
        OnShoot += OnShootHandler;
    }

    void Update()
    {
        
    }

    //******************* Event Handlers ********************************************************************************

    public void OnTakeDamageHandler(int damage)
    {
        _health -= damage;
    }

    public void OnShootHandler()
    {

    }

    public void OnDieHandler()
    {

    }
}
