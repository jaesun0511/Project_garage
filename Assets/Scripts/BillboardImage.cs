using UnityEngine;

public class BillboardImage : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private bool lockYRotation = true;
    [SerializeField] private bool flipHorizontally = true;

    private void LateUpdate()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        if (targetCamera == null)
        {
            return;
        }

        Vector3 direction = transform.position - targetCamera.transform.position;

        if (lockYRotation)
        {
            direction.y = 0f;
        }

        if (direction.sqrMagnitude < 0.0001f)
        {
            return;
        }

        Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up);

        if (flipHorizontally)
        {
            lookRotation *= Quaternion.Euler(0f, 180f, 0f);
        }

        transform.rotation = lookRotation;
    }
}
