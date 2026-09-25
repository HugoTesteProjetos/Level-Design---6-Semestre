using System.Collections;
using UnityEngine;

namespace Gamekit2D
{
    public class HealthUI : MonoBehaviour
    {
        public Damageable representedDamageable;
        public GameObject healthIconPrefab;

        protected Animator[] m_HealthIconAnimators;
        protected bool m_InitializationStarted;

        protected readonly int m_HashActivePara = Animator.StringToHash ("Active");
        protected readonly int m_HashInactiveState = Animator.StringToHash ("Inactive");
        protected const float k_HeartIconAnchorWidth = 0.041f;

        IEnumerator Start ()
        {
            if (!m_InitializationStarted)
                yield return InitializeHearts();
        }

        IEnumerator InitializeHearts ()
        {
            if (representedDamageable == null || healthIconPrefab == null)
                yield break;

            m_InitializationStarted = true;

            yield return null;
            
            m_HealthIconAnimators = new Animator[representedDamageable.startingHealth];

            for (int i = 0; i < representedDamageable.startingHealth; i++)
            {
                GameObject healthIcon = Instantiate (healthIconPrefab);
                healthIcon.transform.SetParent (transform);
                RectTransform healthIconRect = healthIcon.transform as RectTransform;
                healthIconRect.anchoredPosition = Vector2.zero;
                healthIconRect.sizeDelta = Vector2.zero;
                healthIconRect.anchorMin += new Vector2(k_HeartIconAnchorWidth, 0f) * i;
                healthIconRect.anchorMax += new Vector2(k_HeartIconAnchorWidth, 0f) * i;
                m_HealthIconAnimators[i] = healthIcon.GetComponent<Animator> ();

                if (representedDamageable.CurrentHealth < i + 1)
                {
                    m_HealthIconAnimators[i].Play (m_HashInactiveState);
                    m_HealthIconAnimators[i].SetBool (m_HashActivePara, false);
                }
            }
        }

        // Kept for compatibility with scenes created with older versions of the kit.
        public void SetInitialHeartCount (Damageable damageable)
        {
            representedDamageable = damageable;

            if (!m_InitializationStarted && isActiveAndEnabled)
                StartCoroutine(InitializeHearts());
        }

        public void ChangeHitPointUI (Damageable damageable)
        {
            if(m_HealthIconAnimators == null)
                return;
            
            for (int i = 0; i < m_HealthIconAnimators.Length; i++)
            {
                m_HealthIconAnimators[i].SetBool(m_HashActivePara, damageable.CurrentHealth >= i + 1);
            }
        }
    }
}
