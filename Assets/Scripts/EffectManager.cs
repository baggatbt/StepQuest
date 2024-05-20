using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void PlayEffect(GameObject effectPrefab, Vector3 position)
    {
        Instantiate(effectPrefab, position, Quaternion.identity);
    }
}
