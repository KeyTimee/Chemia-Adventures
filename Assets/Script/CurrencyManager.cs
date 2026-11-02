using System;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    [Header("Currency Settings")]
    [SerializeField] private int quarkAmount = 0;

    // Event otomatis untuk memperbarui UI setiap ada perubahan angka Quark
    public event Action<int> OnQuarkChanged;

    public int CurrentQuark => quarkAmount;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Fungsi menambah Quark (Hadiah Quest)
    public void AddQuark(int amount)
    {
        if (amount <= 0) return;

        quarkAmount += amount;
        Debug.Log($"[Currency] +{amount} Quark. Total: {quarkAmount}");
        OnQuarkChanged?.Invoke(quarkAmount);
    }

    // Fungsi mengurangi Quark (Jika beli sesuatu nanti)
    public bool SpendQuark(int amount)
    {
        if (amount <= 0) return false;

        if (quarkAmount >= amount)
        {
            quarkAmount -= amount;
            Debug.Log($"[Currency] -{amount} Quark. Sisa: {quarkAmount}");
            OnQuarkChanged?.Invoke(quarkAmount);
            return true;
        }

        Debug.Log("[Currency] Quark tidak cukup!");
        return false;
    }
}