using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    Transform target;
    public Transform Target
    {
        set { target = value; }
    }

    [SerializeField] float smoothSpeed;

    void FixedUpdate()
    {
        if (target == null)
            return;

        Vector3 targetPos = new Vector3(target.position.x, target.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);
    }
}
