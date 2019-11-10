
using UnityEngine;

public class Camera : MonoBehaviour
{
    public Transform target; // Can be used for what ever we wish to focus on (i.e puzzles)
    public float smoothSpeed = 0.005f;  // The higher the number the snappier it is, lower the smoother
    public Vector3 offset; // Pushes camera -10 back but left open to editor for easy access
    private void LateUpdate() //Same as update but runs after / ensure all movement has happened first
    {
        transform.position = target.position + offset; // Target's position with offset applied
    }
}
