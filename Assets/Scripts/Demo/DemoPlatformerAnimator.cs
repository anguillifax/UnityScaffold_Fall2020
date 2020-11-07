using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scaffold.Demo
{
	// This enum lists the name of each animation clip.
	//
	// Each value must correspond *exactly* with the name of the animation
	// clip, otherwise the animator will reject the clip name.
	//
	// This provides a typesafe public interface.

	public enum DemoPlatformerAnimationId
	{
		Idle, Run, Jump, Dash
	}

	internal class DemoPlatformerAnimator : MonoBehaviour
	{
		private SpriteRenderer sr;
		private Animator anim;

		private void Awake()
		{
			sr = GetComponent<SpriteRenderer>();
			anim = GetComponent<Animator>();
		}

		public void PlayClip(DemoPlatformerAnimationId clip, bool facingRight)
		{
			anim.Play(clip.ToString());
			sr.flipX = !facingRight;
		}
	}
}