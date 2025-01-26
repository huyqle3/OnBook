using System;
using System.Collections.Generic;
using Fusion;
using RootMotion.FinalIK;
using UnityEngine;

public class NetworkedAvatarSetup : NetworkBehaviour
{ 
    public virtual bool IsLocalObject => Object && Object.HasStateAuthority;

    public Transform leftSourceWrist; // The wrist of the source hand
    public Transform[] leftSourceFingerParents; // Parents of each finger in the source hand
    public HandBoneCopier leftHandBoneCopier;
    
    public Transform rightSourceWrist; // The wrist of the source hand
    public Transform[] rightSourceFingerParents; // Parents of each finger in the source hand
    public HandBoneCopier rightHandBoneCopier;

    public bool debugMode = false;
    
    public VRIK avatarIK;
    public SkinnedMeshRenderer avatarVisual;

    public List<Transform> feetTransforms = new List<Transform>();
    
    public override void Spawned()
    {
        base.Spawned();
        
        leftHandBoneCopier.sourceWrist = leftSourceWrist;
        leftHandBoneCopier.sourceFingerParents = leftSourceFingerParents;
            
        rightHandBoneCopier.sourceWrist = rightSourceWrist;
        rightHandBoneCopier.sourceFingerParents = rightSourceFingerParents;

        if (IsLocalObject)
        {
            leftHandBoneCopier.enabled = false;
            rightHandBoneCopier.enabled = false;
            
            if (debugMode)
            {
                avatarIK.enabled = true;
                avatarVisual.enabled = true;
            }
            else
            {
                avatarIK.enabled = false;
                avatarVisual.enabled = false;
            }
        }
        else
        {
            avatarIK.enabled = true;
            avatarVisual.enabled = true;
        }
    }

    private void LateUpdate()
    {
        if (IsLocalObject)
        {
            if (debugMode)
            {
                foreach (Transform feetTransform in feetTransforms)
                {
                    feetTransform.localEulerAngles = new Vector3(0, feetTransform.localEulerAngles.y, feetTransform.localEulerAngles.z);
                }
            }
        }
        else
        { 
            foreach (Transform feetTransform in feetTransforms)
            {
                feetTransform.localEulerAngles = new Vector3(0, feetTransform.localEulerAngles.y, feetTransform.localEulerAngles.z);
            }
        }
    }
}
