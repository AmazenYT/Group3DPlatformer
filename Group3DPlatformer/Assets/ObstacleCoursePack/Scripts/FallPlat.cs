using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallPlat : MonoBehaviour
{
    public float fallTime = 0.5f;
    public float respawnTime = 5f;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Renderer platformRenderer;
    private Collider platformCollider;

    void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        platformRenderer = GetComponent<Renderer>();
        platformCollider = GetComponent<Collider>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(FallAndRespawn(fallTime));
        }
    }

    IEnumerator FallAndRespawn(float time)
    {
        yield return new WaitForSeconds(time);
        platformRenderer.enabled = false;
        platformCollider.enabled = false;
        yield return new WaitForSeconds(respawnTime);
        Respawn();
    }

    void Respawn()
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        platformRenderer.enabled = true;
        platformCollider.enabled = true;
    }
}
