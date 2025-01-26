using System.Collections.Generic;
using NaughtyAttributes;
using Scripts.Utilities;
using UnityEngine;

public class RecordingManager : Singleton<RecordingManager>
{
    public List<RecordingAvatarHelper> recordingAvatars { get; set; } = new List<RecordingAvatarHelper>();
    
    private bool isRecording = false;
    private float recordingStartTime;
    
    public List<BodyFrameData> recordedData { get; set; } = new List<BodyFrameData>();

    private bool _clickedOnce = false;

    public void ToggleClick()
    {
        if (!_clickedOnce)
        {
            StartRecording();
            _clickedOnce = !_clickedOnce;
        }
        else
        {
            StopRecording();
            _clickedOnce = !_clickedOnce;
        }
    }
    
    [System.Serializable]
    public class BodyFrameData
    {
        public float timestamp;
        public List<PlayerBoneData> playerBoneData = new List<PlayerBoneData>();
        // public List<MovementRecorder.HumanoidBoneData> humanoidBones = new List<MovementRecorder.HumanoidBoneData>();
    }

    [System.Serializable]
    public class PlayerBoneData
    {
        public int actorId;
        public MovementRecorder.HumanoidBoneData headData;
        public MovementRecorder.HumanoidBoneData leftHandData;
        public MovementRecorder.HumanoidBoneData rightHandData;
    }
    
    public void AddToRecordingAvatars(RecordingAvatarHelper myself)
    {
        recordingAvatars.Add(myself);
    }
    
    private void FixedUpdate()
    {
        if (isRecording)
        {
            RecordFrameData();
        }
    }
    
    /// <summary>
    /// Starts the recording process.
    /// </summary>
    [Button]
    public void StartRecording()
    {
        if (isRecording)
        {
            Debug.LogWarning("Recording is already in progress.");
            return;
        }

        Debug.Log("Starting movement recording...");
        recordedData.Clear();
        recordingStartTime = Time.time;
        isRecording = true;
    }
    
    [Button]
    public void StopRecording()
    {
        if (!isRecording)
        {
            Debug.LogWarning("Recording is not currently active.");
            return;
        }

        Debug.Log("Stopping movement recording...");
        isRecording = false;
    }

    private void RecordFrameData()
    {
        if (recordingAvatars.Count > 0)
        {
            BodyFrameData frameData = new BodyFrameData
            {
                timestamp = Time.time - recordingStartTime
            };

            for (int i = 0; i < recordingAvatars.Count; i++)
            {
                var avatar = recordingAvatars[i];

                PlayerBoneData playerBoneData = new PlayerBoneData();
                MovementRecorder.HumanoidBoneData headData = new MovementRecorder.HumanoidBoneData
                {
                    position = recordingAvatars[i].headTarget.position,
                    rotation = recordingAvatars[i].headTarget.rotation,
                };

                MovementRecorder.HumanoidBoneData leftHandData = new MovementRecorder.HumanoidBoneData
                {
                    position = recordingAvatars[i].leftHandTarget.position,
                    rotation = recordingAvatars[i].leftHandTarget.rotation,
                };

                MovementRecorder.HumanoidBoneData rightHandData = new MovementRecorder.HumanoidBoneData
                {
                    position = recordingAvatars[i].rightHandTarget.position,
                    rotation = recordingAvatars[i].rightHandTarget.rotation,
                };

                playerBoneData.headData = headData;
                playerBoneData.leftHandData = leftHandData;
                playerBoneData.rightHandData = rightHandData;

                frameData.playerBoneData.Add(playerBoneData);
            }

            recordedData.Add(frameData);
        }
    }
}
