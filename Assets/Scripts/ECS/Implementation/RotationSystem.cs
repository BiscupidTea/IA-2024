using System.Collections.Generic;
using System.Threading.Tasks;

public class RotationSystem : ECSSystem
{
    private ParallelOptions parallelOptions;

    private IDictionary<uint, RotationComponent> rotationComponents;
    private IDictionary<uint, RotationVelocityComponent> rotationVelocityComponents;
    private IEnumerable<uint> queryedEntities;

    public override void Initialize()
    {
        parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 32 };
    }

    protected override void PreExecute(float deltaTime)
    {
        rotationComponents ??= ECSManager.GetComponents<RotationComponent>();
        rotationVelocityComponents ??= ECSManager.GetComponents<RotationVelocityComponent>();
        queryedEntities ??= ECSManager.GetEntitiesWhitComponentTypes(typeof(RotationComponent), typeof(RotationVelocityComponent));
    }

    protected override void Execute(float deltaTime)
    {
        Parallel.ForEach(queryedEntities, parallelOptions, i =>
        {
            rotationComponents[i].X += rotationVelocityComponents[i].X * rotationVelocityComponents[i].velocity * deltaTime;
            rotationComponents[i].Y += rotationVelocityComponents[i].Y * rotationVelocityComponents[i].velocity * deltaTime;
            rotationComponents[i].Z += rotationVelocityComponents[i].Z * rotationVelocityComponents[i].velocity * deltaTime;
        });
    }

    protected override void PostExecute(float deltaTime)
    {
    }
}
