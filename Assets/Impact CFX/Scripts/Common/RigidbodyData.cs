namespace ImpactCFX
{
    /// <summary>
    /// Holds rigidbody data in a form suitable for jobs.
    /// </summary>
    public struct RigidbodyData
    {
        public static RigidbodyData Default => new RigidbodyData();

        /// <summary>
        /// The current velocity of the rigidbody after the collision.
        /// </summary>
        public RigidbodyStateData CurrentState;

        /// <summary>
        /// The previous velocity of the rigidbody before the collision.
        /// </summary>
        public RigidbodyStateData PreviousState;

        public RigidbodyData(RigidbodyStateData previousState, RigidbodyStateData currentState)
        {
            CurrentState = currentState;
            PreviousState = previousState;
        }
    }
}