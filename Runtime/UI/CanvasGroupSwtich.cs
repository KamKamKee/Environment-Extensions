/*
 *  Copyright 2026 KamKamKee <kamkamkeedev@gmail.com>
 *
 *  This Source Code Form is subject to the terms of the Mozilla Public
 *  License, v. 2.0. If a copy of the MPL was not distributed with this
 *  file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *  
 */

using UnityEngine;

#if PRIME_TWEEN_INSTALLED
using PrimeTween;
#endif

namespace EExtensions
{
    namespace UI
    {
#if PRIME_TWEEN_INSTALLED
        public class CanvasGroupSwitch : MonoBehaviour
        {
            [SerializeField] private bool initiallyInactive = false;
            public CanvasGroup canvasGroup { get; private set; }
            public bool active { get { return canvasGroup.alpha == 1.0f; } }

            public float fadeSpeed = 0.1f;
            public Ease easeMode = Ease.Linear;

            public float alpha { get { return canvasGroup.alpha; } set { canvasGroup.alpha = value; } }

            private Tween tweenCanvasAlpha;

            private void Awake()
            {
                canvasGroup = GetComponent<CanvasGroup>();

                if (initiallyInactive)
                    alpha = 0.0f;
            }

            public void Activate(float newFadeSpeed = 0.0f)
            {
                tweenCanvasAlpha.Stop();

                if (canvasGroup.alpha == 1.0f) return;

                if (newFadeSpeed != 0.0f)
                    fadeSpeed = newFadeSpeed;

                tweenCanvasAlpha = Tween.Alpha(canvasGroup, 1.0f, fadeSpeed * (1 - alpha), easeMode);
            }

            public void Deactivate(float newFadeSpeed = 0.0f)
            {
                tweenCanvasAlpha.Stop();

                if (canvasGroup.alpha == 0.0f) return;

                if (newFadeSpeed != 0.0f)
                    fadeSpeed = newFadeSpeed;

                tweenCanvasAlpha = Tween.Alpha(canvasGroup, 0.0f, fadeSpeed * alpha, easeMode);
            }
        }
#endif
    }
}
