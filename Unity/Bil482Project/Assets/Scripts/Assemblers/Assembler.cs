using System.Collections.Generic;
using UnityEngine;

// Define your custom types
public class MyGraphType {}
public class MyMacroType {}

// Define the base Machine class with generics
public abstract class Machine<TGraph, TMacro> : MonoBehaviour
{
    public abstract void AcceptProduct(GameObject product);
    public abstract bool CanAcceptProduct(GameObject product);
    public abstract GameObject GetOutputProduct();
    public abstract bool HasProductToOutput();
}

public class Assembler : Machine<MyGraphType, MyMacroType>
{
    public GameObject outputPrefab; // Üretilecek ürünün prefabı
    private List<GameObject> inputProducts = new List<GameObject>(); // Girdi ürünleri
    private GameObject outputProduct; // Üretilmiş ürün
    public int requiredInputCount = 2; // Kaç girdi ile üretim yapılacak
    public float assemblyTime = 2.0f; // Montaj süresi (saniye)
    private float timer = 0f;
    private bool isAssembling = false;

    public override void AcceptProduct(GameObject product)
    {
        if (CanAcceptProduct(product))
        {
            inputProducts.Add(product);
            product.SetActive(false); // Ürünü gizle (Destroy yerine)

            if (inputProducts.Count >= requiredInputCount && !isAssembling)
            {
                isAssembling = true;
                timer = 0f;
            }
        }
    }

    public override bool CanAcceptProduct(GameObject product)
    {
        return inputProducts.Count < requiredInputCount && !isAssembling;
    }

    public override GameObject GetOutputProduct()
    {
        if (outputProduct != null)
        {
            GameObject result = outputProduct;
            outputProduct = null;
            return result;
        }
        return null;
    }

    public override bool HasProductToOutput()
    {
        return outputProduct != null;
    }

    void Update()
    {
        if (isAssembling)
        {
            timer += Time.deltaTime;
            
            if (timer >= assemblyTime)
            {
                CompleteAssembly();
                isAssembling = false;
            }
        }
    }

    private void CompleteAssembly()
    {
        // Yeni ürün oluştur
        outputProduct = Instantiate(outputPrefab, transform.position, Quaternion.identity);
        
        // Eski ürünleri temizle
        foreach (var product in inputProducts)
        {
            Destroy(product);
        }
        inputProducts.Clear();
    }

    void Start()
    {
        outputProduct = null;
    }
}