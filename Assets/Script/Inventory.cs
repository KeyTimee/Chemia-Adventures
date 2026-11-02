using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory
{
    // 🔥 EVENT: dipanggil saat inventory berubah
    public Action onInventoryChanged;

    [System.Serializable]
    public class Slot
    {
        public CollectableType type;
        public int count;
        public int maxAllowed;
        public Sprite icon;

        public Slot()
        {
            type = CollectableType.NONE;
            count = 0;
            maxAllowed = 99; // 🔥 Ubah default max stack (misal 99)
        }

        public bool IsEmpty()
        {
            return type == CollectableType.NONE;
        }

        public bool CanAddItem(CollectableType itemType)
        {
            // Pastikan tipe item sama dan jumlah belum melebihi kapasitas max stack
            return type == itemType && count < (maxAllowed > 0 ? maxAllowed : 99);
        }

        public void AddItem(CollectableType itemType)
        {
            if (IsEmpty())
            {
                type = itemType;
            }

            count++;
        }

        public void RemoveItem()
        {
            count--;

            if (count <= 0)
            {
                type = CollectableType.NONE;
                count = 0;
            }
        }
    }

    public List<Slot> slots = new List<Slot>();

    public Inventory(int numSlots)
    {
        for (int i = 0; i < numSlots; i++)
        {
            slots.Add(new Slot());
        }
    }

    // 🔥 TAMBAH ITEM
    public void Add(CollectableType typeToAdd)
    {
        // 1. Tambah ke slot yang sudah ada yang tipenya sama dan belum penuh (stack)
        foreach (Slot slot in slots)
        {
            if (slot.CanAddItem(typeToAdd))
            {
                slot.AddItem(typeToAdd);
                onInventoryChanged?.Invoke(); // 🔥 TRIGGER UI UPDATE
                return;
            }
        }

        // 2. Jika tidak ada slot bertipe sama yang bisa di-stack, cari slot kosong
        foreach (Slot slot in slots)
        {
            if (slot.IsEmpty())
            {
                slot.AddItem(typeToAdd);
                onInventoryChanged?.Invoke(); // 🔥 TRIGGER UI UPDATE
                return;
            }
        }

        // 3. Inventory penuh
        Debug.Log("Inventory penuh!");
    }

    public void Swap(int indexA, int indexB)
    {
        if (indexA == indexB) return;

        if (indexA < 0 || indexA >= slots.Count) return;
        if (indexB < 0 || indexB >= slots.Count) return;

        Slot temp = slots[indexA];
        slots[indexA] = slots[indexB];
        slots[indexB] = temp;

        onInventoryChanged?.Invoke(); // 🔥 trigger UI update
    }

    // 🔥 REMOVE ITEM
    public void Remove(CollectableType typeToRemove)
    {
        foreach (Slot slot in slots)
        {
            if (slot.type == typeToRemove)
            {
                slot.RemoveItem();
                onInventoryChanged?.Invoke(); // 🔥 TRIGGER UI UPDATE
                return;
            }
        }
    }

    public void RemoveAt(int index)
    {
        if (index < 0 || index >= slots.Count) return;

        slots[index].RemoveItem();

        onInventoryChanged?.Invoke(); // 🔥 update UI
    }

    public bool Contains(CollectableType itemType)
    {
        foreach (var slot in slots)
        {
            if (slot.type == itemType && slot.count > 0)
            {
                return true;
            }
        }
        return false;
    }
}