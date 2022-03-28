using UnityEngine;

public class GunTurret : MonoBehaviour
{
    public Transform turretHead;
    public Transform rearPoint;
    public Transform midPoint;
    public Transform frontPoint;
    public Vector3 rightHandlePos;
    public Vector3 leftHandlePos;
    public Vector3 rightHandleVolume;
    public Vector3 leftHandleVolume;

    private ControllerInput currentRightHandleUser;
    private bool rightHandleGripped;
    private bool prevRightHandleGripped;
    private ControllerInput grippingRightHandleUser;
    private Vector3 startRightHandlePos;
    private ControllerInput currentLeftHandleUser;
    private bool leftHandleGripped;
    private bool prevLeftHandleGripped;
    private ControllerInput grippingLeftHandleUser;
    private Vector3 startLeftHandlePos;

    private Vector3 startRearPos;

    void Update()
    {
        currentRightHandleUser = GetInputInVolume(turretHead.TransformPoint(rightHandlePos), rightHandleVolume, turretHead.rotation);
        currentLeftHandleUser = GetInputInVolume(turretHead.TransformPoint(leftHandlePos), leftHandleVolume, turretHead.rotation);
        CheckGrippage();

        if (rightHandleGripped || leftHandleGripped)
        {
            if (!prevRightHandleGripped && !prevLeftHandleGripped)
                startRearPos = rearPoint.position;
            if (rightHandleGripped && !prevRightHandleGripped)
                startRightHandlePos = grippingRightHandleUser.transform.position;
            if (leftHandleGripped && !prevLeftHandleGripped)
                startLeftHandlePos = grippingLeftHandleUser.transform.position;
            Vector3 userHandlePos = startRearPos;
            Vector3 offsetPos = Vector3.zero;
            int average = 0;
            if (rightHandleGripped)
            {
                offsetPos += grippingRightHandleUser.transform.position - startRightHandlePos;
                average++;
            }
            if (leftHandleGripped)
            {
                offsetPos += grippingLeftHandleUser.transform.position - startLeftHandlePos;
                average++;
            }
            offsetPos /= average; //No need to worry about dividing by zero since we know in here at least right hand or left hand is gripping so average will have to be at least 1
            Vector3 aimDir = (midPoint.position - (startRearPos + offsetPos)).normalized;
            turretHead.rotation = Quaternion.LookRotation(aimDir, transform.up);
        }
    }
    void LateUpdate()
    {
        prevRightHandleGripped = rightHandleGripped;
        prevLeftHandleGripped = leftHandleGripped;
        // startRightHandlePos = rightHandleUser.transform.position;
        // startLeftHandlePos = leftHandleUser.transform.position;
        // grippingRightHandleUser = currentRightHandleUser;
        // grippingLeftHandleUser = currentLeftHandleUser;
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.matrix = turretHead.localToWorldMatrix;
        Gizmos.DrawWireCube(rightHandlePos, rightHandleVolume);
        Gizmos.DrawWireCube(leftHandlePos, leftHandleVolume);
    }

    private void CheckGrippage()
    {
        if (currentRightHandleUser != null && currentRightHandleUser.isGripping)
        {
            grippingRightHandleUser = currentRightHandleUser;
            rightHandleGripped = true;
        }
        else if (rightHandleGripped && !grippingRightHandleUser.isGripping)
            rightHandleGripped = false;
        if (currentLeftHandleUser != null && currentLeftHandleUser.isGripping)
        {
            grippingLeftHandleUser = currentLeftHandleUser;
            leftHandleGripped = true;
        }
        else if (leftHandleGripped && !grippingLeftHandleUser.isGripping)
            leftHandleGripped = false;
    }
    private static ControllerInput GetInputInVolume(Vector3 pos, Vector3 size, Quaternion rot)
    {
        RaycastHit[] objsInRightHandle = Physics.BoxCastAll(pos, size / 2, Vector3.forward, rot, float.Epsilon, ~0, QueryTriggerInteraction.Collide);
        for (int i = 0; i < objsInRightHandle.Length; i++)
        {
            var currentObj = objsInRightHandle[i];
            var inputDevice = currentObj.transform.GetComponent<ControllerInput>();
            if (inputDevice)
                return inputDevice;
        }
        return null;
    }
}
