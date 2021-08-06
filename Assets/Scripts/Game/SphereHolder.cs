using UnityEngine;

[RequireComponent(typeof(DetailColorController))]
public class SphereHolder : MonoBehaviour
{
    public ParticleSystem Smoke;
    public float SmokeAlpha = 0.2f;

    private StateSwitcher switcher;
    private DetailColorController detailColor;

    private AudioSource audioSource;

    private void Awake()
    {
        detailColor = gameObject.GetComponent<DetailColorController>();
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    private void Start()
    {
        audioSource.time = Random.Range(0f, 1f);
        switcher = gameObject.GetComponent<StateSwitcher>();
        switcher.OnActivated += Activated;
        switcher.OnDeactivated += Deactvated;

        Color color = detailColor.Color.ToColor();
        ParticleSystem.MainModule smokeMain = Smoke.main;
        smokeMain.startColor = new ParticleSystem.MinMaxGradient(new Color(color.r, color.g, color.b, SmokeAlpha), new Color(color.r, color.g, color.b, SmokeAlpha * 0.8f));
    }

    private void Activated()
    {
        Smoke.Play();
        detailColor.SetEmission(1);
        audioSource.Play();
    }

    private void Deactvated()
    {
        Smoke.Stop();
        detailColor.SetEmission(0);
        audioSource.Stop();
    }

    private void OnDestroy()
    {
        switcher.OnActivated -= Activated;
        switcher.OnDeactivated -= Deactvated;
    }
}
