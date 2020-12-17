using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraFollow2 : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed;
    public float angleStep = 4;
    public float wallShown;
    private List<Vector3> contacts = new List<Vector3>();
    private LayerMask layerMask;
    private float height => 2f * Camera.main.orthographicSize;

    // If reply has been given, then update and use! https://answers.unity.com/questions/1796981/stopping-camera-follow-when-wall-is-in-view.html
    void Start()
    {
        layerMask = LayerMask.GetMask("CameraBounds");
    }

    void FixedUpdate()
    {
        contacts.Clear();
        for (int i = 0; i < angleStep; i++)
        {
            var rayHeight = (height / 2) * Mathf.Sin(Mathf.Deg2Rad * ((360 / angleStep) * i));
            var rayLength = (height / 2) * Mathf.Cos(Mathf.Deg2Rad * ((360 / angleStep) * i)) * Camera.main.aspect;
            var ray = new Vector3(rayLength, rayHeight, 0f);
            var hypotenuse = Mathf.Sqrt((ray.x * ray.x) + (ray.y * ray.y));
            RaycastHit2D hit = Physics2D.Raycast(target.position, ray, hypotenuse, layerMask);
            if (hit)
            {
                contacts.Add(ray - ray * (hit.distance / hypotenuse));
                Debug.DrawRay(target.position, ray * (hit.distance / hypotenuse));
            }
            else
                Debug.DrawRay(target.position, ray);
        }
        Vector2 avgOffset = Vector2.zero;
        try
        {
            avgOffset = new Vector3(
                contacts.Average(c => c.x),
                contacts.Average(c => c.y));
        }
        catch (System.Exception) { }
        Vector3 smoothedPos = Vector3.Lerp(transform.position, new Vector3(target.position.x - avgOffset.x, target.position.y - avgOffset.y, transform.position.z), smoothSpeed * Time.fixedDeltaTime);
        transform.position = smoothedPos;
    }
}
