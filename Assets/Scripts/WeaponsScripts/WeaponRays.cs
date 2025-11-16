using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class WeaponRays : MonoBehaviour
{
    public Camera playerCamera;

    // Shooting
    [Header("Shooting")]
    public bool isShooting;
    public bool readyToShoot;
    bool allowReset = true;
    public float shootingDelay = 2f;
    public float bulletForce = 10f;

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

    [Header("Effects")]
    public GameObject muzzleEffect;
    private ParticleSystem particleSystem;
    private AudioSource audioSource;
    private Animator animator;

    [Header("Reloading")]
    public float reloadTime;
    public int magSize, bulletsLeft;
    public bool isReloading;

    // Gun Type Settings
    public enum WeaponModel
    { 
        M1911,
        AK47
    }

    [Header("Gun Type")]
    public WeaponModel thisWeaponModel;



    // Inputs
    InputAction attackAction;
    InputAction reloadAction;

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
        bulletsLeft = magSize;
        #endregion

        audioSource = GetComponent<AudioSource>();
        particleSystem = muzzleEffect.GetComponent<ParticleSystem>();
        animator = GetComponent<Animator>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        attackAction = InputSystem.actions.FindAction("Attack");
        reloadAction = InputSystem.actions.FindAction("Reload");
    }

    // Update is called once per frame
    void Update()
    {
        if (bulletsLeft == 0 && isShooting) SoundManager.Instance.PlayDryFireSound(thisWeaponModel);

        if (currentShootingMode == ShootingMode.Auto)
        {
            isShooting = attackAction.IsPressed();
        }
        else if (currentShootingMode == ShootingMode.Single || currentShootingMode == ShootingMode.Burst)
        {
            isShooting = attackAction.WasPressedThisFrame();
        }

        if (readyToShoot && isShooting && bulletsLeft > 0 && !isReloading)
        {
            burstBulletsLeft = bulletsPerBurst;
            FireWeapon();
            SoundManager.Instance.PlaySootingSound(thisWeaponModel);
            particleSystem.Play();
            animator.SetTrigger("RECOIL");
        }

        if (reloadAction.IsPressed() && bulletsLeft < magSize && !isReloading)
        {
            ReloadWeapon();
        }

        if (readyToShoot && !isShooting && !isReloading && bulletsLeft <= 0)
        {
            ReloadWeapon();
        }

        if (AmmoDisplayManager.Instance.ammoDisplay != null) AmmoDisplayManager.Instance.ammoDisplay.text = $"{bulletsLeft}/{magSize}";

    }

    private void FireWeapon()
    {
        bulletsLeft--;
        readyToShoot = false;

        Vector3 shootingDir = CalculateDirAndSpread().normalized;
        // Start Ray
        RaycastHit[] hits = Physics.RaycastAll(bulletSpawn.position, shootingDir, maxDistance);
        Array.Reverse(hits);
        bool stop = false;

        foreach (RaycastHit raycastHit in hits)
        {
            // Debug.DrawRay(bulletSpawn.position, bulletSpawn.forward, Color.red, 2f);
            // If player hit anything
            lineRenderer.SetPosition(0, bulletSpawn.position);
            lineRenderer.SetPosition(1, raycastHit.point);
            GameObject targetGO = raycastHit.collider.gameObject;
            if (raycastHit.collider != null)
            {
                Collider objectWeHit = raycastHit.collider;
                Debug.Log(raycastHit.distance);
                Debug.Log(objectWeHit.gameObject.name);

                stop = true;

                // Стоит отдаль реализацию этих дейситвий самому объекту, а не этому скрипту. Сделать это можно через делегаты и через ивенты.
                // TODO: посмотреть паттерны событийного проектирования
                if (objectWeHit.gameObject.CompareTag("Target"))
                {
                    if (raycastHit.rigidbody != null)
                        // raycastHit.rigidbody.AddForce((raycastHit.point - bulletSpawn.position).normalized * bulletForce, ForceMode.Impulse);
                        objectWeHit.gameObject.GetComponent<Target>().ShotDown((raycastHit.point - bulletSpawn.position).normalized * bulletForce);
                    else
                        Debug.Log("No rigidbody component? o_O");
                }

                if (objectWeHit.gameObject.CompareTag("Bottle"))
                {
                    Bottle bottleScript = objectWeHit.gameObject.GetComponent<Bottle>();
                    if (bottleScript != null)
                    {
                        if (bottleScript.enabled)
                        {
                            bottleScript.Shatter(raycastHit.point, bulletSpawn.position, bulletForce);
                        }

                    }
                    else
                        raycastHit.rigidbody.AddForce((raycastHit.point - bulletSpawn.position).normalized * bulletForce, ForceMode.Impulse);
                    stop = false;

                }

                if (stop)
                {
                    CreateBulletImpactEffect(raycastHit);
                    break;
                }
            }

            else
            {
                // If player missed
                Vector3 endPosition = bulletSpawn.position + bulletSpawn.forward * maxDistance;
                lineRenderer.SetPosition(0, bulletSpawn.position);
                lineRenderer.SetPosition(1, endPosition);
            }
        }

        lineRenderer.enabled = true;

        StartCoroutine(DisableLineRenderer(RayLifeTime));

        // Checking if we are done shooting
        if (allowReset)
        {
            Invoke("ResetShot", shootingDelay);
            allowReset = false;
        }

        // Burst Mode
        {
            if (currentShootingMode == ShootingMode.Burst && burstBulletsLeft > 1)     // we already shoot once before this check
            {
                burstBulletsLeft--;
                Invoke("FireWeapon", shootingDelay);
            }
        }
    }

    private void ReloadWeapon()
    {
        SoundManager.Instance.PlayReloadingSound(thisWeaponModel);
        animator.SetTrigger("RELOAD");
        isReloading = true;
        Invoke("ReloadComplited", reloadTime);
        
    }

    private void ReloadComplited()
    {
        bulletsLeft = magSize;
        isReloading = false;
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowReset = true;
    }


    public Vector3 CalculateDirAndSpread()
    {
        #region PhysicMethod
        //// Метод из тутора, но у меня к нему вопросы. Напишу чтобы был, так как для физической стрельбы норм

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

        //// Тут получается вопрос, что разброс будет зависить от длины вектора направления, куда попал игрок. А зачем??? И будет ли на самом деле такое?
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

    void CreateBulletImpactEffect(RaycastHit hit)
    {
        GameObject hole = Instantiate(GlobalRefs.Instance.bulletImpactEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
        hole.transform.SetParent(hit.collider.gameObject.transform);
    }

}
