using UnityEngine;

// Computer Assembly Strategy - Workshop pattern'ine uygun
public class ComputerAssembleStrategy : DualResourceAssemblyStrategy
{
    public ComputerAssembleStrategy()
    {
        neededProductA = 1; // 1 CircuitBoard
        neededProductB = 1; // 1 Steel
        neededAmount = 2;   // Toplam (tutarlýlýk için)
    }

    public override GameObject GetOutputPrefab(Assembler assembler)
    {
        // Workshop/Furnace gibi - sadece hangi prefab'ý döndüreceðini belirt
        return assembler.computer_prefab;
    }
}