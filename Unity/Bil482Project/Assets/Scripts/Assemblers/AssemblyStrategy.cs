using UnityEngine;

// Assembly Strategy abstract sýnýfý - Workshop ve Furnace ile tutarlý
public abstract class AssemblyStrategy
{
    public int neededAmount; // Tek resource için (tutarlýlýk için)

    // Çoklu resource için - base'de tanýmla ki tüm strategy'ler eriþebilsin
    public virtual int neededProductA { get; set; } = 0;
    public virtual int neededProductB { get; set; } = 0;

    public abstract void Produce(GameObject input, Assembler assembler); // Tutarlý metod ismi
}

// Çoklu resource için geniþletilmiþ versiyon
public abstract class DualResourceAssemblyStrategy : AssemblyStrategy
{
    // Base'de tanýmlandý, override edilebilir
    public override int neededProductA { get; set; }
    public override int neededProductB { get; set; }

    // Hangi prefab'ý çýkaracaðýný belirtir - ABSTRACT yapýldý
    public abstract GameObject GetOutputPrefab(Assembler assembler);

    public override void Produce(GameObject input, Assembler assembler)
    {
        // Workshop/Furnace gibi - sadece hangi prefab'ý çýkaracaðýný belirle
        // Gerçek üretim Assembler'da yapýlacak
        GameObject outputPrefab = GetOutputPrefab(assembler);
        if (outputPrefab != null)
        {
            assembler.SpawnResource(outputPrefab);
        }
        else
        {
            Debug.LogError($"Output prefab is null in {GetType().Name}!");
        }
    }
}