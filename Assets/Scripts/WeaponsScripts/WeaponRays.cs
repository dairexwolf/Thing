using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class WeaponRays : MonoBehaviour
{
    public Camera playerCamera;

    // Shooting
    [Header("Shooting")]
    public bool isShooting;
    public bool readyToShoot;
    bool allowReset = true;
    public float shootingDelay = 2f;

    // Burst
    [Header("Burst")]
    public int bulletsPerBurst = 3;
    public int burstBulletsLeft;

    // Spread
    [Header("Spread")]
    public float spreadIntensity;

    // Shooting Mode settings
    public enum ShootingMode
    {
        Single,
        Burst,
        Auto
    }

    [Header("Shooting Mode")]
    public ShootingMode currentShootingMode;



    public Transform bulletSpawn;
    public float maxDistance = 100f;

    InputAction attackAction;

    RaycastHit raycastHit;

    // Test
    private LineRenderer lineRenderer;
    public float RayLifeTime = 3f;


    private void Awake()
    {
        #region LineRenderer Settings
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
        #endregion


        #region Shooting Settings
        readyToShoot = true;
        burstBulletsLeft = bulletsPerBurst;
        #endregion
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        attackAction = InputSystem.actions.FindAction("Attack");
    }

    // Update is called once per frame
    void Update()
    {
        if(currentShootingMode==ShootingMode.Auto)
        {
            isShooting = attackAction.IsPressed();
        }
        else if(currentShootingMode==ShootingMode.Single || currentShootingMode==ShootingMode.Burst)
        {
            isShooting = attackAction.WasPressedThisFrame();
        }

        if(readyToShoot && isShooting)
        {
            burstBulletsLeft = bulletsPerBurst;
            FireWeapon();
        }
    }

    private void FireWeapon()
    {
        readyToShoot = false;

        Vector3 shootingDir = CalculateDirAndSpread().normalized;

        // Start Ray
        if (Physics.Raycast(bulletSpawn.position, shootingDir, out raycastHit, maxDistance))
        {
            // Debug.DrawRay(bulletSpawn.position, bulletSpawn.forward, Color.red, 2f);
            // If player hit anything
            lineRenderer.SetPosition(0, bulletSpawn.position);
            lineRenderer.SetPosition(1, raycastHit.point);
            
            if (raycastHit.collider != null)
            {
                Debug.Log(raycastHit.distance);
                Debug.Log(raycastHit.collider.gameObject.name);
                if(raycastHit.collider.gameObject.CompareTag("Target"))
                {
                    if (raycastHit.rigidbody != null)
                        raycastHit.rigidbody.AddForce((raycastHit.point - bulletSpawn.position).normalized * 10f, ForceMode.Impulse);
                    else
                        Debug.Log("No rigidbody component? o_O");
                }
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

        // Checking if we are done shooting
        if(allowReset)
        {
            Invoke("ResetShot", shootingDelay);
            allowReset = false;
        }

        // Burst Mode
        {
            if(currentShootingMode == ShootingMode.Burst && burstBulletsLeft>1)     // we already shoot once before this check
            {
                burstBulletsLeft--;
                Invoke("FireWeapon", shootingDelay);
            }
        }
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowReset = true;
    }

    
    public Vector3 CalculateDirAndSpread()
    {
        #region PhysicMethod
        //// ћетод из тутора, но у мен€ к нему вопросы. Ќапишу чтобы был, так как дл€ физической стрельбы норм

        //// Shooting from the middle of the screen to check where are pointing at
        //Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        //RaycastHit hit;

        //Vector3 targetPoint;
        //if(Physics.Raycast(ray, out hit))
        //{
        //    // Hitting
        //    targetPoint = hit.point;
        //}
        //else
        //{
        //    // Shooting at the air
        //    targetPoint = ray.GetPoint(100f);
        //}

        //// “ут получаетс€ вопрос, что разброс будет зависить от длины вектора направлени€, куда попал игрок. ј зачем??? » будет ли на самом деле такое?
        //Vector3 dir = targetPoint - bulletSpawn.position;


        //float x = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        //float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);

        //// Returning the shoot dir and spread
        //return dir + new Vector3(x, y, 0);
        #endregion

        #region MyMethod
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 targetPoint = ray.GetPoint(100f);
        Vector3 dir = targetPoint - bulletSpawn.position;

        float x = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        Debug.DrawRay(bulletSpawn.position, dir, Color.red, 2f);

        // Returning the shoot dir and spread
        return dir + new Vector3(x, y, 0);
        #endregion
    }

    private IEnumerator DisableLineRenderer(float delay)
    {
        yield return new WaitForSeconds(delay);
        lineRenderer.enabled = false;
    }

}
