using UnityEngine;

public abstract class NetworkAIMovement : ScriptableObject
{
    protected NetworkAIPlayerController Target { get; private set; }

    public void SetTarget(NetworkAIPlayerController target )
    {
        Target = target;
        OnSetTarget();
    }


    protected virtual void OnSetTarget()
    {

    }
    public abstract void Move();

}