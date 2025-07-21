using System.Collections.Generic;

public class ResearchNode
{
    public string id;
    public string displayName;
    public bool isStarted = false;
    public bool isCompleted = false;
    public List<ResearchNode> parents = new();

    public void StartResearch()
    {
        isStarted = true;
        // Coroutine vs ile ilerleme yapýlabilir
    }

    public void CancelResearch()
    {
        isStarted = false;
    }
}