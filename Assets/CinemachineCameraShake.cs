using Cinemachine;
using UnityEngine;

public class CinemachineCameraShake : MonoBehaviour
{
    public static CinemachineCameraShake Instance { get; private set; }

    private CinemachineFreeLook cinemachineFreeLook;
    private float shakeTimer;

    // Store the Perlin noise component for each rig (Top, Middle, Bottom)
    private CinemachineBasicMultiChannelPerlin topRigNoise;
    private CinemachineBasicMultiChannelPerlin middleRigNoise;
    private CinemachineBasicMultiChannelPerlin bottomRigNoise;

    private float defaultTopRigAmplitude;
    private float defaultMiddleRigAmplitude;
    private float defaultBottomRigAmplitude;

    private void Awake()
    {
        Instance = this;

        // Get the Cinemachine FreeLook camera component
        cinemachineFreeLook = GetComponent<CinemachineFreeLook>();

        if (cinemachineFreeLook != null)
        {

            // Get the Perlin noise component from each rig of the FreeLook camera
            topRigNoise = cinemachineFreeLook.GetRig(0).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            middleRigNoise = cinemachineFreeLook.GetRig(1).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            bottomRigNoise = cinemachineFreeLook.GetRig(2).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            // Store the default amplitude gain values to reset after shaking
            if (topRigNoise != null) defaultTopRigAmplitude = topRigNoise.m_AmplitudeGain;
            if (middleRigNoise != null) defaultMiddleRigAmplitude = middleRigNoise.m_AmplitudeGain;
            if (bottomRigNoise != null) defaultBottomRigAmplitude = bottomRigNoise.m_AmplitudeGain;
        }
    }

    public void ShakeCamera(float intensity, float time)
    {
        if (topRigNoise != null && middleRigNoise != null && bottomRigNoise != null)
        {
            // Set the amplitude gain to create the shake effect for each rig
            topRigNoise.m_AmplitudeGain = intensity;
            middleRigNoise.m_AmplitudeGain = intensity;
            bottomRigNoise.m_AmplitudeGain = intensity;

            // Set the shake duration
            shakeTimer = time;
        }
    }

    private void Update()
    {
        // Handle the shake timer to reset the noise after the shake duration
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;

            if (shakeTimer <= 0f)
            {
                // Reset the amplitude gain to the original values after the shake duration
                ResetShake();
            }
        }
    }

    // Method to reset the shake (set amplitude back to default values)
    private void ResetShake()
    {
        if (topRigNoise != null) topRigNoise.m_AmplitudeGain = defaultTopRigAmplitude;
        if (middleRigNoise != null) middleRigNoise.m_AmplitudeGain = defaultMiddleRigAmplitude;
        if (bottomRigNoise != null) bottomRigNoise.m_AmplitudeGain = defaultBottomRigAmplitude;
    }
}
