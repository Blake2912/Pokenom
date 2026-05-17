using System.Reflection.Metadata.Ecma335;
using Godot;
using Pokenom.Scripts.Core;


namespace Pokenom.Scripts.Gameplay.Characters;

public partial class CharacterMovement : Node
{
	[Signal]
	public delegate void AnimationEventHandler(string animationType);

	[ExportCategory("Nodes")]
	[Export]
	public Node2D Character;
	[Export]
	public CharacterInput CharacterInput;

	[ExportCategory("Movement")]
	[Export]
	public Vector2 TargetPosition = Vector2.Down;
	[Export]
	public bool IsWalking = false;
	[Export]
	public bool IsCollisionDetected = false;




	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		CharacterInput.Walk += StartWalking;
		CharacterInput.Turn += Turn;

		CustomLogger.Debug("Loading player movement component...");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Walk(delta);
	}

	public bool IsMoving()
	{
		return IsWalking;
	}

	public bool IsColliding()
	{
		return IsCollisionDetected;
	}

	public bool IsTargetOccupied(Vector2 targetWorldPosition)
	{
		var spaceState = GetViewport().GetWorld2D().DirectSpaceState;

		Vector2 adjustTargetPosition = targetWorldPosition;
		adjustTargetPosition.X += 8;
		adjustTargetPosition.Y += 8;

		var query = new PhysicsPointQueryParameters2D
		{
			Position = adjustTargetPosition,
			CollisionMask = 1,
			CollideWithAreas = true
		};

		var result = spaceState.IntersectPoint(query);

		if (result.Count > 0)
		{
			foreach (var collision in result)
			{
				var collider = (Node)(GodotObject)collision["collider"];
				var colliderType = collider.GetType().Name;

				switch (colliderType)
				{
					case "TimeMapLayer":
						return true;
					case "SceneTriggers":
						return false;
					default: 
						return true;
				}

			}
		}

		return false;
	}

	public void StartWalking()
	{
		TargetPosition = Character.Position + CharacterInput.Direction * Globals.Instance.GRID_SIZE;

		if (!IsMoving() && !IsTargetOccupied(TargetPosition))
		{
			EmitSignal(SignalName.Animation, "walk");
			CustomLogger.Info($"Moving from {Character.Position} to {TargetPosition} ");
			IsWalking = true;
		}
	}

	public void Walk(double delta)
	{
		if (IsWalking)
		{
			Character.Position = Character.Position.MoveToward(TargetPosition, (float)delta * Globals.Instance.GRID_SIZE * 4);

			if (Character.Position.DistanceTo(TargetPosition) < 1f)
			{
				StopWalking();
			}
		}
		else
		{
			EmitSignal(SignalName.Animation, "idle");
		}	
	}


	public void StopWalking()
	{
		IsWalking = false;
		SnapPositionToGrid();
	}

	public void Turn()
	{
		EmitSignal(SignalName.Animation, "turn");
	}

	public void SnapPositionToGrid()
	{
		Character.Position = new Vector2(
			Mathf.Round(Character.Position.X / Globals.Instance.GRID_SIZE) * Globals.Instance.GRID_SIZE,
			Mathf.Round(Character.Position.Y / Globals.Instance.GRID_SIZE) * Globals.Instance.GRID_SIZE
		);
	}

}
