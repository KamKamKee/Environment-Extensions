/*
 *  Copyright 2026 KamKamKee <kamkamkeedev@gmail.com>
 *
 *  This Source Code Form is subject to the terms of the Mozilla Public
 *  License, v. 2.0. If a copy of the MPL was not distributed with this
 *  file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *  
 */

using System;
using UnityEngine;

namespace EExtensions
{
    namespace Properties
    {
        public interface IGenericPropertySubscription
        {
            public abstract void Unsubscribe();

            public abstract void Resubscribe();

            public abstract bool IsSubscribed();

            public abstract bool IsSubscribedTo<T>(Property<T> property);

            public abstract bool TargetPropertyIsIntact();
        }

        /// <summary>
        /// Stores subscription data for a callback method and it's corresponding property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public struct PropertySubscription<T> : IGenericPropertySubscription
        {
            /// <summary>
            /// The target property that the subscription is about.
            /// </summary>
            public Property<T> propertyUsed { get; private set; }
            /// <summary>
            /// The callback method taking old value and new value.
            /// </summary>
            public Action<T, T> callbackMethod { get; private set; }
            public bool isSubscribed { get; private set; }

            public PropertySubscription(Property<T> propertyUsed, ref Action<T, T> callbackMethod)
            {
                this.propertyUsed = propertyUsed;
                this.callbackMethod = callbackMethod;
                this.isSubscribed = true;
            }

            /// <summary>
            /// Unsubscribes from the property. The subscription data is kept.
            /// </summary>
            public void Unsubscribe()
            {
                if (!isSubscribed)
                    return;

                propertyUsed.Unsubscribe(this);

                isSubscribed = false;
            }

            /// <summary>
            /// When unsubscribed before, this will reactivate the subscription.
            /// </summary>
            public void Resubscribe()
            {
                if (isSubscribed)
                    return;

                propertyUsed.Resubscribe(this);
                isSubscribed = true;
            }

            /// <summary>
            /// Returns true if the subscription is active.
            /// </summary>
            /// <returns></returns>
            public bool IsSubscribed()
            {
                return isSubscribed;
            }

            /// <summary>
            /// Returns true if the given property is the same as the target property of the subscription.
            /// </summary>
            /// <param name="property"></param>
            /// <returns></returns>
            public bool IsSubscribedTo<CompareType>(Property<CompareType> property)
            {
                if (property is Property<T>)
                {
                    return property.Equals(this.propertyUsed);
                }

                return false;
            }

            public bool TargetPropertyIsIntact()
            {
                return propertyUsed.isAlive;
            }
        }

