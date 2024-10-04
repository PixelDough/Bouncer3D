using UnityEngine;

namespace PixelDough.Bouncer
{
    public class BallReturnAnimationEvents : MonoBehaviour
    {
        [SerializeField] private BallReturn ballReturn;

        public void SpitPlayer()
        {
            ballReturn.SpitPlayer();
        }
    }
}
