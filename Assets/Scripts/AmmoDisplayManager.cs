using TMPro;
using UnityEngine;

public class AmmoDisplayManager : MonoBehaviour
{
    public static AmmoDisplayManager Instance { get; set; }

    [Header("UI")]
    public TextMeshProUGUI ammoDisplay;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }
}