        /// <summary>
        /// Wraps an observable pattern arround a value and calls events whenever something was changed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class Property<T>
        {
            private event Action<T, T> _valueChangedEventCallback;
            private T propertyValue;
            private MonoBehaviour parent;
            private bool isStatic;

            public bool isAlive
            {
                get
                {
                    if (isStatic) return true;
                    return parent != null;
                }
            }

            public Property(T initialValue, MonoBehaviour parent)
            {
                ConnectParentObject(parent);
                propertyValue = initialValue;
            }

            private Property(T initialValue)
            {
                propertyValue = initialValue;
                parent = null;
                isStatic = true;
            }

            /// <summary>
            /// Factory method for creating a property in a static field.
            /// These don't connect to a parent as they won't be destroyed at all.
            /// In case you have to abandon the property instance, use Dispose() first.
            /// Returns a new property object without a parent.
            /// </summary>
            /// <typeparam name="Type"></typeparam>
            /// <param name="initialValue"></param>
            /// <returns></returns>
            public static Property<Type> Static<Type>(Type initialValue)
            {
                return new(initialValue);
            }

            /// <summary>
            /// Replaces the old value with the new one and calls the changed event.
            /// </summary>
            /// <param name="newValue"></param>
            public void SetLazy(T newValue)
            {
                T oldValue = propertyValue;
                propertyValue = newValue;
                Notify(oldValue);
            }

            /// <summary>
            /// Replaces the old value with the new one without calling the changed event.
            /// </summary>
            /// <param name="newValue"></param>
            public void SetQuiet(T newValue)
            {
                propertyValue = newValue;
            }

            /// <summary>
            /// Replaces the old value with the new one and calls the changed event if the new value is not the same as the old one.
            /// </summary>
            /// <param name="newValue"></param>
            public void Set(T newValue)
            {
                if (!propertyValue.Equals(newValue))
                {
                    T oldValue = propertyValue;
                    propertyValue = newValue;
                    Notify(oldValue);
                }
            }

            /// <summary>
            /// More dynamic version of the set method. Takes in the current value and should return the updated value. After modifying the value it calls the changed event.
            /// </summary>
            /// <param name="updateFunction"></param>
            public void UpdateLazy(Func<T, T> updateFunction)
            {
                T oldValue = propertyValue;
                propertyValue = updateFunction.Invoke(propertyValue);
                Notify(oldValue);
            }

            /// <summary>
            /// More dynamic version of the set method. 
            /// Takes in the current value and should return the updated value. 
            /// After modifying the value it calls the changed event, but only if the updated value is not the same as the old one.
            /// </summary>
            /// <param name="updateFunction"></param>
            public void Update(Func<T, T> updateFunction)
            {
                T newValue = updateFunction.Invoke(propertyValue);

                if (!propertyValue.Equals(newValue))
                {
                    T oldValue = propertyValue;
                    propertyValue = newValue;
                    Notify(oldValue);
                }

            }

            /// <summary>
            /// When you're storing a list or any object, this should be used to modify it's value. 
            /// Takes in the current value (in this case an object), then you can modify it (e.g. add an element) and the changed event gets called.
            /// </summary>
            /// <param name="updateFunctionNoReturn"></param>
            public void Update(Action<T> updateFunctionNoReturn)
            {
                T oldValue = propertyValue;
                updateFunctionNoReturn.Invoke(propertyValue);
                Notify(oldValue);
            }

            /// <summary>
            /// Returns the current stored value.
            /// </summary>
            /// <returns></returns>
            public T Get()
            {
                return propertyValue;
            }

            /// <summary>
            /// Sends a manual change event. Although this doesn't provide a last value, only the new one.
            /// </summary>
            public void Notify()
            {
                _valueChangedEventCallback?.Invoke(propertyValue, propertyValue);
            }

            /// <summary>
            /// Sends a manual change event.
            /// </summary>
            public void Notify(T previousValue)
            {
                _valueChangedEventCallback?.Invoke(previousValue, propertyValue);
            }

            /// <summary>
            /// Subscribes to the property without a value callback.
            /// </summary>
            /// <param name="noValueCallback"></param>
            public PropertySubscription<T> Subscribe(Action noValueCallback)
            {
                Action<T, T> wrappedValueCallback = (_, _) => noValueCallback.Invoke();

                _valueChangedEventCallback += wrappedValueCallback;

                return new PropertySubscription<T>(this, ref wrappedValueCallback);
            }

            /// <summary>
            /// Subscribes to the property without a value callback.
            /// </summary>
            /// <param name="noValueCallback"></param>
            public PropertySubscription<T> Subscribe(PropertyBehaviour target, Action noValueCallback)
            {
                return target.SubscribeClean(this, noValueCallback);
            }

            /// <summary>
            /// Subscribes to the property with a value callback of the new value.
            /// </summary>
            /// <param name="onlyNewValueCallback"></param>
            public PropertySubscription<T> Subscribe(Action<T> onlyNewValueCallback)
            {
                Action<T, T> wrappedValueCallback = (_, newValue) => onlyNewValueCallback.Invoke(newValue);

                _valueChangedEventCallback += wrappedValueCallback;

                return new PropertySubscription<T>(this, ref wrappedValueCallback);
            }

            /// <summary>
            /// Subscribes to the property with a value callback of the new value.
            /// </summary>
            /// <param name="onlyNewValueCallback"></param>
            public PropertySubscription<T> Subscribe(PropertyBehaviour target, Action<T> onlyNewValueCallback)
            {
                return target.SubscribeClean(this, onlyNewValueCallback);
            }

            /// <summary>
            /// Subscribes to the property with a value callback of first the old value, second the new value.
            /// </summary>
            /// <param name="fullValueCallback"></param>
            public PropertySubscription<T> Subscribe(Action<T, T> fullValueCallback)
            {
                _valueChangedEventCallback += fullValueCallback;

                return new PropertySubscription<T>(this, ref fullValueCallback);
            }

            /// <summary>
            /// Subscribes to the property with a value callback of first the old value, second the new value.
            /// </summary>
            /// <param name="fullValueCallback"></param>
            public PropertySubscription<T> Subscribe(PropertyBehaviour target, Action<T, T> fullValueCallback)
            {
                return target.SubscribeClean(this, fullValueCallback);
            }

            /// <summary>
            /// /// USE subscription.Resubscribe() INSTEAD. THIS METHOD IS NOT MENT TO BE CALLED ON IT'S OWN.
            /// Resubscribes to the property with an existing subscription already given.
            /// </summary>
            /// <param name="subscription"></param>
            public void Resubscribe(PropertySubscription<T> subscription)
            {
                _valueChangedEventCallback += subscription.callbackMethod;
            }

            /// <summary>
            /// Unsubscribes from the property when a subscription with value callback was used.
            /// </summary>
            /// <param name="unsubscribeToCallback"></param>
            private void Unsubscribe(Action<T, T> unsubscribeToCallback)
            {
                _valueChangedEventCallback -= unsubscribeToCallback;
            }

            /// <summary>
            /// USE subscription.Unsubscribe() INSTEAD. THIS METHOD IS NOT MENT TO BE CALLED ON IT'S OWN.
            /// Unsubscribes from the property when a subscription with value callback was used.
            /// </summary>
            /// <param name="unsubscribeTo"></param>
            public void Unsubscribe(PropertySubscription<T> subscription)
            {
                Unsubscribe(subscription.callbackMethod);
            }

            private void ConnectParentObject(MonoBehaviour parent)
            {
                if (parent != null)
                {
                    this.parent = parent;
                    isStatic = false;
                }

                else
                {
                    Debug.LogError("Non-static property was not assigned a valid parent. The parent MonoBehaviour is null.");
                }
            }

            /// <summary>
            /// Marks the property as not alive for all subscriptions. In a MonoBehaviour, propertys manage themselves, but not for plain C# classes.
            /// In any case this is the manual option to dispose of a property.
            /// </summary>
            public void Dispose()
            {
                isStatic = false;
                parent = null;
            }

            public override string ToString()
            {
                return propertyValue.ToString();
            }

            public static implicit operator T(Property<T> property)
            {
                return property.Get();
            }
        }

