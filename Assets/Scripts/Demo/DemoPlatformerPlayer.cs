using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scaffold.Demo
{
	internal class DemoPlatformerPlayer : MonoBehaviour
	{
		public enum State
		{
			Run, Jump, Dash, Die
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
		public AnimationCurve dashVel = AnimationCurve.Linear(0, 0, 1, 1);
		public float dashDecayAccel = 30;

		private Vector2 velocity;
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
				input.jump.Store();
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
		}

		private void FixedUpdate()
		{
			// Create a local copy that can be edited intuitively.
			velocity = body.velocity;

			CheckIfLeftGround();

			switch (state)
			{
				case State.Run: StateRun(); break;
				case State.Jump: StateJump(); break;
				case State.Dash:
					break;
				case State.Die:
					break;
			}

			// Apply the results of the state machine.
			body.velocity = velocity;
		}

		// =========================================================
		// Callbacks
		// =========================================================

		private void OnCollisionEnter2D(Collision2D collision)
		{
			CheckIfTouchedGround(collision);
		}
	}
}