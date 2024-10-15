using System;
using UnityEngine;

public class Tank : TankBase
{
    float fitness = 1;
    float lastDistanceToMine = 0;

    private float timeDistance = 5;
    private float timerDistance = 0;

    private float timeSerchingMine = 10;
    private float timerSerchingMine = 0;

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

        MathF.Max(fitness, 1);
        genome.fitness = fitness;

        SetForces(output[0], output[1], dt);
    }

    private void EvaluateFitness(float Right, float Left, float dt)
    {
        timerDistance -= dt;
        timerSerchingMine -= dt;

        if (timeDistance <= 0)
        {
            float currentDistanceToMine = GetDistanceToMine(nearMine);

            if (isGoodMine > 0)
            {
                if (currentDistanceToMine < lastDistanceToMine)
                {
                    fitness += 0.1f;
                }
                else
                {
                    fitness += 0.01f;
                }
            }
            else
            {
                if (currentDistanceToMine > lastDistanceToMine)
                {
                    fitness += 0.1f;
                }
                else
                {
                    fitness += 0.01f;
                }
            }

            if (timeSerchingMine > 0)
            {
                fitness += timeSerchingMine;
            }

            lastDistanceToMine = currentDistanceToMine;

            timerDistance = timeDistance;
            timerSerchingMine = timeSerchingMine;
        }
    }

    protected override void OnTakeMine(IMineTank mine)
    {
        if (mine.IsGoodMine())
        {
            fitness += 5; // Incremento de fitness en un 90% por recoger una mina buena.
        }
        else
        {
            fitness -= 0.5f; // Reducción de fitness en un 90% por recoger una mina mala.
        }
    }

    private float GetDistanceToMine(IMineTank mine)
    {
        if (mine == null) return float.MaxValue;
        return Vector3.Distance(transform.position, mine.GetPosition());
    }
}