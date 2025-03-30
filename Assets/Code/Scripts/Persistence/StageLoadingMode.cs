namespace Code.Scripts.Persistence
{
    public enum StageLoadingMode
    {
        None,
        
        /// <summary>
        /// When player started new game
        /// </summary>
        NewGame,
        
        /// <summary>
        /// When player loading save files
        /// </summary>
        SavesLoading,
        
        /// <summary>
        /// When player transfer between stages
        /// </summary>
        StageTransition
    }
}