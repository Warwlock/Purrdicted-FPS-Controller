using UnityEngine;
using Unity.Cinemachine;

namespace Warwlock.Prediction.PlayerController
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineCamera cinemachineCamera;
        [SerializeField] private CinemachineInputAxisController cinemachineInputAxisController;

        public Vector3 forward => transform.forward;// Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0) * Vector3.forward;

        void Awake()
        {
            cinemachineCamera.Priority.Value = -1;
            cinemachineInputAxisController.enabled = false;
        }

        public void Init()
        {
            cinemachineCamera.Priority.Value = 10;
            cinemachineInputAxisController.enabled = true;
        }
    }
}
