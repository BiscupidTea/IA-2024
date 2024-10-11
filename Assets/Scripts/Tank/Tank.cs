using UnityEngine;

public class Tank : TankBase
{
    float fitness = 0;

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

        SetForces(output[0], output[1], dt);

        EvaluateFitness(output[0], output[1]);
    }

    private void EvaluateFitness(float Right, float Left)
    {
        if (Right == 0 && Left == 0)
        {
            fitness += 20 * mineFitness;
            genome.fitness = fitness;
        }
    }

    protected override void OnTakeMine(GameObject mine)
    {
        if (isGoodMine == 1)
        {
            if (mineFitness < maxfitness)
            {
                mineFitness *= 1.1f;
            }
        }
        else
        {
            if (mineFitness > minfitness)
            {
                mineFitness *= 0.9f;
            }
        }
    }
}