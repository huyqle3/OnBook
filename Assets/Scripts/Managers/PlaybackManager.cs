using System.Collections.Generic;
using NaughtyAttributes;
using RootMotion.FinalIK;
using Scripts.Utilities;
using UnityEngine;

public class PlaybackManager : Singleton<PlaybackManager>
{
    private bool isPlaying = false;
    private bool isPaused = false; // To track pause state
    private float playbackStartTime;
    private float pauseTimeOffset; // To maintain elapsed time during pause
    private int currentFrameIndex = 0;

    public GameObject emptyAvatarPrefab;
    private List<GameObject> _cachedAvatars = new List<GameObject>();
    private List<PlaybackAvatarHelper> _playbackAvatarHelpers = new List<PlaybackAvatarHelper>();

    public List<Color> playbackColors = new List<Color>();
    
    [Button]
    public void StartPlayback()
    {
        foreach (GameObject avatar in _cachedAvatars)
        {
            if (avatar)
            {
                Destroy(avatar);
            }
        }

        if (RecordingManager.Instance)
        {
            for (int i = 0; i < RecordingManager.Instance.recordingAvatars.Count; i++)
            {
                GameObject emptyAvatar = Instantiate(emptyAvatarPrefab);
                _cachedAvatars.Add(emptyAvatar);
                PlaybackAvatarHelper playbackAvatarHelper = emptyAvatar.GetComponent<PlaybackAvatarHelper>();
                _playbackAvatarHelpers.Add(playbackAvatarHelper);
                
                // Change the color of the playbackAvatar
                if (playbackAvatarHelper.armatureMesh)
                {
                    // Iterate through all materials and change their color
                    foreach (Material material in playbackAvatarHelper.armatureMesh.materials)
                    {
                        if (material.HasProperty("_Color")) // Ensure the material has a _Color property
                        {
                            if (i < playbackColors.Count)
                            {
                                material.color = playbackColors[i];
                            }
                        }
                    }
                }
            }
        }
        
        playbackStartTime = Time.time; // Set playback start time
        pauseTimeOffset = 0f; // Reset pause offset
        currentFrameIndex = 0; // Reset to the first frame
        isPlaying = true;
        isPaused = false;
    }

    private void Update()
    {
        if (isPlaying && !isPaused && currentFrameIndex < RecordingManager.Instance.recordedData.Count)
        {
            float elapsedTime = (Time.time - playbackStartTime) + pauseTimeOffset;

            while (currentFrameIndex < RecordingManager.Instance.recordedData.Count &&
                   elapsedTime >= RecordingManager.Instance.recordedData[currentFrameIndex].timestamp)
            {
                ApplyFrameData(RecordingManager.Instance.recordedData[currentFrameIndex]);
                currentFrameIndex++;
            }

            if (currentFrameIndex >= RecordingManager.Instance.recordedData.Count)
            {
                EndPlayback();
            }
        }
    }
    
    [Button]
    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            // Pause playback
            pauseTimeOffset += Time.time - playbackStartTime; // Calculate elapsed time before pausing
            // audioSource.Pause();
            Debug.Log("Playback paused.");
        }
        else
        {
            // Resume playback
            playbackStartTime = Time.time; // Reset playback start time
            // audioSource.Play();
            Debug.Log("Playback resumed.");
        }
    }
    
    public void EndPlayback()
    {
        Debug.Log("Playback finished. All frames have been applied.");
        isPlaying = false;
        isPaused = false;

        if (NetworkButtonsManager.Instance)
        {
            // RESET THIS.
            NetworkButtonsManager.Instance._clickedPlayOnce = false;
            NetworkButtonsManager.Instance.PlayPauseText.text = "Play";
        }

        foreach (GameObject avatar in _cachedAvatars)
        {
            if (avatar)
            {
                Destroy(avatar);
            }
        }
        
        _cachedAvatars.Clear();
        _playbackAvatarHelpers.Clear();
    }
    
    void ApplyFrameData(RecordingManager.BodyFrameData frameData)
    {
        for (int i = 0; i < frameData.playerBoneData.Count; i++)
        { 
            PlaybackAvatarHelper playbackAvatarHelper = _playbackAvatarHelpers[i];
            RecordingManager.PlayerBoneData playerBoneData = frameData.playerBoneData[i];

            if (playbackAvatarHelper.headTarget != null && playerBoneData.headData != null)
            {
                playbackAvatarHelper.headTarget.position = playerBoneData.headData.position;
                playbackAvatarHelper.headTarget.rotation = playerBoneData.headData.rotation;
            }

            if (playbackAvatarHelper.leftHandTarget != null && playerBoneData.leftHandData != null)
            {
                playbackAvatarHelper.leftHandTarget.position = playerBoneData.leftHandData.position;
                playbackAvatarHelper.leftHandTarget.rotation = playerBoneData.leftHandData.rotation;
            }


            if (playbackAvatarHelper.rightHandTarget != null && playerBoneData.rightHandData != null)
            {
                playbackAvatarHelper.rightHandTarget.position = playerBoneData.rightHandData.position;
                playbackAvatarHelper.rightHandTarget.rotation = playerBoneData.rightHandData.rotation;
            }
        }
        
        /*
        if (avatarBones.Length == 0)
        {
            Debug.LogError("No avatar bones found. Ensure the avatar is correctly configured.");
            return;
        }

        foreach (var humanoidBoneData in frameData.humanoidBones)
        {
            string cleanBoneName = humanoidBoneData.boneName.Replace("_", "");

            foreach (HumanBodyBones humanBone in System.Enum.GetValues(typeof(HumanBodyBones)))
            {
                string humanBoneName = humanBone.ToString();
                if (cleanBoneName.Equals(humanBoneName, System.StringComparison.OrdinalIgnoreCase))
                {
                    Transform boneTransform = avatar.GetComponent<Animator>().GetBoneTransform(humanBone);
                    if (boneTransform != null)
                    {
                        boneTransform.localPosition = humanoidBoneData.position;
                        boneTransform.localRotation = humanoidBoneData.rotation;
                    }
                    else
                    {
                        Debug.LogWarning($"Bone '{humanBoneName}' not found on the avatar's Animator.");
                    }
                    break;
                }
            }
        }
        */
    }
}
