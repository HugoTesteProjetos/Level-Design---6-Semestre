using System.Collections.Generic;
using UnityEngine;

namespace Gamekit2D
{
    public class LevelBackgroundExtender : MonoBehaviour
    {
        [Header("Sky")]
        public SpriteRenderer skyRenderer;
        public float skyWidth = 240f;

        [Header("Parallax Art")]
        public Transform parallaxRoot;
        public int additionalSections = 3;
        public float sectionWidth = 80f;

        void Awake()
        {
            ExtendSky();
            RepeatParallaxArt();
        }

        void ExtendSky()
        {
            if (skyRenderer == null || skyRenderer.sprite == null)
                return;

            // The supplied sky sprite is imported as a tight mesh, which Unity cannot tile
            // without a warning. Stretch only its X scale so the original height is preserved.
            skyRenderer.drawMode = SpriteDrawMode.Simple;
            float parentScaleX = skyRenderer.transform.parent == null
                ? 1f
                : Mathf.Abs(skyRenderer.transform.parent.lossyScale.x);
            float spriteWidth = skyRenderer.sprite.bounds.size.x;

            if (spriteWidth > Mathf.Epsilon && parentScaleX > Mathf.Epsilon)
            {
                Vector3 scale = skyRenderer.transform.localScale;
                scale.x = skyWidth / (spriteWidth * parentScaleX);
                skyRenderer.transform.localScale = scale;
            }
        }

        void RepeatParallaxArt()
        {
            if (parallaxRoot == null || additionalSections <= 0)
                return;

            List<Transform> originalChildren = new List<Transform>();
            for (int i = 0; i < parallaxRoot.childCount; i++)
                originalChildren.Add(parallaxRoot.GetChild(i));

            for (int section = 1; section <= additionalSections; section++)
            {
                float offset = sectionWidth * section;
                for (int i = 0; i < originalChildren.Count; i++)
                {
                    Transform original = originalChildren[i];
                    GameObject copy = Instantiate(original.gameObject, parallaxRoot);
                    copy.name = $"{original.name} (Background {section})";
                    copy.transform.localPosition = original.localPosition + Vector3.right * offset;
                }
            }
        }
    }
}
