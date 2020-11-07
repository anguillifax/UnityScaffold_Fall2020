using System;
using System.Collections.Generic;
using UnityEngine;

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

		private Rigidbody2D body;
		private DemoPlatformerAnimator anim;

		private void Awake()
		{
			body = GetComponent<Rigidbody2D>();
			anim = GetComponent<DemoPlatformerAnimator>();
		}

		private void Start()
		{
			BeginStateRun();
		}

		private void BeginStateRun()
		{
			state = State.Run;
		}

		private void StateRun()
		{
			velocity.x = Mathf.MoveTowards(velocity.x, runVel, runAccel * Time.fixedDeltaTime);
		}

		private void FixedUpdate()
		{
			switch (state)
			{
				case State.Run: StateRun();
					break;
				case State.Jump:
					break;
				case State.Dash:
					break;
				case State.Die:
					break;
			}
		}
	}
}