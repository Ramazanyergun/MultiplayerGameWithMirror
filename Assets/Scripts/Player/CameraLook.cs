using System;
using UnityEngine;

namespace Player
{
    public class CameraLook : MonoBehaviour
    {
        [SerializeField] private GameObject player = null;
        [SerializeField] private float ySensitivity = 200f, xSensitivity = 200f;
        private float xRotation;


        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            float xAngle = Input.GetAxis("Mouse Y") * ySensitivity * Time.deltaTime;
            float yAngle = Input.GetAxis("Mouse X") * xSensitivity * Time.deltaTime;

            xRotation -= xAngle;
            xRotation = Mathf.Clamp(xRotation, -80, 80);
            transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
            player.transform.Rotate(0, yAngle, 0);
        }
    }
}