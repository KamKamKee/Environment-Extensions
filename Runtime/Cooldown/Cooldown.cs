/*
 *  Copyright 2026 KamKamKee <kamkamkeedev@gmail.com>
 *
 *  This Source Code Form is subject to the terms of the Mozilla Public
 *  License, v. 2.0. If a copy of the MPL was not distributed with this
 *  file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *  
 */

using UnityEngine;
using EExtensions.Properties;

namespace EExtensions
{
    [System.Serializable]
    public class Cooldown
    {
        [SerializeField] private float durationSec = 0;
        [SerializeField] private AnimationCurve progressValueCurve = AnimationCurve.Linear(0, 0, 1, 1);

        public Property<bool> isReady { get; private set; }

        private float lastUseTime = 0;

        #region Constructor
        public Cooldown(float duration)
        {
            SetCooldown(duration);
        }

        public Cooldown(float duration, AnimationCurve valueCurve)
        {
            SetCooldown(duration);
            SetValueCurve(valueCurve);
        }

        public Cooldown(Cooldown cooldown)
        {
            SetCooldown(cooldown.durationSec);
            SetValueCurve(cooldown.valueCurve);
        }

        public void Initialize()
        {
            lastUseTime = -durationSec;
            isReady = Property<bool>.Static(true);
        }

        ~Cooldown()
        {
            isReady.Dispose();
        }
        #endregion

        // Setters
        public void SetCooldown(float duration)
        {
            durationSec = Mathf.Clamp(duration, 0, float.MaxValue);
        }

        public void SetValueCurve(AnimationCurve valueCurve)
        {
            if (valueCurve == null) return;

            progressValueCurve = valueCurve;
        }

        // Getter
        public float ProgressOnCurve()
        {
            // Value on curve
            return valueCurve.Evaluate(Progress());
        }

        public float Progress()
        {
            // Percentage
            return ProgessInTime() / durationSec;
        }

        public float ProgessInTime()
        {
            // Time value
            return Mathf.Clamp(Time.time - lastUseTime, 0, durationSec);
        }

        public float duration
        {
            get
            {
                return durationSec;
            }
        }

        public AnimationCurve valueCurve
        {
            get
            {
                return progressValueCurve;
            }
        }

        // Functions
        public void Trigger()
        {
            if (isReady == null)
            {
                Debug.LogError("Cooldown object was not Instantiated. Use Cooldown.Instantiate() in your Awake function first.");
                return;
            }

            lastUseTime = Time.time;
            isReady.Set(false);
            WaitForReady();
        }

        public void TriggerWithCooldown(float duration)
        {
            if (isReady == null)
            {
                Debug.LogError("Cooldown object was not Instantiated. Use Cooldown.Instantiate() in your Awake function first.");
                return;
            }

            SetCooldown(duration);
            lastUseTime = Time.time;
            isReady.Set(false);
        }

        public void ForceReady()
        {
            if (isReady == null)
            {
                Debug.LogError("Cooldown object was not Instantiated. Use Cooldown.Instantiate() in your Awake function first.");
                return;
            }

            lastUseTime = -durationSec;
            isReady.Set(true);
        }

        // System
        private async void WaitForReady()
        {
            // In case the cooldown duration changed
            while (Progress() < 1)
            {
                await Awaitable.WaitForSecondsAsync(durationSec - (Time.time - lastUseTime));
            }

            isReady.Set(true);
        }
    }
}