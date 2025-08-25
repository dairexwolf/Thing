using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class WeaponRays : MonoBehaviour
{
    public Transform bulletSpawn;
    public float maxDistance = 100f;

    InputAction attackAction;

    RaycastHit raycastHit;

    // Test
    private LineRenderer lineRenderer;
    public float RayLifeTime = 3f;

    private void Awake()
    {
        if (GetComponent<LineRenderer>() == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }
        else
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;

        // Disable LineRenderer before first start
        lineRenderer.enabled = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        attackAction = InputSystem.actions.FindAction("Attack");
    }

    // Update is called once per frame
    void Update()
    {
        // Left mouse click
        if(attackAction.IsPressed())
        {
            FireWeapon();
        }
    }

    private void FireWeapon()
    {


        // Start Ray
        if (Physics.Raycast(bulletSpawn.position, bulletSpawn.forward, out raycastHit, maxDistance))
        {
            // Debug.DrawRay(bulletSpawn.position, bulletSpawn.forward, Color.red, 2f);
            // If player hit anythick
            lineRenderer.SetPosition(0, bulletSpawn.position);
            lineRenderer.SetPosition(1, raycastHit.point);
            
            if (raycastHit.collider != null)
            {
                Debug.Log(raycastHit.distance);
                Debug.Log(raycastHit.collider.gameObject.name);
            }
        }

        else
        {
            // If player missed
            Vector3 endPosition = bulletSpawn.position + bulletSpawn.forward * maxDistance;
            lineRenderer.SetPosition(0, bulletSpawn.position);
            lineRenderer.SetPosition(1, endPosition);
        }

        lineRenderer.enabled = true;

        StartCoroutine(DisableLineRenderer(RayLifeTime));

    }

    private IEnumerator DisableLineRenderer(float delay)
    {
        yield return new WaitForSeconds(delay);
        lineRenderer.enabled = false;
    }
}
