using Sandbox;
using System;
using System.Linq;

namespace Automata;
partial class Pawn : AnimatedEntity
{
	private float OrbitDistance = 64.0f;
	private float TargetOrbitDistance = 64.0f;

	private const float ZoomSpeed = 3.0f;

	public override void Spawn()
	{
		if ( IsServer ) return;
		base.Spawn();

		SetModel( "models/sbox_props/watermelon/watermelon.vmdl" );

		EnableDrawing = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;
		EnableLagCompensation = false;
	}

	public override void Simulate( Client cl )
	{
		if ( IsServer ) return;

		base.Simulate( cl );

		Vector3 Offset = new Vector3( Automata.BoundsSize, Automata.BoundsSize, Automata.BoundsSize ) * 0.5f;
		var Origin = Vector3.Zero + Offset;

		Position = Origin;
		Rotation = Input.Rotation;

		TargetOrbitDistance = MathX.Clamp( TargetOrbitDistance + (-Input.MouseWheel * ZoomSpeed), 10.0f, 200.0f );
		OrbitDistance = MathX.Lerp(OrbitDistance, TargetOrbitDistance, Time.Delta / 0.1f);

		Position += Rotation.Backward * OrbitDistance;
	}
	public override void FrameSimulate( Client cl )
	{
		if ( IsServer ) return;
		base.FrameSimulate( cl );

		Rotation = Input.Rotation;
		EyeRotation = Rotation;
	}
}
