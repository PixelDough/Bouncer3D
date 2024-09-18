using UnityEngine;

namespace PixelDough.Bouncer
{
    public class LiveZone : MonoBehaviour
    {
        public BoxCollider boxCollider;

        public bool IsInZone(Vector3 pos)
        {
            Vector3 closestPoint = boxCollider.ClosestPoint(pos);
            return closestPoint == pos;
        }
    }
}
