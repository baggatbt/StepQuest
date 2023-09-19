using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance { get; private set; }

    public GameObject swordWavePrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SpawnAndPushSwordWave(Vector3 spawnPosition, Vector3 direction)
    {
        GameObject swordWave = Instantiate(swordWavePrefab, spawnPosition, Quaternion.identity);

        // Push it forward. Assuming the swordWave has a Rigidbody component.
        Rigidbody rb = swordWave.GetComponent<Rigidbody>();
        if(rb != null)
        {
            float forceAmount = 30f; // Adjust as needed
            rb.AddForce(direction * forceAmount, ForceMode.Impulse);
        }
    }
}
