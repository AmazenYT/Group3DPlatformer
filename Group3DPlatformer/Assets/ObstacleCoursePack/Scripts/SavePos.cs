using UnityEngine;

public class SavePos : MonoBehaviour
{
    public Transform checkPoint;

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            col.GetComponent<CharacterControls>().checkPoint = checkPoint.position;
        }
    }
}
