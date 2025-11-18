using UnityEngine;
using System.Collections.Generic;
using static WeaponRays;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; set; }

    public List<AudioSource> soundsAK47;
    public List<AudioSource> soundsM1911;

    public AudioClip ak47ShootSound;
    public AudioClip m1911ShootSound;
    [SerializeField] private AudioSource shootingChannel;


    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void PlaySootingSound(WeaponModel weapon)
    {
        switch (weapon)
        {
            case WeaponModel.AK47:
                if (shootingChannel) shootingChannel.PlayOneShot(ak47ShootSound);
                break;
            case WeaponModel.M1911:
                if (shootingChannel) shootingChannel.PlayOneShot(m1911ShootSound);
                break;

        }
    }

    public void PlayReloadingSound(WeaponModel weapon)
    {
        switch (weapon)
        {
            case WeaponModel.AK47:
                if (soundsAK47[1]) soundsAK47[1].Play();
                break;
            case WeaponModel.M1911:
                if (soundsM1911[1]) soundsM1911[1].Play();
                break;
        }
    }

    public void PlayDryFireSound(WeaponModel weapon)
    {
        switch (weapon)
        {
            case WeaponModel.AK47:
                if (soundsAK47[0]) soundsAK47[0].Play();
                break;
            case WeaponModel.M1911:
                if (soundsM1911[0]) soundsM1911[0].Play();
                break;
        }
    }
}
