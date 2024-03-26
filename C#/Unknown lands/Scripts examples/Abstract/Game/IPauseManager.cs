namespace Assets.Project.Scripts {
    public interface IPauseManager {

        public void PauseDynamicObjects();

        public void Pause();

        public void Unpause();

        public bool IsPaused();
    }
}
