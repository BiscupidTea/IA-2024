using UnityEngine;
using System.Collections;

public class TankBase : MonoBehaviour
{
    public float Speed = 10.0f;
    public float RotSpeed = 20.0f;

    protected Genome genome;
	protected NeuralNetwork brain;
    protected IMineTank nearMine;
    protected IMineTank goodMine;
    protected IMineTank badMine;
    protected float[] inputs;
    protected float isGoodMine;
    
    protected float mineFitness = 1;
    protected float maxfitness = 2;
    protected float minfitness = 0;
    
    protected float LastDistanceMine = 0;

    public void SetBrain(Genome genome, NeuralNetwork brain)
    {
        this.genome = genome;
        this.brain = brain;
        inputs = new float[brain.InputsCount];
        OnReset();
    }

    public void SetNearestMine(IMineTank mine)
    {
        nearMine = mine;
    }

    public void SetGoodNearestMine(IMineTank mine)
    {
        goodMine = mine;
    }

    public void SetBadNearestMine(IMineTank mine)
    {
        badMine = mine;
    }

    protected bool IsGoodMine(IMineTank mine)
    {
        return goodMine == mine;
    }

    protected Vector3 GetDirToMine(IMineTank mine)
    {
        return (mine.GetPosition() - transform.position).normalized;
    }
    
    protected bool IsCloseToMine(IMineTank mine)
    {
        return (this.transform.position - nearMine.GetPosition()).sqrMagnitude <= 2.0f;
    }

    protected void SetForces(float leftForce, float rightForce, float dt)
    {
        Vector3 pos = this.transform.position;
        float rotFactor = Mathf.Clamp((rightForce - leftForce), -1.0f, 1.0f);
        this.transform.rotation *= Quaternion.AngleAxis(rotFactor * RotSpeed * dt, Vector3.up);
        pos += this.transform.forward * Mathf.Abs(rightForce + leftForce) * 0.5f * Speed * dt;
        this.transform.position = pos;
    }

	public void Think(float dt) 
	{
        OnThink(dt);

        if(IsCloseToMine(nearMine))
        {
            OnTakeMine(nearMine);
            PopulationManager.Instance.RelocateMine(nearMine);
        }
	}

    protected virtual void OnThink(float dt)
    {

    }

    protected virtual void OnTakeMine(IMineTank mine)
    {
    }

    protected virtual void OnReset()
    {

    }
}
