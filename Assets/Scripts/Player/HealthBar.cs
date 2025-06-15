using Mirror;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Player
{
    public class HealthBar : NetworkBehaviour
    {
        [SerializeField] private TextMeshProUGUI healthText = null;
        [SerializeField] private Slider healthSlider = null;
        [SerializeField] private Animator tpAnimator = null;

        [SerializeField] private GameObject mainFpsCamera, afterDeathCamera, tpModelMesh;

        [SerializeField] private Movement _movement;
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private Fire fire;
        private Vector3 startPosition;
        [SerializeField] private GameObject roundOverCanvas;
        [SerializeField] private TextMeshProUGUI winLoseText;


        [SyncVar(hook = nameof(HealthChanged))]
        private float health = 100f;

        void Start()
        {
            startPosition = transform.position;
            if (!isLocalPlayer)
                return;

            healthText.text = health.ToString();
            healthSlider.value = health;
        }

        public void NewRoundCall()
        {
            CmdMaxHealth();
        }

        [Server]
        public void TakeDamage(float damage)
        {
            health -= damage;
            if (health <= 0)
            {
                health = 0;
            }
        }

        private void HealthChanged(float oldHealth, float newHealth)
        {
            if (!isLocalPlayer)
                return;
            healthText.text = health.ToString();
            healthSlider.value = health;
            if (newHealth <= 0)
            {
                _characterController.enabled = false;
                _movement.enabled = false;
                fire.enabled = false;
                print("die");
                winLoseText.text = "You Lost !";
                roundOverCanvas.SetActive(true);
                afterDeathCamera.SetActive(true);
                mainFpsCamera.SetActive(false);
                tpModelMesh.SetActive(true);
                tpAnimator.SetBool("isDead", true);
                tpAnimator.SetBool("isWalking", false);

                Invoke(nameof(BeginNewRound), 5f);
            }
        }

        [Command]
        private void CmdMaxHealth()
        {
            ServerMaxHealth();
        }

        [Server]
        private void ServerMaxHealth()
        {
            health = 100;
        }

        public void BeginNewRound()
        {
            roundOverCanvas.SetActive(false);
            NewRoundCall();
            _characterController.enabled = false;
            _movement.enabled = false;
            fire.enabled = false;

            transform.position = startPosition;

            afterDeathCamera.SetActive(false);
            mainFpsCamera.SetActive(true);
            tpModelMesh.SetActive(false);
            tpAnimator.SetBool("isDead", false);
            tpAnimator.SetBool("isWalking", false);

            _movement.enabled = true;
            _characterController.enabled = true;
            fire.enabled = true; 
        }

        public float GetHealth()
        {
            return health;
        }
    }
}