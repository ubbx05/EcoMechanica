using UnityEngine;

public class GameController : MonoBehaviour
{
    Extractor bronzeExtractor;
    Extractor ironExtractor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bronzeExtractor = new Extractor(new BronzeExtractingStrategy());
        ironExtractor = new Extractor(new IronExtractingStrategy());
    }

    float extractingTimer = 3f;

    // Update is called once per frame
    void Update()
    {
        extractingTimer -= Time.deltaTime;
        if (extractingTimer < 0)
        {
            bronzeExtractor.extractResource();
            ironExtractor.extractResource();
            extractingTimer = 3f;
        }
            
    }
}
