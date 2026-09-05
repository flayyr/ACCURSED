using UnityEngine;


[CreateAssetMenu(fileName = "NewNPCDialogue", menuName = "NPC Dialogue")]
public class NPCDialogue : ScriptableObject
{
    //to be implemeneted later
    /*
    public string npcName;
    public Sprite npcPortrait;
    public AudioSourceOrWhatever npcTalkSound;

    */

    [Header("Insert dialogue here; press the plus arrow, \n each array spot is a line of dialogue.")]
    [TextArea(1, 100)]
    public string[] dialogueList;

    [Header("This should be at which point in the conversation \n the branches; count from 0")]
    public int branchNum;

    [Header("Switch this on if the conversation has branching \n options. If it doesn't, then leave it off.")]
    public bool branching;

    [Header("This holds all the branching dialogues.")]
    public NPCDialogue[] branches;

    public AudioClip sound;
}
