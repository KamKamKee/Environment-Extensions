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
    namespace VectorUtility
    {
        public static class VectorUtility
        {
            /// <summary>
            /// Multiplies the vector on it's axises. Defaults x, y, z to 1.
            /// </summary>
            /// <param name="vector"></param>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="z"></param>
            /// <returns></returns>
            public static void Multiply(this ref Vector3 vector, float x = 1, float y = 1, float z = 1)
            {
                vector = vector.Multiplied(x, y, z);
            }

            /// <summary>
            /// Returns the vector multiplied on it's axises. Defaults x, y, z to 1.
            /// </summary>
            /// <param name="vector"></param>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="z"></param>
            /// <returns></returns>
            public static Vector3 Multiplied(this Vector3 vector, float x = 1, float y = 1, float z = 1)
            {
                return new Vector3(
                    vector.x * x,
                    vector.y * y,
                    vector.z * z
                );
            }

            /// <summary>
            /// Returns the flat vector with only the x and z axis. The y axis is 0.
            /// </summary>
            /// <param name="vector"></param>
            /// <returns></returns>
            public static Vector3 Flat(this Vector3 vector)
            {
                return Multiplied(vector, y: 0);
            }

            /// <summary>
            /// Returns the flat vector with only the x and z axis as Vector2. The y axis gets removed.
            /// </summary>
            /// <param name="vector"></param>
            /// <returns></returns>
            public static Vector2 FlatV2(this Vector3 vector)
            {
                Vector3 flatVector = Flat(vector);

                return new Vector2(flatVector.x, flatVector.z);
            }

            /// <summary>
            /// Returns the adjusted vector with the new magnitude.
            /// </summary>
            /// <param name="vector"></param>
            /// <param name="magnitude"></param>
            /// <returns></returns>
            public static Vector3 WithMagnitude(this Vector3 vector, float magnitude)
            {
                return vector.normalized * magnitude;
            }

            /// <summary>
            /// Sets the magnitude of a vector to a specified value thus controlling the length.
            /// </summary>
            /// <param name="vector"></param>
            /// <param name="magnitude"></param>
            public static void SetMagnitude(this ref Vector3 vector, float magnitude)
            {
                vector = vector.WithMagnitude(magnitude);
            }

            /// <summary>
            /// Returns the distance to another vector. Just a wrapper for Vector3.Distance().
            /// </summary>
            /// <param name="vector"></param>
            /// <param name="other"></param>
            /// <returns></returns>
            public static float DistanceTo(this Vector3 vector, Vector3 other)
            {
                return Vector3.Distance(vector, other);
            }

            /// <summary>
            /// Returns the distance to another vector. Just a wrapper for Vector3.Distance().
            /// </summary>
            /// <param name="vector"></param>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="z"></param>
            /// <returns></returns>
            public static float DistanceTo(this Vector3 vector, float x, float y, float z)
            {
                return Vector3.Distance(vector, new Vector3(x, y, z));
            }

            public static float RandomFloatFromRange(this Vector2 range)
            {
                return Random.Range(range.x, range.y);
            }

            public static int RandomIntFromRange(this Vector2 range)
            {
                return Mathf.RoundToInt(Random.Range(range.x, range.y));
            }
        }
    }
}
