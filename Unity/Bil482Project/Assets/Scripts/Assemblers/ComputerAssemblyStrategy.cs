using UnityEngine;

// Computer Assembly Strategy - Workshop pattern'ine uygun
public class ComputerAssembleStrategy : DualResourceAssemblyStrategy
{
    public ComputerAssembleStrategy()
    {
        neededProductA = 1; // 1 CircuitBoard
        neededProductB = 1; // 1 Steel
        neededAmount = 2;   // Toplam (tutarl�l�k i�in)
    }

    public override GameObject GetOutputPrefab(Assembler assembler)
    {
        // Workshop/Furnace gibi - sadece hangi prefab'� d�nd�rece�ini belirt
        return assembler.computer_prefab;
    }
}