// Interface for all steppable objects in the game.
// This allows for a common method to be called when the player steps on an object, regardless of its specific type or behavior.
public interface ISteppable
{
    void OnStep();
}
