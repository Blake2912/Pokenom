using Godot;
using System;

namespace Pokenom.Scripts.Utilites;

public partial class StateMachine : Node
{
	[ExportCategory("State Maching Vars")]
	[Export]
	public Node Customer;
	[Export]
	public State CurrentState;

	public override void _Ready()
	{
		foreach (var child in GetChildren())
		{
			if (child is State state)
			{
				state.StateOwner = Customer;
				state.SetProcess(false);
			}
		}
	}

	public string GetCurrentState()
	{
		return CurrentState.Name.ToString();
	}

	public void ChangeState(State newState)
	{
		CurrentState?.ExitState();
		CurrentState = newState;
		CurrentState?.EnterState();

		foreach (var child in GetChildren())
		{
			if (child is State state)
			{
				state.SetProcess(child == CurrentState);
			}
		}
	}
}
