using UnityEngine;

public static class HelixParticleEffects
{
    public static void Burst(Vector3 position, Color color, int particleCount, float minSpeed, float maxSpeed,
        float particleSize)
    {
        int safeCount = Mathf.Clamp(particleCount, 1, 100);
        GameObject effectObject = new GameObject("Helix Particle Burst");
        effectObject.transform.position = position;

        ParticleSystem particles = effectObject.AddComponent<ParticleSystem>();
        ParticleSystem.MainModule main = particles.main;
        main.playOnAwake = false;
        main.loop = false;
        main.duration = 0.35f;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.35f, 0.7f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(Mathf.Max(0f, minSpeed), Mathf.Max(minSpeed, maxSpeed));
        main.startSize = new ParticleSystem.MinMaxCurve(particleSize * 0.55f, particleSize);
        main.startColor = color;
        main.gravityModifier = 0.35f;
        main.maxParticles = safeCount;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.stopAction = ParticleSystemStopAction.Destroy;

        ParticleSystem.EmissionModule emission = particles.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[]
        {
            new ParticleSystem.Burst(0f, (short)safeCount, (short)safeCount)
        });

        ParticleSystem.ShapeModule shape = particles.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.14f;
        shape.radiusThickness = 0f;

        particles.Play();
    }
}
