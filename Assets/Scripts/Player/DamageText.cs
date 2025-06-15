using System;
using UnityEngine;
using TMPro;

namespace Player
{
    public class DamageText : MonoBehaviour
    {
        private GameObject cameraGO;

        private void DestroyText()
        {
            Destroy(gameObject);
        }

        public void GetCalled(float damage, GameObject camera)
        {
            GetComponent<TMP_Text>().text = damage.ToString();
            cameraGO = camera;
        }

        private void LateUpdate()
        {
            if (cameraGO != null)
            {
                transform.LookAt(transform.position + cameraGO.transform.rotation * Vector3.forward,
                    cameraGO.transform.rotation * Vector3.up);
            }
        }
    }
}