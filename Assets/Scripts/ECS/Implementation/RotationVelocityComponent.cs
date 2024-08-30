public class RotationVelocityComponent : ECSComponent
{

    public float velocity;

    public float X;
    public float Y;
    public float Z;

    public RotationVelocityComponent(float velocity, float X, float Y, float Z)
    {
        this.velocity = velocity;

        this.X = X;
        this.Y = Y;
        this.Z = Z;
    }
}
