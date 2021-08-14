using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float StartHealth = 100f;

    public float CurrentHealth { get { return _currentHealth; } private set { _currentHealth = value; HealthWasUpdated(); } }
    private float _currentHealth;

    public delegate void OnDiedHandler(Health sender, CauseOfDeath causeOfDeath);
    public event OnDiedHandler OnDied;

    public delegate void OnHealthUpdatedHandler();
    public event OnHealthUpdatedHandler OnHealthUpdated;

    public GameObject WaterSplash;
    public GameObject SpawnOnDeathPrefab;
    public SoundSource SoundSource;
    public string DamageSoundName;

    public bool DestroyOnDeath = true;
    public bool CanDie = true;

    private bool IsPlayer { get { playerMovement = gameObject.GetComponent<PlayerMovement>(); return playerMovement != null; } }
    private PlayerMovement playerMovement;

    private List<Renderer> meshRenderers;

    private bool emissionIsOn;
    private float emissionOnTime;

    private void Awake()
    {
        meshRenderers = new List<Renderer>();
        _currentHealth = StartHealth;
    }

    private void Start()
    {
        Renderer meshRenderer = gameObject.GetComponent<Renderer>();
        if (meshRenderer != null)
            meshRenderers.Add(meshRenderer);

        meshRenderers.AddRange(gameObject.GetComponentsInChildren<Renderer>());
    }

    private void Update()
    {
        if(emissionIsOn)
        {
            if(Time.time - emissionOnTime > 0.1f)
            {
                foreach(Renderer renderer in meshRenderers)
                {
                    SetEmission(renderer.material, 0);
                    emissionIsOn = false;
                }
            }
        }
    }

    private void HealthWasUpdated()
    {
        if (_currentHealth <= 0)
        {
            if (CanDie)
            {
                Died(CauseOfDeath.Damage);

                if (DestroyOnDeath)
                {
                    if (IsPlayer)
                        RemoveFromPlayerList();

                    Destroy(gameObject);
                }
            }
        }

        OnHealthUpdated?.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Water")
        {
            if (CanDie)
            {
                Instantiate(WaterSplash, transform.position, Quaternion.Euler(-90, 0, 0));
                Died(CauseOfDeath.Water);
            }
        }
    }

    private void Died(CauseOfDeath causeOfDeath)
    {
        if (SpawnOnDeathPrefab != null)
            Instantiate(SpawnOnDeathPrefab, transform.position, Quaternion.identity);

        OnDied?.Invoke(this, causeOfDeath);
    }

    private void RemoveFromPlayerList()
    {
        GameManager.Instance.PlayerCharacters.Remove(playerMovement);
        GameManager.Instance.MultipleTargetCamera.Targets.Remove(playerMovement.transform);
    }

    public void Heal(float amount)
    {
        CurrentHealth += amount;
    }

    public void TakeDamage(float amount)
    {
        CurrentHealth -= amount;

        foreach(Renderer meshRenderer in meshRenderers)
        {
            SetEmission(meshRenderer.material, 1);
        }

        emissionIsOn = true;
        emissionOnTime = Time.time;

        if (SoundSource != null && !string.IsNullOrEmpty(DamageSoundName))
            SoundSource.Play(DamageSoundName);
    }

    private void SetEmission(Material material, float emission)
    {
        material.EnableKeyword("_EMISSION");

        Color baseColor = Color.red; //Replace this with whatever you want for your base color at emission level '1'

        Color finalColor = baseColor * Mathf.LinearToGammaSpace(emission);

        material.SetColor("_EmissionColor", finalColor);
    }
}

public enum CauseOfDeath
{
    Damage, Water
}