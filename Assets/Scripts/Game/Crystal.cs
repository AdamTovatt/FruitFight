using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal : MonoBehaviour
{
    private bool consumed = false;
    private bool hidden = false;
    private float positionAdder = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (consumed)
            return;

        consumed = true;
        GameManager.Instance.LevelFinished();
    }

    private void Update()
    {
        if(consumed && !hidden)
        {
            if (positionAdder < 100)
            {
                positionAdder += positionAdder * Time.deltaTime;
                transform.position = new Vector3(transform.position.x, transform.position.y + positionAdder, transform.position.z);
            }
            else
            {
                hidden = true;
                gameObject.SetActive(false);
            }
        }
    }
}
