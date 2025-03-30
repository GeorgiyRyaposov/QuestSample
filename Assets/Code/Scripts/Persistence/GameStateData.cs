namespace Code.Scripts.Persistence
{
    public class GameStateData
    {
        public SessionStateData SessionState = new ();
        public InputState InputState = new();
        public InputSettings InputSettings = new ();
        
        public bool SaveInProgress;
        
        public StageLoadingMode StageLoadingMode;
    }
}