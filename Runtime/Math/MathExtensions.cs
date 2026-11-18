/*
 *  Copyright 2026 KamKamKee <kamkamkeedev@gmail.com>
 *
 *  This Source Code Form is subject to the terms of the Mozilla Public
 *  License, v. 2.0. If a copy of the MPL was not distributed with this
 *  file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *  
 */

using UnityEngine;

namespace EExtensions
{
    namespace MathExtensions
    {
        public static class MathExtensions
        {
            /// <summary>
            /// Returns true if the value is inside the specified range including min and max. False if the value is outside of min and max.
            /// </summary>
            /// <param name="floatNumber"></param>
            /// <param name="min"></param>
            /// <param name="max"></param>
            /// <returns></returns>
            public static bool IsInRange(this float floatNumber, float min, float max)
            {
                return floatNumber >= min && floatNumber <= max;
            }

            /// <summary>
            /// Returns true if the value is inside the specified range including min and max. False if the value is outside of min and max.
            /// </summary>
            /// <param name="floatNumber"></param>
            /// <param name="range"></param>
            /// <returns></returns>
            public static bool IsInRange(this float floatNumber, Vector2 range)
            {
                return floatNumber >= range.x && floatNumber <= range.y;
            }

            /// <summary>
            /// Returns true if the value is inside the range of a target point.
            /// E.g. the target being 8.0f and the radius being 0.5f, the range would be 7.5f to 8.5f including both min and max.
            /// </summary>
            /// <param name="floatNumber"></param>
            /// <param name="target"></param>
            /// <param name="radius"></param>
            /// <param name="offset"></param>
            /// <returns></returns>
            public static bool IsArround(this float floatNumber, float target, float radius, float offset=0)
            {
                return IsInRange(floatNumber, target - radius + offset, target + radius + offset);
            }

            /// <summary>
            /// Return true if the value is the target point or higher up to a certain range.
            /// E.g. the target being 8.0f and the radius being 0.5f, the range would be 8.0 to 8.5f including both min and max.
            /// </summary>
            /// <param name="floatNumber"></param>
            /// <param name="target"></param>
            /// <param name="radius"></param>
            /// <returns></returns>
            public static bool IsArroundHigher(this float floatNumber, float target, float radius)
            {
                return IsInRange(floatNumber, target, target + radius);
            }

            /// <summary>
            /// Return true if the value is the target point or lower down to a certain range.
            /// E.g. the target being 8.0f and the radius being 0.5f, the range would be 7.5 to 8.0f including both min and max.
            /// </summary>
            /// <param name="floatNumber"></param>
            /// <param name="target"></param>
            /// <param name="radius"></param>
            /// <returns></returns>
            public static bool IsArroundLower(this float floatNumber, float target, float radius)
            {
                return IsInRange(floatNumber, target - radius, target);
            }

            /// <summary>
            /// Returns true if the value is inside the specified range including min and max. False if the value is outside of min and max.
            /// </summary>
            /// <param name="integerNumber"></param>
            /// <param name="min"></param>
            /// <param name="max"></param>
            /// <returns></returns>
            public static bool IsInRange(this int integerNumber, float min, float max)
            {
                return ((float)integerNumber).IsInRange(min, max);
            }

            /// <summary>
            /// Returns true if the value is inside the range of a target point.
            /// E.g. the target being 8.0f and the radius being 0.5f, the range would be 7.5f to 8.5f including both min and max.
            /// </summary>
            /// <param name="integerNumber"></param>
            /// <param name="target"></param>
            /// <param name="radius"></param>
            /// <param name="offset"></param>
            /// <returns></returns>
            public static bool IsArround(this int integerNumber, float target, float radius, float offset = 0)
            {
                return ((float)integerNumber).IsArround(target, radius, offset);
            }

            /// <summary>
            /// Return true if the value is the target point or higher up to a certain range.
            /// E.g. the target being 8.0f and the radius being 0.5f, the range would be 8.0 to 8.5f including both min and max.
            /// </summary>
            /// <param name="integerNumber"></param>
            /// <param name="target"></param>
            /// <param name="radius"></param>
            /// <returns></returns>
            public static bool IsArroundHigher(this int integerNumber, float target, float radius)
            {
                return ((float)integerNumber).IsArroundHigher(target, radius);
            }

            /// <summary>
            /// Return true if the value is the target point or lower down to a certain range.
            /// E.g. the target being 8.0f and the radius being 0.5f, the range would be 7.5 to 8.0f including both min and max.
            /// </summary>
            /// <param name="integerNumber"></param>
            /// <param name="target"></param>
            /// <param name="radius"></param>
            /// <returns></returns>
            public static bool IsArroundLower(this int integerNumber, float target, float radius)
            {
                return ((float)integerNumber).IsArroundLower(target, radius);
            }

            /// <summary>
            /// Checks if the Layermask includes a specific layer.
            /// </summary>
            /// <param name="layerMask"></param>
            /// <param name="layer"></param>
            /// <returns></returns>
            public static bool IncludesLayer(this LayerMask layerMask, int layer)
            {
                return ((1 << layer) & layerMask) != 0;
            }

            /// <summary>
            /// Creates a range stored inside a Vector2. The lower value becomes x and the higher value becomes y.
            /// </summary>
            /// <param name="startValue"></param>
            /// <param name="endValue"></param>
            /// <returns></returns>
            public static Vector2 Range(this float startValue, float endValue)
            {
                if (startValue < endValue)
                {
                    return new Vector2(startValue, endValue);
                }

                else
                {
                    return new Vector2(endValue, startValue);
                }
            }

            /// <summary>
            /// Creates a range stored inside a Vector2. The range is based off the radius.
            /// E.g. the current value being 8.0f and the radius being 0.5f, the range would be x: 7.5 and y: 8.5f.
            /// </summary>
            /// <param name="targetValue"></param>
            /// <param name="radius"></param>
            /// <param name="offset"></param>
            /// <returns></returns>
            public static Vector2 RangeArround(this float targetValue, float radius, float offset=0)
            {
                return new Vector2(targetValue - radius + offset, targetValue + radius + offset);
            }

            /// <summary>
            /// Creates a range stored inside a Vector2. The lower value becomes x and the higher value becomes y.
            /// </summary>
            /// <param name="startValue"></param>
            /// <param name="endValue"></param>
            /// <returns></returns>
            public static Vector2 Range(this int startValue, float endValue)
            {
                return ((float)startValue).Range(endValue);
            }

            /// <summary>
            /// Creates a range stored inside a Vector2. The range is based off the radius.
            /// E.g. the current value being 8 and the radius being 0.5f, the range would be x: 7.5 and y: 8.5f.
            /// </summary>
            /// <param name="targetValue"></param>
            /// <param name="radius"></param>
            /// <param name="offset"></param>
            /// <returns></returns>
            public static Vector2 RangeArround(this int targetValue, float radius, float offset = 0)
            {
                return ((float)targetValue).RangeArround(radius + offset);
            }
        }
    }
}
