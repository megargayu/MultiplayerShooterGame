using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    public float smoothSpeed = 30f;
    [Range(0, 1)] public float wallShown;

    private Vector3 offset;
    private LayerMask layerMask;
    private Camera cam;

    void Start()
    {
        layerMask = LayerMask.GetMask("CameraBounds");
        cam = GetComponent<Camera>();
    }

    void FixedUpdate()
    {
        if (target)
        {
            float rayHeight = cam.orthographicSize;
            float rayWidth = cam.orthographicSize * cam.aspect;
            offset = Vector3.zero;

            ChangeOffset(new Vector3(0, 1), rayHeight - wallShown);
            ChangeOffset(new Vector3(0, -1), rayHeight - wallShown);
            ChangeOffset(new Vector3(1, 0), rayWidth - wallShown);
            ChangeOffset(new Vector3(-1, 0), rayWidth - wallShown);

            // Smooth camera
            Vector3 smoothedPos = Vector3.Lerp(transform.position, new Vector3(target.position.x - offset.x, target.position.y - offset.y, transform.position.z), smoothSpeed * Time.fixedDeltaTime);
            transform.position = smoothedPos;
        }
    }

    void ChangeOffset(Vector3 rayAngle, float rayLength)
    {
        RaycastHit2D hit = Physics2D.Raycast(target.position, rayAngle, rayLength, layerMask);
        if (hit)
        {
            offset += (rayAngle * rayLength) - (rayAngle * hit.distance);
            Debug.DrawRay(target.position, rayAngle * hit.distance);
        }
        else
            Debug.DrawRay(target.position, rayAngle * rayLength);
    }
}
