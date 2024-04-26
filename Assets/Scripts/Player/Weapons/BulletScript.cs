using Riptide;
using System.Collections;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    [SerializeField] Rigidbody2D rbody;

    [SerializeField] private float bulletForce;
    [SerializeField] private float bulletTime;

    private void Awake()
    {
        StartCoroutine(Shoot(bulletTime));
    }

    IEnumerator Shoot(float time)
    {
        rbody.AddForce(transform.right * bulletForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
