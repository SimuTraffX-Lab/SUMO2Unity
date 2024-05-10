namespace VRDriving.Movement
{
	/// <summary>
    /// A class that holds movement inputs.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class MovementInputs
    {
        public float accelerate;	// Accelerate input. 	[0->1]
		public float brake;			// Brakes input.		[0->1]

        public MovementInputs Copy()
        {
            return new MovementInputs
            {
                accelerate = accelerate,
				brake = brake
            };
        }

        /// <summary>Instaniates and returns an empty MovementInputs (no inputs) object.</summary>
        /// <returns>an empty MovementInputs (no inputs) object.</returns>
        public static MovementInputs EmptyInputs()
        {
            return new MovementInputs()
            {
                accelerate = 0,
                brake = 0
            };
        }
    }
}
