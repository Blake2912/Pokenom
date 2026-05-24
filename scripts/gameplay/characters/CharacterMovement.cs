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

	private bool _wasWalking;




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

	public void CancelMovement()
	{
		IsWalking = false;
		_wasWalking = false;
	}

	public void SetFacingDirection(Vector2 direction)
	{
		if (direction == Vector2.Zero)
		{
			return;
		}

		CharacterInput.Direction = ToCardinalDirection(direction);
		EmitSignal(SignalName.Animation, "idle");
	}

	public void StartWalkingInDirection(Vector2 direction)
	{
		if (direction == Vector2.Zero)
		{
			return;
		}

		CharacterInput.Direction = ToCardinalDirection(direction);
		StartWalking();
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

	private static Vector2 ToCardinalDirection(Vector2 direction)
	{
		direction = direction.Normalized();
		if (Mathf.Abs(direction.X) > Mathf.Abs(direction.Y))
		{
			return new Vector2(Mathf.Sign(direction.X), 0);
		}

		return new Vector2(0, Mathf.Sign(direction.Y));
	}

	public void Walk(double delta)
	{
		if (IsWalking)
		{
			_wasWalking = true;
			Character.Position = Character.Position.MoveToward(TargetPosition, (float)delta * Globals.Instance.GRID_SIZE * 4);

			if (Character.Position.DistanceTo(TargetPosition) < 1f)
			{
				StopWalking();
			}
		}
		else if (_wasWalking)
		{
			_wasWalking = false;
			EmitSignal(SignalName.Animation, "idle");
		}
	}


	public void StopWalking()
	{
		IsWalking = false;
		Character.Position = TargetPosition;
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
