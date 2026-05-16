using Godot;
using Pokenom.Scripts.Core;

namespace Pokenom.Scripts.Utilites;

public abstract partial class State : Node
{
    [Export] public Node StateOwner;

    public virtual void EnterState()
    {
        CustomLogger.Debug($"Entering {GetType().Name} state ....");

    }

    public virtual void ExitState()
    {
        CustomLogger.Debug($"Exiting {GetType().Name} state ....");
        
    }
}