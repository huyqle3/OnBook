using System;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class HandBoneCopier : MonoBehaviour
{
    [Header("Source and Target Transforms")]
    public Transform sourceWrist; // The wrist of the source hand
    public Transform targetWrist; // The wrist of the target hand
    public Transform[] sourceFingerParents; // Parents of each finger in the source hand
    public Transform[] targetFingerParents; // Parents of each finger in the target hand

    [Header("Finger Rotation Increments (Local)")]
    public Vector3 fingerWristIncrement;
    public Vector3[] fingerRotationOffetLevel0; // Local rotation increments for each finger

    public Vector3[] fingerRotationOffsetLevel1;
    public Vector3[] fingerRotationOffsetLevel2;
    
    private void LateUpdate()
    { 
        if (sourceWrist != null && targetWrist != null)
        {
            // Copy the wrist rotation
            CopyWristPositionRotation(sourceWrist, targetWrist);
        }
        
        // Ensure there are equal numbers of source and target finger parents
        if (sourceFingerParents.Length != targetFingerParents.Length || sourceFingerParents.Length != fingerRotationOffetLevel0.Length)
        {
            Debug.LogError("Source and target finger parent arrays and increment arrays must have the same length.");
            return;
        }

        // Copy the finger rotations
        for (int i = 0; i < sourceFingerParents.Length; i++)
        {
            if (sourceFingerParents[i] != null && targetFingerParents[i] != null)
            {
                CopyBoneRotations(sourceFingerParents[i], targetFingerParents[i], i, 0);
            }
            else
            {
                Debug.LogWarning($"Finger parent at index {i} is not set correctly in source or target.");
            }
        }
    }

    private void CopyWristPositionRotation(Transform sourceWrist, Transform targetWrist)
    {
        if (targetWrist == null)
        {
            Debug.LogWarning($"Target bone for source bone {sourceWrist.name} not found.");
            return;
        }

        // Update the wrist position.
        // targetWrist.position = sourceWrist.position;
        
        // Copy the local rotation
        // targetWrist.localRotation = sourceWrist.localRotation * Quaternion.Euler(fingerWristIncrement);
        targetWrist.localEulerAngles = sourceWrist.localRotation * fingerWristIncrement;
    }

    /// <summary>
    /// Recursively copies the local rotations of all child bones from the source to the target.
    /// </summary>
    /// <param name="source">Source transform</param>
    /// <param name="target">Target transform</param>
    private void CopyBoneRotations(Transform source, Transform target, int index, int level)
    {
        if (target == null)
        {
            Debug.LogWarning($"Target bone for source bone {source.name} not found.");
            return;
        }
        
        Vector3 offset = Vector3.zero;

        if (level == 0)
        {
            target.localEulerAngles = source.localEulerAngles + fingerRotationOffetLevel0[index];
            // target.localRotation = source.localRotation * Quaternion.Euler(fingerRotationOffetLevel0[index]);
        }
        else if (level == 1)
        {
            target.localEulerAngles = source.localEulerAngles + fingerRotationOffsetLevel1[index];
            // target.localRotation = source.localRotation * Quaternion.Euler(fingerRotationOffsetLevel1[index]);
        }
        else if (level == 2)
        {
            target.localEulerAngles = source.localEulerAngles + fingerRotationOffsetLevel2[index];
            // target.localRotation = source.localRotation * Quaternion.Euler(fingerRotationOffsetLevel2[index]);
        }

        // Copy the local rotation
        // target.localRotation = source.localRotation * Quaternion.Euler(offset);

        level += 1;

        // Recursively copy rotations for all child bones with the Vector3.zero.
        for (int i = 0; i < source.childCount; i++)
        {
            if (i < target.childCount)
            {
                // CopyBoneRotations(source.GetChild(i), target.GetChild(i), Vector3.zero);
                CopyBoneRotations(source.GetChild(i), target.GetChild(i), index, level);
            }
        }
    }
}
