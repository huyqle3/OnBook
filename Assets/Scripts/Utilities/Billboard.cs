using UnityEngine;

public enum PivotAxis
{
    // Rotate about all axes.
    Free,
    // Rotate about an individual axis.
    Y
}

/// <summary>
/// The Billboard class implements the behaviors needed to keep a GameObject oriented towards the user.
/// </summary>
public class Billboard : MonoBehaviour
{
    /// <summary>
    /// The axis about which the object will rotate.
    /// </summary>
    [Tooltip("Specifies the axis about which the object will rotate.")]
    public PivotAxis PivotAxis = PivotAxis.Free;

    [Tooltip("Specifies the target we will orient to. If no Target is specified the main camera will be used.")]
    public Transform TargetTransform;

    private void OnEnable()
    {
        if (TargetTransform == null)
        {
            TargetTransform = Camera.main.transform;
        }

        Update();
    }

    /// <summary>
    /// Keeps the object facing the camera.
    /// </summary>
    private void Update()
    {
        if (TargetTransform == null)
        {
            return;
        }

        // Get a Vector that points from the target to the main camera.
        Vector3 directionToTarget = TargetTransform.position - transform.position;

        // Adjust for the pivot axis.
        switch (PivotAxis)
        {
            case PivotAxis.Y:
                directionToTarget.y = 0.0f;
                break;

            case PivotAxis.Free:
            default:
                // No changes needed.
                break;
        }

        // If we are right next to the camera the rotation is undefined. 
        if (directionToTarget.sqrMagnitude < 0.001f)
        {
            return;
        }

        // Calculate and apply the rotation required to reorient the object
        transform.rotation = Quaternion.LookRotation(-directionToTarget);
    }
}