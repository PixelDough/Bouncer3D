using UnityEngine;

namespace PixelDough.Bouncer
{
    public static class MathHelpers
    {
        /// <summary>
        /// Exponential decay function
        /// </summary>
        /// <param name="a">Start value</param>
        /// <param name="b">Destination value</param>
        /// <param name="decay">Useful range approx. 1 to 25, from slow to fast. 16 is a good default.</param>
        /// <param name="dt">Delta Time (in seconds)</param>
        /// <returns></returns>
        public static float ExpDecay(float a, float b, float decay, float dt)
        {
            return b+(a-b)*Mathf.Exp(-decay*dt);
        }
        
        public static Vector3 ExpDecay(Vector3 a, Vector3 b, float decay, float dt)
        {
            return b+(a-b)*Mathf.Exp(-decay*dt);
        }
        
        public static Quaternion ExpDecay(Quaternion a, Quaternion b, float decay, float dt)
        {
            return Quaternion.Slerp(a, b, 1 - Mathf.Exp(-decay * dt));
        }

        public static int Sign(float a)
        {
            if ((int)a == 0) return 0;
            return (int) Mathf.Sign(a);
        }
    }
}
