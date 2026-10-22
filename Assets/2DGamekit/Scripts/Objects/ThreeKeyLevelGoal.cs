using UnityEngine;
using UnityEngine.Events;

namespace Gamekit2D
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class ThreeKeyLevelGoal : MonoBehaviour
    {
        [Header("Door")]
        public Animator doorAnimator;
        public string doorOpeningState = "DoorOpening";
        public string[] requiredKeys = { "Key1", "Key2", "Key3" };
        public bool openOnlyWhenPlayerNearby = true;
        [Min(0.1f)] public float doorOpenDistance = 7f;

        [Header("Completion")]
        public LayerMask playerLayers;
        public string completionMessage = "FASE CONCLUIDA!";
        public bool pauseOnComplete = true;
        public LevelCompleteUI completionUI;
        public UnityEvent onDoorOpened;
        public UnityEvent onLevelCompleted;

        InventoryController m_Inventory;
        bool m_DoorOpened;
        bool m_Completed;

        public bool DoorOpened => m_DoorOpened;
        public bool Completed => m_Completed;

        void Awake()
        {
            BoxCollider2D trigger = GetComponent<BoxCollider2D>();
            trigger.isTrigger = true;

            if (completionUI == null)
                completionUI = GetComponent<LevelCompleteUI>();
        }

        void Update()
        {
            if (m_DoorOpened)
                return;

            if (m_Inventory == null && PlayerCharacter.PlayerInstance != null)
                m_Inventory = PlayerCharacter.PlayerInstance.inventoryController;

            if (m_Inventory != null && HasAllRequiredKeys() && IsPlayerNearDoor())
                OpenDoor();
        }

        bool IsPlayerNearDoor()
        {
            if (!openOnlyWhenPlayerNearby)
                return true;

            if (PlayerCharacter.PlayerInstance == null)
                return false;

            Vector2 doorPosition = transform.position;
            if (doorAnimator != null)
            {
                Collider2D doorCollider = doorAnimator.GetComponentInChildren<Collider2D>();
                doorPosition = doorCollider != null ? doorCollider.bounds.center : (Vector2)doorAnimator.transform.position;
            }

            return Vector2.Distance(PlayerCharacter.PlayerInstance.transform.position, doorPosition) <= doorOpenDistance;
        }

        bool HasAllRequiredKeys()
        {
            for (int i = 0; i < requiredKeys.Length; i++)
            {
                if (!m_Inventory.HasItem(requiredKeys[i]))
                    return false;
            }

            return true;
        }

        void OpenDoor()
        {
            m_DoorOpened = true;

            if (doorAnimator != null)
                doorAnimator.Play(doorOpeningState, 0, 0.0f);

            onDoorOpened.Invoke();
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (!m_DoorOpened || m_Completed || !playerLayers.Contains(other.gameObject))
                return;

            m_Completed = true;
            if (PlayerInput.Instance != null)
                PlayerInput.Instance.ReleaseControl();

            if (completionUI != null)
                completionUI.Show(completionMessage, pauseOnComplete);
            else if (pauseOnComplete)
                Time.timeScale = 0f;

            onLevelCompleted.Invoke();
        }

        void OnDisable()
        {
            if (m_Completed)
            {
                if (completionUI != null)
                    completionUI.Hide();
                else if (pauseOnComplete)
                    Time.timeScale = 1f;
            }
        }
    }
}
