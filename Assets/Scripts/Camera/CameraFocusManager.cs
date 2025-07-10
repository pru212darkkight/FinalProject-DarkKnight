using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class CameraFocusManager : MonoBehaviour
{
    public CinemachineCamera vCamPlayer;
    public CinemachineCamera vCamPortal;

    public void FocusPortalThenBack(Transform portalTransform, float focusDuration = 2f)
    {
        vCamPortal.Follow = portalTransform;
        vCamPortal.LookAt = portalTransform;
        vCamPortal.Priority = vCamPlayer.Priority + 10;

        StartCoroutine(FocusBackAfterDelay(focusDuration));
    }

    private IEnumerator FocusBackAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        vCamPortal.Priority = vCamPlayer.Priority - 1;
    }
}
