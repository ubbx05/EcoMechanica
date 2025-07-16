using UnityEngine;

public class LevelExtractingStrategyDecorator : ExtractingStrategy
{
    private ExtractingStrategy baseStrategy;
    private int level;

    public LevelExtractingStrategyDecorator(ExtractingStrategy baseStrategy, int level)
    {
        this.baseStrategy = baseStrategy;
        this.level = level;
    }

    public void extract()
    {
        // Seviye 2 ise 2 kat extract et
        for (int i = 0; i < level; i++)
        {
            baseStrategy.extract();
        }
    }
}