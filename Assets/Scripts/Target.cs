using UnityEngine;

public class Target : MonoBehaviour
{

    private Rigidbody _rigidbody;


    private AudioSource audioSource;

    public void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    public void ShotDown(Vector3 impulse)
    {
        if (_rigidbody != null)
        {
            _rigidbody.isKinematic = false;
            _rigidbody.AddForce(impulse, ForceMode.Impulse);
        }
        else
            Debug.Log("No rigidbody component? o_O");
        audioSource.Play();
    }
}
