using UnityEngine;

public class Tank : TankBase
{
    float fitness = 0;
    float lastDistanceToMine = 0;

    protected override void OnReset()
    {
        fitness = 1;
    }

    protected override void OnThink(float dt)
    {
        Vector3 dirToMine = GetDirToMine(nearMine);

        inputs[0] = dirToMine.x;
        inputs[1] = dirToMine.z;
        inputs[2] = transform.forward.x;
        inputs[3] = transform.forward.z;
        inputs[4] = isGoodMine;

        float[] output = brain.Synapsis(inputs);

        EvaluateFitness(output[0], output[1], dt);
        
        SetForces(output[0], output[1], dt);
    }

 private void EvaluateFitness(float Right, float Left, float dt)
    {
        float currentDistanceToMine = GetDistanceToMine(nearMine);

        if (isGoodMine > 0)
        {
            if (currentDistanceToMine < lastDistanceToMine)
            {
                fitness *= 1.1f;
            }
            else
            {
                fitness *= 0.9f;
            }
        }
        else
        {
            if (currentDistanceToMine > lastDistanceToMine)
            {
                fitness *= 1.1f;
            }
            else
            {
                fitness *= 0.9f;
            }
        }
        
        fitness = Mathf.Max(fitness, 0.1f);
        genome.fitness = fitness;
        lastDistanceToMine = currentDistanceToMine;
    }

    protected override void OnTakeMine(IMineTank mine)
    {
        if (mine.IsGoodMine())
        {
            fitness *= 1.1f; // Incremento de fitness en un 10% por recoger una mina buena.
        }
        else
        {
            fitness *= 0.9f; // Reducción de fitness en un 10% por recoger una mina mala.
        }
        
        fitness = Mathf.Max(fitness, 0.1f);
        genome.fitness = fitness;
    }

    private float GetDistanceToMine(IMineTank mine)
    {
        if (mine == null) return float.MaxValue;
        return Vector3.Distance(transform.position, mine.GetPosition());
    }
}