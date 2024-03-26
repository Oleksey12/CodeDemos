internal interface IMoveAnimations
{
    // Управляет анимациями при движении игрока
    public void StartEmission();
    public void StopEmission();
    public void SetAnimationToReverse();
    public void SetAnimationToNormal();
    public void StopAnimation();
    public void PlayAnimation();
    public void SetParticleStystemToNormal();
    public void SetParticleStystemToReverse();
}