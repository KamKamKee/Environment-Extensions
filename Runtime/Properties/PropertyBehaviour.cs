/*
 *  Copyright 2026 KamKamKee <kamkamkeedev@gmail.com>
 *
 *  This Source Code Form is subject to the terms of the Mozilla Public
 *  License, v. 2.0. If a copy of the MPL was not distributed with this
 *  file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *  
 */

using System;
using System.Collections.Generic;
using EExtensions.ListExpressions;
using UnityEngine;

namespace EExtensions
{
    namespace Properties
    {
        public abstract class PropertyBehaviour : MonoBehaviour
        {
            private List<IGenericPropertySubscription> subscriptions = new();

            /// <summary>
            /// Unsubscribes from all current subscriptions. Clears the list. New subscriptions have to be created again.
            /// </summary>
            protected void UnsubscribeAll()
            {
                subscriptions.ForEach(subscription => {
                    subscription.Unsubscribe();
                });

                subscriptions.Clear();
            }

            /// <summary>
            /// Ubsubscribes from all current subscriptions but keeps the list.
            /// </summary>
            protected void PauseAllSubscriptions()
            {
                subscriptions.ForEach(subscription => {
                    subscription.Unsubscribe();
                });
            }

            /// <summary>
            /// Resubscribes to all current inactive subscriptions.
            /// </summary>
            protected void ResumeAllSubscriptions()
            {
                CleanupSubscriptions();

                subscriptions.ForEach(subscription => {
                    subscription.Resubscribe();
                });
            }

            /// <summary>
            /// Checks if the subscriped properties are still alive and filters accordingly.
            /// </summary>
            protected void CleanupSubscriptions()
            {
                subscriptions.Filter(subscription => {
                    return subscription.TargetPropertyIsIntact();
                });
            }

            /// <summary>
            /// Returns true if this object currently has any active subscriptions.
            /// </summary>
            /// <returns></returns>
            public bool AnySubscriptionsActive()
            {
                foreach (IGenericPropertySubscription subscription in subscriptions)
                {
                    if (subscription.IsSubscribed())
                        return true;
                }

                return false;
            }

            /// <summary>
            /// Subscribes to a property without a value callback. The subscription is tracked and controlled by the object.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="property"></param>
            /// <param name="noValueCallback"></param>
            /// <returns></returns>
            public PropertySubscription<T> SubscribeClean<T>(Property<T> property, Action noValueCallback)
            {
                PropertySubscription<T> subscription = property.Subscribe(noValueCallback);
                subscriptions.Add(subscription);
                return subscription;
            }

            /// <summary>
            /// Subscribes to a property with a value callback of the new value. The subscription is tracked and controlled by the object.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="property"></param>
            /// <param name="singleValueCallback"></param>
            /// <returns></returns>
            public PropertySubscription<T> SubscribeClean<T>(Property<T> property, Action<T> singleValueCallback)
            {
                PropertySubscription<T> subscription = property.Subscribe(singleValueCallback);
                subscriptions.Add(subscription);
                return subscription;
            }

            /// <summary>
            /// Subscribes to a property with a value callback of first the old value, second the new value. The subscription is tracked and controlled by the object.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="property"></param>
            /// <param name="fullValueCallback"></param>
            /// <returns></returns>
            public PropertySubscription<T> SubscribeClean<T>(Property<T> property, Action<T, T> fullValueCallback)
            {
                PropertySubscription<T> subscription = property.Subscribe(fullValueCallback);
                subscriptions.Add(subscription);
                return subscription;
            }

            /// <summary>
            /// Unsubscribes from a property and cancels tracking. The subscription is no longer controlled by the Object.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="property"></param>
            protected void UnsubscribeFrom<T>(Property<T> property)
            {
                IGenericPropertySubscription subscription = FindGenericSubscriptionFrom(property);

                if (subscription != null)
                {
                    subscription.Unsubscribe();
                    subscriptions.Remove(subscription);
                }
            }

            /// <summary>
            /// Returns the generic subscription with the corresponding property from the current subscriptions.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="property"></param>
            /// <returns></returns>
            public IGenericPropertySubscription FindGenericSubscriptionFrom<T>(Property<T> property)
            {
                foreach (IGenericPropertySubscription subscription in subscriptions)
                {
                    if (subscription.IsSubscribedTo(property))
                    {
                        return subscription;
                    }
                }

                return null;
            }

            protected virtual void OnEnable()
            {
                ResumeAllSubscriptions();
            }

            protected virtual void OnDisable()
            {
                PauseAllSubscriptions();
            }

            protected virtual void OnDestroy()
            {
                UnsubscribeAll();
            }
        }
    }
}
