using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyObject : MonoBehaviour
{
    public SoundSource Sound;

    public float BounceFalloff = 3.3f;
    public float BounceFrequency = 5f;
    public float BounceAmplitude = 0.3f;
    public float AnimationSpeedMultiplier = 1f;
    public float AnimationLength = 2f;

    private float temporaryAmplitudeModifier = 1f;
    private float animationValue = 0;
    private bool animating = false;

    private static Dictionary<Transform, PlayerMovement> playerDictionary = new Dictionary<Transform, PlayerMovement>();

    private void Update()
    {
        if (animating)
        {
            animationValue += Time.deltaTime * AnimationSpeedMultiplier;
            float bounceValue = temporaryAmplitudeModifier * BounceAmplitude * Mathf.Sin(animationValue * 2 * Mathf.PI * BounceFrequency) * Mathf.Pow(2.718f, -1 * animationValue * BounceFalloff) * -1 + 1;
            transform.localScale = new Vector3(1, bounceValue, 1);

            if (animationValue > 2f)
            {
                animating = false;
                animationValue = 0;
                transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }

    public void PlayBounceAnimation(float temporaryAmplitudeModifier = 1f)
    {
        this.temporaryAmplitudeModifier = temporaryAmplitudeModifier;
        animationValue = 0;
        animating = true;
    }

    public void PlayBounceSound()
    {
        Sound.Play("Bounce");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Player")
        {
            if (!playerDictionary.ContainsKey(collision.transform))
                playerDictionary.Add(collision.transform, collision.transform.GetComponent<PlayerMovement>());

            if (collision.relativeVelocity.y < -5)
            {
                Rigidbody rigidbody = playerDictionary[collision.transform].RigidBody;
                rigidbody.velocity = new Vector3(rigidbody.velocity.x, collision.relativeVelocity.y * -1, rigidbody.velocity.z);

                float bounceAmplitudeModifier = Mathf.Clamp(0.083f * Mathf.Abs(collision.relativeVelocity.y) + 0.343f, 0.8f, 1.4f);

                PlayBounceAnimation(bounceAmplitudeModifier);
                PlayBounceSound();
            }
            else
            {
                playerDictionary[collision.transform].LandedOnBouncyObject();
            }
        }
    }
}
