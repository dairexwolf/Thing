using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionManager : MonoBehaviour
{
    // Input actions
    InputAction interactAction;

    public static InteractionManager Instance { get; set; }
    [SerializeField] private Transform bulletSpawn;

    private WeaponRays hoveredWeapon = null;


    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        interactAction = InputSystem.actions.FindAction("Interact");
    }


    private void Update()
    {
        Ray ray = new Ray(bulletSpawn.position, bulletSpawn.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            GameObject objectHit = hit.transform.gameObject;

            // WeaponRay
            if (objectHit.GetComponent<WeaponRays>())
            {

                if (interactAction.IsPressed())
                {
                    WeaponManager.Instance.PickupWeapon(objectHit);
                }
            }

            // AmmoBox
            AmmoBox ammo = objectHit.GetComponent<AmmoBox>();
            if (ammo)
            {
                if (interactAction.IsPressed())
                {
                    WeaponManager.Instance.PickupAmmo(ammo);
                    Destroy(objectHit);
                }
            }
        }
    }

    // TODO: В Update стоит прописать доп. логику вызова отрисовки Outline при наведении курсора на объект. Желательно это сделать через интерфейс по типу IOutlinableObject или что то типо того + вызывать не if, а Event-ом
    //void Update()
    //{
    //    Ray ray = new Ray(bulletSpawn.position, bulletSpawn.forward);
    //    RaycastHit hit;

    //    if (Physics.Raycast(ray, out hit))
    //    {
    //        GameObject objectHit = hit.transform.gameObject;

    //        if (objectHit.GetComponent<WeaponRays>())
    //        {
    //            hoveredWeapon = objectHit.GetComponent<WeaponRays>();
    //            hoveredWeapon.GetComponent<Outline>().enabled = true;
    //        }
    //        else
    //        {
    //            if (hoveredWeapon)
    //            {
    //                hoveredWeapon.GetComponent<Outline>().enabled = false;
    //            }
    //        }
    //    }
    //}
}
