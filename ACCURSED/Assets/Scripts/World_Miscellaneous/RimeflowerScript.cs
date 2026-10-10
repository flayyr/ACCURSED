using UnityEngine;

public class RimeflowerScript : InteractableObjectManager, IResetable, ICullable
{
    [SerializeField] float griefGain;
    [Space]
    [SerializeField] GameObject bloomSprite;
    [SerializeField] GameObject pickedSprite;
    [SerializeField] GameObject brokenSprite;

    public enum State {bloom, picked, broken}

    public State state = State.bloom;
    private bool picked;
    private PlayerStatistics pStats;

    private void Start()
    {
        SwitchState(state);
        OnPromptTriggered += PickFlower;
        pStats = PersistentPlayer.Instance.gameObject.GetComponent<PlayerStatistics>();
        ResetObjectsManager.Instance.AddResetable(this);
        CullManager.instance.AddObject(this, transform.position);
    }

    private void OnDestroy() {
        ResetObjectsManager.Instance.RemoveResetable(this);
    }

    //Collisions and Interactable Manager calls -----
    protected override void Update() {
        if (state==State.bloom) {
            base.Update();
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.tag == "Player")
            base.OnTriggerEnter2D(collision);
        else if(collision.gameObject.tag == "Illusion") {
            SwitchState(State.broken);
        }
    }

    protected override void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.tag == "Player")
            base.OnTriggerExit2D(collision);
        else if (collision.gameObject.tag == "Illusion") {
            SwitchState(picked ? State.picked : State.bloom);
        }
    }

    protected override void OnTriggerStay2D(Collider2D collision) {
        if (collision.gameObject.tag == "Player")
            base.OnTriggerStay2D(collision);
    }
    //-----------------------------------------------

    private void PickFlower() {
        if (state!=State.bloom) {
            Debug.LogError("tried to pick rimeflower when not blooming");
            return;
        }

        SwitchState(State.picked);
        pStats.UpdateVitality(griefGain);
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

    public void Reset() {
        SwitchState(State.bloom);
    }

    public void SetVisibility(bool visible) {
        gameObject.SetActive(visible);
    }
}