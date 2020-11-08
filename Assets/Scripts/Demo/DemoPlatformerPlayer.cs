using UnityEngine;

namespace Scaffold.Demo
{
	// monolithic architecture. requires least understanding of c# classes. todo

	internal class DemoPlatformerPlayer : MonoBehaviour, IDemoPlayerDamageTarget
	{
		public enum State
		{
			Run, Jump, Dash
		}

		[Header("Common")]
		public State state;
		public BoxCollider2D groundRegion = default;
		public LayerMask groundMask = ~(1 << 10); // Omit player layer
		public bool grounded;
		public float gravity = 20;
		public float terminalVel = 15;

		[Header("Run")]
		public float runVel = 4;
		public float runAccel = 50;

		[Header("Jump")]
		public float jumpVel = 5;
		public ManualTimer jumpTimer = new ManualTimer(0.2f);

		[Header("Dash")]
		public ManualTimer dashTimer = new ManualTimer();
		// Animation curves are a simple and versatile way to change a value
		// over time. Using an animation curve allows quick iteration in the
		// editor and vast flexibility over possible inputs.
		[Tooltip("Normalized from [0, 1] over total duration of dash")]
		public AnimationCurve dashVel = AnimationCurve.Linear(0, 0, 1, 1);
		public float dashDecayAccel = 30;
		public AnimationCurve dashScreenshake = default;
		public AnimationCurve dashTimeSlow = default;

		[Header("Die")]
		public AnimationCurve dieScreenshake = default;

		private Vector2 velocity;
		private Vector2 dashDecayVel;
		private Vector2 dashDir;
		private DemoPlatformerInputState input;

		private Rigidbody2D body;
		private DemoPlatformerAnimator anim;
		private GameScripts.GameControls controls;

		// =========================================================
		// Setup
		// =========================================================

		private void Awake()
		{
			body = GetComponent<Rigidbody2D>();
			anim = GetComponent<DemoPlatformerAnimator>();
			controls = new GameScripts.GameControls();
			input = new DemoPlatformerInputState();
		}

		private void Start()
		{
			BeginStateRun();
		}

		private void OnEnable()
		{
			controls.Enable();
		}

		private void OnDisable()
		{
			controls.Disable();
		}

		// =========================================================
		// Update
		// =========================================================

		internal void FetchInput()
		{
			var inp = controls.Gameplay; // For readability

			input.axes = inp.Move.ReadValue<Vector2>();
			if (input.axes.x != 0)
			{
				input.lastHorz = input.axes.x;
			}

			if (inp.Jump.triggered)
			{
				input.jump.Store();
			}
			// Checks if the button is held. This unusual snippet is one of the
			// only ways this can be done when using the newer InputSystem
			// package.
			input.jumpHeld = inp.Jump.ReadValue<float>() > 0;

			if (inp.Attack.triggered)
			{
				input.dash.Store();
			}
		}

		private void Update()
		{
			FetchInput();
		}

		// =========================================================
		// Grounded Logic
		// =========================================================

		private void CheckIfTouchedGround(Collision2D collision)
		{
			// Sets the grounded status to true if the player lands on the top face
			// of a collider.

			if (grounded) return;

			const float MaxSlopeAngle = 30f;

			foreach (var cp in collision.contacts)
			{
				if (Vector2.Angle(cp.normal, Vector2.up) < MaxSlopeAngle)
				{
					grounded = true;
					return;
				}
			}
		}

		private void CheckIfLeftGround()
		{
			// Sets the grounded status to false if there is no objects in the
			// space underneath the player.
			//
			// Necessary because of noise in the physics simulation.

			if (!grounded) return;

			bool hitObject = Physics2D.OverlapBox(body.position + groundRegion.offset, groundRegion.size, 0, groundMask);

			if (!hitObject)
			{
				grounded = false;
			}
		}

		// =========================================================
		// Fixed Update
		// =========================================================

		private void ApplyGravity()
		{
			if (grounded)
			{
				velocity.y = 0;
				return;
			}

			velocity.y -= gravity * Time.fixedDeltaTime;
			if (velocity.y < -terminalVel)
			{
				velocity.y = -terminalVel;
			}
		}

		private void BeginStateRun()
		{
			state = State.Run;
		}

		private void StateRun()
		{
			velocity.x = Mathf.MoveTowards(velocity.x, runVel * input.axes.x, runAccel * Time.fixedDeltaTime);
			ApplyGravity();
			anim.PlayClip(DemoPlatformerAnimationId.Run, input.lastHorz > 0);

			if (grounded && input.jump.PeekThenClear())
			{
				BeginStateJump();
			}

			if (input.dash.PeekThenClear())
			{
				BeginStateDash();
			}
		}

		private void BeginStateJump()
		{
			state = State.Jump;
			jumpTimer.Start();
			velocity.y = jumpVel;
		}

		private void StateJump()
		{
			jumpTimer.Update(Time.fixedDeltaTime);

			if (jumpTimer.Running && input.jumpHeld)
			{
				velocity.y = jumpVel;
			}
			else
			{
				BeginStateRun();
			}

			if (input.dash.PeekThenClear())
			{
				BeginStateDash();
			}
		}

		private void BeginStateDash()
		{
			state = State.Dash;
			dashTimer.Start();
			dashDecayVel = velocity;
			dashDir = new Vector2(input.lastHorz, 0).normalized;
			DemoCameraController.instance.screenshake.AddTimedShake(dashScreenshake);
			TimeManager.AddTimedDilation(dashTimeSlow);
		}

		private void StateDash()
		{
			// This design pattern simplifies working with complicated velocity
			// changes.
			//
			// The pattern splits apart the original velocity into multiple
			// pieces. Each piece is stored and manipulated individually. At
			// the end, each component piece is recombined to form the output
			// velocity.
			//
			// A typical example may combine previous velocity, current axis
			// input, and velocity from a curve.

			// Feather in the previous velocity
			dashDecayVel = Vector2.MoveTowards(dashDecayVel, Vector2.zero, dashDecayAccel * Time.fixedDeltaTime);

			// Calculate raw dash velocity
			dashTimer.Update(Time.fixedDeltaTime);
			Vector2 curveOutput = dashVel.Evaluate(dashTimer.NormalizedProgress) * dashDir;

			// Blend together for smooth output
			velocity = dashDecayVel + curveOutput;

			if (dashTimer.Done)
			{
				BeginStateRun();
			}
		}

		private void FixedUpdate()
		{
			// Create a local copy that can be edited intuitively.
			velocity = body.velocity;

			CheckIfLeftGround();

			// Update appropriate state in state machine
			switch (state)
			{
				case State.Run: StateRun(); break;
				case State.Jump: StateJump(); break;
				case State.Dash: StateDash(); break;
			}

			// Apply the results of the state machine.
			body.velocity = velocity;
		}

		private void KillPlayer()
		{
			// Destroying the player is the easiest, safest way of stopping
			// gameplay logic and preventing unusual side-effects.

			anim.PlayDieAnimation();
			DemoCameraController.instance.screenshake.AddTimedShake(dieScreenshake);
			Destroy(gameObject);
		}

		// =========================================================
		// Physics Callbacks
		// =========================================================

		private void OnCollisionEnter2D(Collision2D collision)
		{
			CheckIfTouchedGround(collision);
		}

		// =========================================================
		// Interfaces
		// =========================================================

		// Explicit interface implementation syntax greatly reduces chance of
		// errors.

		void IDemoPlayerDamageTarget.Attack()
		{
			KillPlayer();
		}
	}
}