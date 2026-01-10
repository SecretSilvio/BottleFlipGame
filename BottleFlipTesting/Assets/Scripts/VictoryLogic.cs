using UnityEngine;

public class VictoryLogic : MonoBehaviour
{
    public ParticleSystem victoryParticles;
    private void OnEnable()
    {
        GameEvents.OnFlipSuccess += HandleFlipSuccess;
    }
    private void OnDisable()
    {
        GameEvents.OnFlipSuccess -= HandleFlipSuccess;
    }

    private void HandleFlipSuccess()
    {
        Debug.Log("VictoryLogic: Flip Success Event Received!");
        // Add additional victory handling logic here
        victoryParticles.Play();

    }
}
