namespace Osmo.Core.Simulation
{
    public enum AnimationType
    {
        /// <summary>
        /// Used if an animation should follow a specific pattern of frames (like menu-back)
        /// </summary>
        Predefined,
        /// <summary>
        /// Used if an animation should be animated using <see cref="RenderTransform"/> (for example approachcircle)
        /// </summary>
        RenderTransform,
        /// <summary>
        /// Used if no animation shall be used
        /// </summary>
        NoAnimation
    }
}
