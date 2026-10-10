using UnityEngine;

public class RimeflowerScript : MonoBehaviour
{
    [SerializeField] GameObject bloomSprite;
    [SerializeField] GameObject pickedSprite;
    [SerializeField] GameObject brokenSprite;

    public enum State {bloom, picked, broken}

    public State state = State.bloom;
    private bool picked;

    private void Awake()
    {
        SwitchState(state);
    }

    public State PlayerInteract()
    {
        if (state == State.picked){
            return state;
        }
        else if (state == State.bloom) { 
            SwitchState(State.picked);
            return State.bloom; 
        } else
        {
            return State.broken;
        }
    }

    public void BubbleInteract(bool touching)
    {
        if (touching)
        {
            SwitchState(State.broken);
        }
        else
        {
            SwitchState(picked?State.picked : State.bloom);
        }
    }

    private void SwitchState(State s)
    {
        bloomSprite.SetActive(false);
        pickedSprite.SetActive(false);
        brokenSprite.SetActive(false);

        state = s;
        switch (state)
        {
            case State.bloom:
                picked = false;
                bloomSprite.SetActive(true);
                break;
            case State.picked:
                picked = true;
                pickedSprite.SetActive(true);
                break;
            case State.broken:
                brokenSprite.SetActive(true);
                break;
        }
    }

}