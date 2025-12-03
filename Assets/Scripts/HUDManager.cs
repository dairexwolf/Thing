using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; set; }

    [Header("Ammo")]
    public TextMeshProUGUI magazineAmmoUI;
    public TextMeshProUGUI totalAmmoUI;
    public Image ammoTypeUI;

    [Header("Weapon")]
    public TextMeshProUGUI activeWeaponUI;
    public TextMeshProUGUI unActiveWeaponUI;

    [Header("Throwables")]
    public Image granadeUI;
    public TextMeshProUGUI granadeAmountUI;

    public Image tacticalUI;
    public TextMeshProUGUI tacticalAmountUI;

    public Sprite emptySlot;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    // Ясеное дело оно не должно так работать. 
    // TODO: подписка на событие выстрела, смены оружия и т.д.
    private void Update()
    {
        WeaponRays activeWeapon = WeaponManager.Instance.activeWeaponSlot.GetComponentInChildren<WeaponRays>();
        WeaponRays unActiveWeapon = GetUnActiveSlot().GetComponentInChildren<WeaponRays>(true);

        if (activeWeapon)
        {
            magazineAmmoUI.text = $"{activeWeapon.bulletsLeft}";
            totalAmmoUI.text = WeaponManager.Instance.CheckAmmoLeftFor(activeWeapon.thisWeaponModel).ToString();

            WeaponRays.WeaponModel type = activeWeapon.thisWeaponModel;
            ammoTypeUI.sprite = GetAmmoSprite(type);

            activeWeaponUI.text = $"{WeaponManager.Instance.GetWeaponModel(activeWeapon)}";

            if (unActiveWeapon)
            {
                unActiveWeaponUI.text = $"{WeaponManager.Instance.GetWeaponModel(unActiveWeapon)}";
            }
        }
        else
        {
            magazineAmmoUI.text = "";
            totalAmmoUI.text = "";

            ammoTypeUI.sprite = emptySlot;
            activeWeaponUI.text = "";
            unActiveWeaponUI.text = "";
        }
    }

    private GameObject GetUnActiveSlot()
    {
        if (WeaponManager.Instance.IsFirstSlot)
        {
            return WeaponManager.Instance.rifleSlot;
        }

        return WeaponManager.Instance.pistolSlot;

    }

    /// <summary>
    /// Способ под слоты, которые могут вмещать в себя все
    /// </summary>
    /// <returns></returns>
    //private GameObject GetUnActiveSlotAnother()
    //{
    //    foreach (GameObject ws in WeaponManager.Instance.weaponSlots)
    //    {
    //        if (ws != WeaponManager.Instance.activeWeaponSlot) return ws;
    //    }
    //    return null;
    //}



    private Sprite GetAmmoSprite(WeaponRays.WeaponModel weaponType)
    {
        switch(weaponType)
        {
            case WeaponRays.WeaponModel.M1911:
                return Resources.Load<GameObject>("PistolAmmo").GetComponent<SpriteRenderer>().sprite;

            case WeaponRays.WeaponModel.AK47:
                return Resources.Load<GameObject>("RifleAmmo").GetComponent<SpriteRenderer>().sprite;

            default:    
                return null;
        }
    }

    private Sprite GetWeaponSprite(WeaponRays.WeaponModel weaponType)
    {
        switch (weaponType)
        {
            case WeaponRays.WeaponModel.M1911:
                return Instantiate(Resources.Load<GameObject>("Pistol_Weapon")).GetComponent<SpriteRenderer>().sprite;

            case WeaponRays.WeaponModel.AK47:
                return Instantiate(Resources.Load<GameObject>("Rifle_Weapon")).GetComponent<SpriteRenderer>().sprite;

            default:
                return null;
        }
    }
}
