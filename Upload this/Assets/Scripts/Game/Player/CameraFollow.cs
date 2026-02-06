using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private Vector3 offset;
    [SerializeField]
    private float damping;
    private Vector3 vel = Vector3.zero;

    public Transform target; //Change waht to look at

    private void LateUpdate()
    {
        Vector3 targetPos = target.position + offset;
        targetPos.z = transform.position.z;
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref vel, damping);
    }
}
