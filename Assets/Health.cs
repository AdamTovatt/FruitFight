using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float StartHealth = 100f;

    public float CurrentHealth { get { return _currentHealth; } set { _currentHealth = value; HealthWasUpdated(); } }
    private float _currentHealth;

    public delegate void OnDiedHandler(Health sender);
    public event OnDiedHandler OnDied;

    public GameObject WaterSplash;

    public bool DestroyOnDeath = true;
    public bool CanDie = true;

    private void Start()
    {
        _currentHealth = StartHealth;
    }

    private void HealthWasUpdated()
    {
        if (_currentHealth <= 0)
        {
            if (CanDie)
            {
                OnDied?.Invoke(this);

                if (DestroyOnDeath)
                    Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Water")
        {
            if (CanDie)
            {
                OnDied?.Invoke(this);
                Instantiate(WaterSplash, transform.position, Quaternion.Euler(-90, 0, 0));
                DestroyWithDelay destroyWithDelay = gameObject.AddComponent<DestroyWithDelay>();
                destroyWithDelay.DelaySeconds = 0.2f;
                //Destroy(gameObject);
            }
        }
    }
}
