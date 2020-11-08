namespace Scaffold.Demo
{
	// Interface for a component that deals damage to the player.
	//
	// Using an interface simplifies interactions between objects. Attacking
	// objects do not need to know how the player is implemented. They only
	// need to search for a component that supports this action.
	//
	// Separating the mechanism to deal damage to the player versus dealing
	// damage to other entities can be helpful in some games.

	internal interface IDemoPlayerDamageTarget
	{
		void Attack();
	}

	// An example of a generic interface that passes additional information. It
	// is not used in the demo.

#if false // `#if false` is like commenting out, but for large spans of code
	internal interface IDamageTarget
	{
		void Attack(float damage, Vector2 knockback);
	}
#endif
}