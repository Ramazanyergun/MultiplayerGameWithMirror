using System;
using Mirror;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

namespace Player
{
    public class Fire : NetworkBehaviour
    {
        [SerializeField] private new GameObject camera = null;
        [SerializeField] private LayerMask playerMask;
        [SerializeField] private float damage = 15f;
        private float lastShootTime = 0.0f;
        private float waitForSecondsBetweenShoots = 0.2f;
        [SerializeField] private GameObject damageTextParent;
        [SerializeField] private HealthBar _healthBar;
        [SerializeField] private GameObject roundOverCanvas;
        [SerializeField] private TextMeshProUGUI winLoseText;

        private void Update()
        {
            if (!isLocalPlayer)
                return;
            if (Input.GetKey(KeyCode.Mouse0))
            {
                if (lastShootTime == 0 || lastShootTime + waitForSecondsBetweenShoots < Time.time)
                {
                    lastShootTime = Time.time;
                    if (Physics.Raycast(camera.transform.position, camera.transform.forward, out RaycastHit hit,
                            playerMask))
                    {
                        if (hit.collider.TryGetComponent(out HealthBar playerHealthBar))
                        {
                            if (playerHealthBar.GetHealth() - 15 <= 0)
                            {
                                roundOverCanvas.SetActive(true);
                                winLoseText.text = "You Won !";
                                RoundOver();
                            }

                            if (playerHealthBar.GetHealth() <= 0f)
                                return;

                            GameObject newDmgTxtParent = Instantiate(damageTextParent, hit.point, Quaternion.identity);
                            newDmgTxtParent.GetComponentInChildren<DamageText>().GetCalled(damage, camera);
                            if (isServer)
                            {
                                ServerHit(damage, playerHealthBar);
                                return;
                            }

                            CmdHit(damage, playerHealthBar);
                        }
                    }
                }
            }
        }


        [Command]
        private void CmdHit(float damage, HealthBar playerHealth)
        {
            ServerHit(damage, playerHealth);
        }

        [Server]
        private void ServerHit(float damage, HealthBar playerHealth)
        {
            playerHealth.TakeDamage(damage);
        }

        private void RoundOver()
        {
            Invoke(nameof(BeginNewRound), 5f);
        }

        private void BeginNewRound()
        {
            _healthBar.BeginNewRound();
        }
    }
}