using UnityEngine;

public class Triggerable : MonoBehaviour
{
    protected bool activated;
    public virtual void Trigger(bool activate) {
        activated = activate;
    }

    public virtual void InitialState(bool activate) { 
        activated = activate; 
    }

    public bool isActivated() { return activated; }
}
