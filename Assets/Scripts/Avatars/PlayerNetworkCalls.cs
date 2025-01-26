using Fusion;
using UnityEngine;

public class PlayerNetworkCalls : NetworkBehaviour
{
    public virtual bool IsLocalObject => Object && Object.HasStateAuthority;
    
    public override void Spawned()
    {
        base.Spawned();

        if (IsLocalObject)
        {
            if (NetworkButtonsManager.Instance)
            {
                NetworkButtonsManager.Instance.AddMyLocalPlayer(this);
            }
        }
    }

    public void TriggerRPCToggleRecording()
    {
        RPC_ToggleRecording();
    }
    
    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_ToggleRecording()
    {
        if (NetworkButtonsManager.Instance)
        {
            NetworkButtonsManager.Instance.OnToggleRecordingResponse();
        }
    }
    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_PlayPause()
    {
        if (NetworkButtonsManager.Instance)
        {
            NetworkButtonsManager.Instance.OnTogglePlayPauseResponse();
        }
    }
}
