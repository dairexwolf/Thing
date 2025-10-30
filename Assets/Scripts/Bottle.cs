using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]

/// <summary>
/// Скрипт бутылки. Считаю, что реализация бутылки в корне не верна - по сути, это должно быть 2 разных объект - нормальная бутылка и разрушенная бутылка, которая подменяет бутылку при разрушении
/// </summary>
public class Bottle : MonoBehaviour
{
    public List<Rigidbody> allParts = new List<Rigidbody>();
    private BoxCollider collider;


    private AudioSource audioSource;

    private void Awake()
    {
        collider = GetComponent<BoxCollider>();
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Уничтожение бутылки и отдача ему силы для отпрасывания.
    /// 
    /// <param name="hitPoint">hitPoint: Куда попал. Стоит отдавать raycastHit.point</param>
    /// <param name="bulletSpawnPosition">bulletSpawnPosition: Откуда стреляли. Стоит отдавать bulletSpawn.position, но с рандомными добавлениями к x и y для того, чтобы разлетались в стороны</param>
    /// <param name="bulletForce">bulletForce: Сила пули, берется из характеристик оружия.</param>
    /// </summary>
    public void Shatter(Vector3 hitPoint, Vector3 bulletSpawnPosition, float bulletForce)
    {
        foreach(Rigidbody part in allParts)
        {
            part.isKinematic = false;
            Vector3 randPos = new Vector3(bulletSpawnPosition.x + Random.Range(-2f, 2f), bulletSpawnPosition.y + Random.Range(-2f, 2f), bulletSpawnPosition.z);
            part.AddForce((hitPoint - randPos).normalized * bulletForce, ForceMode.Impulse);
        }
        collider.enabled = false;
        this.enabled = false;
        audioSource.Play();
    }
}
