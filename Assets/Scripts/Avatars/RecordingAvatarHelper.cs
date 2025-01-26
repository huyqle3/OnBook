using Fusion;
using UnityEngine;

public class RecordingAvatarHelper : NetworkBehaviour
{
    public Transform headTarget;
    public Transform leftHandTarget;
    public Transform rightHandTarget;
    
    public override void Spawned()
    {
        base.Spawned();

        if (RecordingManager.Instance)
        {
            RecordingManager.Instance.AddToRecordingAvatars(this);
        }
    }
}
