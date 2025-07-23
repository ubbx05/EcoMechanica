using UnityEngine;

// CircuitBoard Assembly Strategy - Workshop pattern'ine uygun
public class CircuitBoardAssembleStrategy : DualResourceAssemblyStrategy
{
    public CircuitBoardAssembleStrategy()
    {
        neededProductA = 1; // 1 DemirIngot
        neededProductB = 1; // 1 Magnet
        neededAmount = 2;   // Toplam (tutarl�l�k i�in)
    }

    public override GameObject GetOutputPrefab(Assembler assembler)
    {
        // Workshop/Furnace gibi - sadece hangi prefab'� d�nd�rece�ini belirt
        return assembler.circuitBoard_prefab;
    }
}