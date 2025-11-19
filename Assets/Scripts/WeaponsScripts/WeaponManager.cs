using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance { get; set; }

    [SerializeField] private Transform bulletSpawn;
    [SerializeField] private Camera playerCamera;

    float dropForce=5f;

    public List<GameObject> weaponSlots;

    public GameObject activeWeaponSlot;

    // Input actions
    InputAction switchWeaponAction;

    // Переключатель между первым и вторым слотом
    bool isFirstSlot = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        switchWeaponAction = InputSystem.actions.FindAction("Previous");
    }

    private void Start()
    {
        activeWeaponSlot = weaponSlots[0];
    }

    private void Update()
    {
        // Это очень неправильно, обязательно надо переделать
        foreach(GameObject weaponSlot in weaponSlots)
        {
            if (weaponSlot == activeWeaponSlot) weaponSlot.SetActive(true);
            else weaponSlot.SetActive(false);
        }

        // Если нажимаем на соотвествующую кнопку, меняем слот

        if (switchWeaponAction.WasPressedThisFrame())
        {
            switch (isFirstSlot)
            {
                case true:
                    SwitchActiveSlot(1);
                    isFirstSlot = false;
                    break;
                case false:
                    SwitchActiveSlot(0);
                    isFirstSlot = true;
                    break;
            } 
        }
    }

    /// <summary>
    /// Метод для поднятия оружия
    /// </summary>
    /// <param name="pickedupWeapon"></param>
    public void PickupWeapon(GameObject pickedupWeapon)
    {
        AddWeaponIntoActiveSlot(pickedupWeapon);
    }

    /// <summary>
    /// Метод для вставки оружия в активный слот
    /// </summary>
    /// <param name="pickedupWeapon"></param>
    private void AddWeaponIntoActiveSlot(GameObject pickedupWeapon)
    {
        DropCurrentWeapon();

        pickedupWeapon.transform.SetParent(activeWeaponSlot.transform, false);

        WeaponRays weapon = pickedupWeapon.GetComponent<WeaponRays>();

        pickedupWeapon.transform.localPosition = new Vector3(weapon.spawnPosition.x, weapon.spawnPosition.y, weapon.spawnPosition.z);
        pickedupWeapon.transform.localRotation = Quaternion.Euler(weapon.spawnRotation.x, weapon.spawnRotation.y, weapon.spawnRotation.z);
        weapon.playerCamera = this.playerCamera;
        weapon.bulletSpawn = this.bulletSpawn;
        weapon.Animator.enabled = true;
        pickedupWeapon.GetComponent<Rigidbody>().constraints=RigidbodyConstraints.FreezeAll;

        weapon.isActiveWeapon = true;

    }

    /// <summary>
    /// Метод, чтобы бросить текущее оружие
    /// </summary>
    private void DropCurrentWeapon()
    {
        if(activeWeaponSlot.transform.childCount>0)
        {
            GameObject weaponToDrop = activeWeaponSlot.transform.GetChild(0).gameObject;

            WeaponRays weaponScript = weaponToDrop.GetComponent<WeaponRays>();
            weaponScript.isActiveWeapon = false;
            weaponScript.playerCamera=null;
            weaponScript.bulletSpawn = null;
            weaponScript.Animator.enabled = false;
            Rigidbody weaponRigidbody = weaponToDrop.GetComponent<Rigidbody>();
            weaponRigidbody.constraints = RigidbodyConstraints.None;
            weaponToDrop.transform.parent = null;
            weaponRigidbody.AddForce(bulletSpawn.forward * dropForce);
        }
    }

    public void SwitchActiveSlot(int slotNumber)
    {
        if(activeWeaponSlot.transform.childCount>0)
        {
            WeaponRays currentWeapon = activeWeaponSlot.transform.GetChild(0).GetComponent<WeaponRays>();
            currentWeapon.isActiveWeapon = false;
        }

        activeWeaponSlot = weaponSlots[slotNumber];

        if (activeWeaponSlot.transform.childCount > 0)
        {
            WeaponRays currentWeapon = activeWeaponSlot.transform.GetChild(0).GetComponent<WeaponRays>();
            currentWeapon.isActiveWeapon = true;
        }
    }
}
