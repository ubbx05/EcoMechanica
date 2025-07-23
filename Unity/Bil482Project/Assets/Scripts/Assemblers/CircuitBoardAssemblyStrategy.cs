using UnityEngine;

// CircuitBoard Assembly Strategy - Workshop pattern'ine uygun
public class CircuitBoardAssembleStrategy : DualResourceAssemblyStrategy
{
    public CircuitBoardAssembleStrategy()
    {
        neededProductA = 1; // 1 DemirIngot
        neededProductB = 1; // 1 Magnet
        neededAmount = 2;   // Toplam (tutarlýlýk için)
    }

    public override GameObject GetOutputPrefab(Assembler assembler)
    {
        // Workshop/Furnace gibi - sadece hangi prefab'ý döndüreceðini belirt
        return assembler.circuitBoard_prefab;
    }
}