        /// <summary>
        /// Works as an observer and keeps track of it's dependencies (which should be of type Property). Whenever the value of the dependency changed, the stored value gets recomputed aswell.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class ComputedProperty<T>
        {
            private T computedValue;
            private bool computedAlready = false;
            private Func<T> computeFunction;

            /// <summary>
            /// The compute function is the algorithm that should be used for calculating the value stored in the class. Every property used inside the function must be listed in the dependencies.
            /// </summary>
            /// <param name="computeFunction"></param>
            /// <param name="dependencies"></param>
            public ComputedProperty(Func<T> computeFunction, params object[] dependencies)
            {
                this.computeFunction = computeFunction;

                foreach (var dependency in dependencies)
                {
                    var evt = dependency.GetType().GetEvent("_valueChangedEvent");

                    if (evt != null)
                    {
                        evt.AddEventHandler(dependency, (Action)(() => computedAlready = false));
                    }
                }
            }

            /// <summary>
            /// Returns the current stored value and recomputes it if necessary.
            /// </summary>
            /// <returns></returns>
            public T Get()
            {
                if (!computedAlready)
                    Compute();

                return computedValue;
            }

            /// <summary>
            /// Manually calls the stored compute function and recomputes the stored value.
            /// </summary>
            public void Compute()
            {
                computedAlready = true;
                computedValue = computeFunction.Invoke();
            }

            public override string ToString()
            {
                return Get().ToString();
            }

            public static implicit operator T(ComputedProperty<T> property)
            {
                return property.Get();
            }
        }
    }
}
