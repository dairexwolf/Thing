using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance { get; set; }
    [SerializeField] private Transform bulletSpawn;


    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(bulletSpawn.position, bulletSpawn.forward);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit))
        {
            GameObject objectHit = hit.transform.gameObject;
            if (objectHit != null) print(objectHit.name);
        }
    }
}
