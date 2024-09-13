public interface ICoordType<T>
{
    public void Init(params T[] parameter);
    public T[] GetXY();

    public float DistanceTo(T[] coordinates);
}
