using System;
using System.Collections.Generic;
using UnityEngine;

namespace StepQuest.Adventure
{
    public enum DialogueAction { None, AcceptQuest, ClaimReward }
    [Serializable] public class DialogueReply
    {
        public string text;
        public string nextNode;
        public DialogueAction action;
    }
    [Serializable] public class DialogueNode
    {
        public string id;
        public string speaker = "Mayor Bramble";
        [TextArea(3, 9)] public string text;
        public List<DialogueReply> replies = new List<DialogueReply>();
    }
    [CreateAssetMenu(menuName = "StepQuest/Adventure dialogue")]
    public class AdventureDialogue : ScriptableObject
    {
        [Header("Location visuals")]
        public Sprite forestBackdrop;
        public Sprite npcPortrait;
        public Sprite questIcon;
        [Header("Quest")]
        public string questTitle = "A Sign of Trouble";
        public string encounterStageId = "1.1";
        public int copperReward = 50;
        public string offerNode = "offer", activeNode = "active", victoryNode = "victory", completeNode = "complete";
        public List<DialogueNode> nodes = new List<DialogueNode>();
        public DialogueNode Find(string id) => nodes.Find(n => n.id == id);
    }
}
