using UnityEngine;

public class GlobalRefs : MonoBehaviour
{
    public static GlobalRefs Instance { get; set; }

    public GameObject bulletImpactEffectPrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }
}
