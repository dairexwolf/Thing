using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponIntantinates : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform bulletSpawn;

    public float bulletVelocity = 30f;
    public float bulletLifeTime = 3f;

    InputAction attackAction;

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
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.Fireing(bulletSpawn.forward, bulletVelocity);
        bulletScript.DestroyBullet(bulletLifeTime);

    }
}
