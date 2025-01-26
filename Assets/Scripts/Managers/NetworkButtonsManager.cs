using NaughtyAttributes;
using Scripts.Utilities;
using TMPro;
using UnityEngine;

public class NetworkButtonsManager : Singleton<NetworkButtonsManager>
{
    private PlayerNetworkCalls MyLocalPlayer;

    private bool _clickedStartRecordingOnce = false;
    public bool _clickedPlayOnce { get; set; } = false;
    
    public TextMeshProUGUI RecordStopText;
    public TextMeshProUGUI PlayPauseText;

    public GameObject PlayPauseButton;
    
    public void AddMyLocalPlayer(PlayerNetworkCalls myLocalPlayer)
    {
        MyLocalPlayer = myLocalPlayer;
    }

    // Triggered by the Red Button
    [Button]
    public void ToggleRecordStop()
    { 
        // MyLocalPlayer.TriggerRPCToggleRecording();
        MyLocalPlayer.RPC_ToggleRecording();
    }

    [Button]
    public void PlayPause()
    {
        MyLocalPlayer.RPC_PlayPause();
    }

    public void OnToggleRecordingResponse()
    {
        if (!_clickedStartRecordingOnce)
        {
            PlaybackManager.Instance.EndPlayback();
            
            // Turn off this button active.
            PlayPauseButton.SetActive(false);
            
            RecordStopText.text = "Stop Recording";
            
            RecordingManager.Instance.StartRecording();
        }
        else
        {
            RecordStopText.text = "Record";
            
            RecordingManager.Instance.StopRecording();
            
            // Set this button active.
            PlayPauseButton.SetActive(true);
        }

        _clickedStartRecordingOnce = !_clickedStartRecordingOnce;
    }

    public void OnTogglePlayPauseResponse()
    {
        if (!_clickedPlayOnce)
        {
            PlayPauseText.text = "Pause";
            
            PlaybackManager.Instance.StartPlayback();
        }
        else
        {
            PlayPauseText.text = "Play";

            PlaybackManager.Instance.TogglePause();
        }

        _clickedPlayOnce = !_clickedPlayOnce;
    }
}
