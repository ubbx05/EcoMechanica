using UnityEngine;

public class Extractor //Context of Strategy Pattern
{
    ExtractingStrategy extractingStrategy;

    public Extractor()
    {

    }
    public Extractor(ExtractingStrategy extractingStrategy)
    {
        this.extractingStrategy =  extractingStrategy;
    }
    public void extractResource() {
        extractingStrategy.extract();
    }
}

