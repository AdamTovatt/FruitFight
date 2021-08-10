using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal : MonoBehaviour
{
    public GameObject Confetti;
    public Transform ConfettiSpawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        Instantiate(Confetti, ConfettiSpawnPoint.transform.position + Vector3.up * 0.5f, Quaternion.identity);
        GameManager.Instance.LevelFinished();
        gameObject.SetActive(false);
    }
}
