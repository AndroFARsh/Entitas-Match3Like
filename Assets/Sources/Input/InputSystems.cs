using Input.Components.Systems;

namespace Input
{
    public class InputSystems : Feature 
    {
        public InputSystems(Contexts contexts)
        {
            Add(new EmitMouseInputSystem(contexts));
            Add(new SelectItemSystem(contexts));
        }
    }
}