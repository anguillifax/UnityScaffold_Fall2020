using UnityEngine;

namespace Scaffold.Demo
{
	// Stores the current input in processed form.
	//
	// Using an intermediate class separates the process of reading input from
	// using input. Since gathering input is often a non-trivial operation,
	// separation simplifies code and reduces complexity.
	//
	// This technique is optional, but recommended.
	//
	// As an interesting side benefit, this means the player can be driven
	// autonomously via a script.

	internal class DemoPlatformerInputState
	{
		public Vector2 axes;
		public float lastHorz = 1f;

		public BufferedButtonPress jump = new BufferedButtonPress();
		public bool jumpHeld;

		public BufferedButtonPress dash = new BufferedButtonPress();
	}
}