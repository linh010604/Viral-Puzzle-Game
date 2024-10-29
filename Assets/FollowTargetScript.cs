using Unity.VisualScripting;
using UnityEngine;

public class FollowTargetScript : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float TopClamp = 70f;
    [SerializeField] private float BottomClamp = -40f;

    private float cinemachineTargetYaw;
    private float cinemachineTargetPitch;
    // Start is called before the first frame update
    void LateUpdate()
    {
        CameraLogic();
    }
    void CameraLogic()
    {
        float mouseX = GetMouseInput("Mouse X");
        float mouseY = GetMouseInput("Mouse Y");

        cinemachineTargetYaw = UpdateRotation(cinemachineTargetYaw, mouseY, BottomClamp, TopClamp, true);
        cinemachineTargetPitch = UpdateRotation(cinemachineTargetPitch, mouseX, float.MinValue, float.MaxValue, false);

        ApplyRotations(cinemachineTargetPitch, cinemachineTargetYaw);
    }

    private void ApplyRotations(float pitch, float yaw)
    {
        target.rotation = Quaternion.Euler(pitch, yaw, target.eulerAngles.z);
    }
    private float UpdateRotation(float currentRotaion,float input, float min, float max, bool isXAxis)
    {
        currentRotaion += isXAxis ? -input : input;
        return Mathf.Clamp(currentRotaion, min, max);
    }
    private float GetMouseInput(string mouseInputName)
    {
        return Input.GetAxis(mouseInputName) * rotationSpeed * Time.deltaTime;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
