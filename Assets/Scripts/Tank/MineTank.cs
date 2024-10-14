using UnityEngine;

public class MineTank : MonoBehaviour, IMineTank
{
    private bool isGoodMine = false;

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public bool IsGoodMine()
    {
        return isGoodMine;
    }

    public void SetMine(bool value)
    {
        isGoodMine = value;
        GetComponent<MeshRenderer>().material.color = value ? Color.green : Color.red;
    }

    public void SetPosition(Vector3 newPosition)
    {
        transform.position = newPosition;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }
}

public interface IMineTank
{
    public Vector3 GetPosition();
    public bool IsGoodMine();
    public void SetMine(bool value);
    void SetPosition(Vector3 newPosition);
    GameObject GetGameObject();
}