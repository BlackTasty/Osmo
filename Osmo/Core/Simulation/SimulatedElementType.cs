namespace Osmo.Core.Simulation
{
    public enum SimulatedElementType
    {
        /// <summary>
        /// Tells the <see cref="Controls.SimulatedElement"/> that it only contains an animated element 
        /// (for example menu-back)
        /// </summary>
        AnimatedOnly,
        /// <summary>
        /// Tells the <see cref="Controls.SimulatedElement"/> that it contains both an animated element and a fixed element
        /// (for example hitcircle with approachcircle)
        /// </summary>
        AnimatedAndFixed,
        /// <summary>
        /// Tells the <see cref="Controls.SimulatedElement"/> that it only contains a fixed element
        /// </summary>
        FixedOnly
    }
}
