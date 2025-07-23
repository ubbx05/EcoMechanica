using UnityEngine;

// Assembly Strategy abstract s�n�f� - Workshop ve Furnace ile tutarl�
public abstract class AssemblyStrategy
{
    public int neededAmount; // Tek resource i�in (tutarl�l�k i�in)

    // �oklu resource i�in - base'de tan�mla ki t�m strategy'ler eri�ebilsin
    public virtual int neededProductA { get; set; } = 0;
    public virtual int neededProductB { get; set; } = 0;

    public abstract void Produce(GameObject input, Assembler assembler); // Tutarl� metod ismi
}

// �oklu resource i�in geni�letilmi� versiyon
public abstract class DualResourceAssemblyStrategy : AssemblyStrategy
{
    // Base'de tan�mland�, override edilebilir
    public override int neededProductA { get; set; }
    public override int neededProductB { get; set; }

    // Hangi prefab'� ��karaca��n� belirtir - ABSTRACT yap�ld�
    public abstract GameObject GetOutputPrefab(Assembler assembler);

    public override void Produce(GameObject input, Assembler assembler)
    {
        // Workshop/Furnace gibi - sadece hangi prefab'� ��karaca��n� belirle
        // Ger�ek �retim Assembler'da yap�lacak
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