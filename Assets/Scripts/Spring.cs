using UnityEngine;

namespace SweetLibs
{
    public static class Spring
    {
        /// <summary>
        /// Springs a float value
        /// </summary>
        /// <param name="x">current value</param>
        /// <param name="v">velocity</param>
        /// <param name="xt">target value</param>
        /// <param name="zeta">damping ratio</param>
        /// <param name="omega">angular frequency</param>
        /// <param name="dt">time step</param>
        /// <returns>Float with applied spring simulation</returns>
        public static float FloatSpring(float x, ref float v, float xt, float zeta, float omega, float dt)
        {
            float f = 1.0f + 2.0f * dt * zeta * omega;
            float oo = omega * omega;
            float hoo = dt * oo;
            float hhoo = dt * hoo;
            float detInv = 1.0f / (f + hhoo);
            float detX = f * x + dt * v + hhoo * xt;
            float detV = v + hoo * (xt - x);
            v = detV * detInv;
            return detX * detInv;
        }
        
        /// <summary>
        /// Springs a Vector3 value
        /// </summary>
        /// <param name="x">current value</param>
        /// <param name="v">velocity</param>
        /// <param name="xt">target value</param>
        /// <param name="zeta">damping ratio</param>
        /// <param name="omega">angular frequency</param>
        /// <param name="dt">time step</param>
        /// <returns>Vector with applied spring simulation</returns>
        public static Vector3 Vector3Spring(Vector3 from, ref Vector3 velocity, Vector3 to, float zeta, float omega,
            float dt)
        {
            var x = FloatSpring(from.x, ref velocity.x, to.x, zeta, omega, dt);
            var y = FloatSpring(from.y, ref velocity.y, to.y, zeta, omega, dt);
            var z = FloatSpring(from.z, ref velocity.z, to.z, zeta, omega, dt);
            return new Vector3(x, y, z);
        }
    }
}