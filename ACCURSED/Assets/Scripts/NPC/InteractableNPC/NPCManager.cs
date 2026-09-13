using System;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    [Serializable]
    public class NPCSpawnState
    {
        public Vector2 position;
        public float chanceOfState = 1f;
        public string animationName;
    }

    [SerializeField] private List<NPCSpawnState> spawnStates = new List<NPCSpawnState>();
    [SerializeField] private CharacterAnimator characterAnimation;

    void Start()
    {
        if (characterAnimation == null)
            characterAnimation = GetComponent<CharacterAnimator>();

        NPCSpawnState chosenState = ChooseSpawnState();
        if (chosenState == null) return;

        transform.position = chosenState.position;

        if (characterAnimation != null && !string.IsNullOrEmpty(chosenState.animationName))
            characterAnimation.SwitchAnimationState(chosenState.animationName);
    }

    private NPCSpawnState ChooseSpawnState()
    {
        if (spawnStates == null || spawnStates.Count == 0) return null;

        float totalWeight = 0f;
        foreach (NPCSpawnState state in spawnStates)
            totalWeight += Mathf.Max(0f, state.chanceOfState);

        if (totalWeight <= 0f) return spawnStates[0];

        float roll = UnityEngine.Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (NPCSpawnState state in spawnStates)
        {
            cumulative += Mathf.Max(0f, state.chanceOfState);
            if (roll <= cumulative)
                return state;
        }

        return spawnStates[spawnStates.Count - 1];
    }

    // Update is called once per frame
    void Update()
    {

    }
}
