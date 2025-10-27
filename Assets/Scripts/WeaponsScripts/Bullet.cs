using System;
using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Fireing(Vector3 bulletSpawn, float bulletVelocity)
    {
        rb.AddForce(bulletSpawn.normalized * bulletVelocity, ForceMode.Impulse);
    }

    public void DestroyBullet(float bulletLifeTime)
    {
        StartCoroutine(DestroyBulletAfterTime(bulletLifeTime));
    }

    private IEnumerator DestroyBulletAfterTime(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Target"))
        {
            print("hit " + collision.gameObject.name);

            CreateBulletImpactEffect(collision);

            Destroy(gameObject);
        }
    }

    void CreateBulletImpactEffect(Collision objectWeHit)
    {
        ContactPoint contact = objectWeHit.contacts[0];

        GameObject hole = Instantiate(GlobalRefs.Instance.bulletImpactEffectPrefab, contact.point, Quaternion.LookRotation(contact.normal));

        hole.transform.SetParent(objectWeHit.gameObject.transform);



    }
}
