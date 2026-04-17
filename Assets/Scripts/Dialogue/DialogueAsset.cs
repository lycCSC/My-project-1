using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueAsset", menuName = "Dialogue/Dialogue Asset")]
public class DialogueAsset : ScriptableObject
{
    public string dialogueId = "dialogue-id";
    public string displayName = "Dialogue";
    public string startNodeId = "start";
    public List<DialogueNodeData> nodes = new();

    Dictionary<string, DialogueNodeData> nodeLookup;

    void OnEnable()
    {
        RebuildLookup();
    }

    public DialogueNodeData GetStartNode()
    {
        return GetNode(startNodeId);
    }

    public DialogueNodeData GetNode(string nodeId)
    {
        if (string.IsNullOrWhiteSpace(nodeId))
        {
            return null;
        }

        if (nodeLookup == null || nodeLookup.Count != nodes.Count)
        {
            RebuildLookup();
        }

        nodeLookup.TryGetValue(nodeId, out DialogueNodeData nodeData);
        return nodeData;
    }

    void RebuildLookup()
    {
        nodeLookup = new Dictionary<string, DialogueNodeData>();
        foreach (DialogueNodeData node in nodes)
        {
            if (node == null || string.IsNullOrWhiteSpace(node.nodeId) || nodeLookup.ContainsKey(node.nodeId))
            {
                continue;
            }

            nodeLookup.Add(node.nodeId, node);
        }
    }
}

[Serializable]
public class DialogueNodeData
{
    public string nodeId;
    public string speakerName;
    [TextArea(2, 6)]
    public string message;
    public string nextNodeId;
    public bool isEndNode;
    public List<DialogueChoiceData> choices = new();
}

[Serializable]
public class DialogueChoiceData
{
    public string choiceText;
    public string nextNodeId;
}
