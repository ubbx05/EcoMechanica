using UnityEngine;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    // Scene'deki tüm extractor'larý tutacak liste
    private List<Extractor> extractors = new List<Extractor>();

    // Extraction timer
    private float extractingTimer = 3f;

    void Start()
    {
        // Scene'deki tüm Extractor'larý bul
        GameObject[] extractorObjects = GameObject.FindGameObjectsWithTag("Extractor");

        foreach (GameObject extractorObj in extractorObjects)
        {
            Extractor extractor = extractorObj.GetComponent<Extractor>();
            if (extractor != null)
            {
                extractors.Add(extractor);
            }
        }

        //Debug.Log($"Found {extractors.Count} extractors in the scene");
    }

    void Update()
    {
        extractingTimer -= Time.deltaTime;

        if (extractingTimer < 0)
        {
            // Tüm extractor'larý çalýþtýr
            foreach (Extractor extractor in extractors)
            {
                extractor.ExtractResource();
            }

            extractingTimer = 3f;
        }
    }

    // Yeni extractor ekleme metodu
    public void AddExtractor(Extractor newExtractor)
    {
        if (!extractors.Contains(newExtractor))
        {
            extractors.Add(newExtractor);
        }
    }

    // Extractor kaldýrma metodu
    public void RemoveExtractor(Extractor extractorToRemove)
    {
        if (extractors.Contains(extractorToRemove))
        {
            extractors.Remove(extractorToRemove);
        }
    }
}