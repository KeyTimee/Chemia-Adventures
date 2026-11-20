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
    public int GetItemCount(CollectableType itemType)
    {
        int total = 0;
        foreach (var slot in slots)
        {
            if (slot.type == itemType && slot.count > 0)
            {
                total += slot.count;
            }
        }
        return total;
    }

    // 2. Mengurangi item sebanyak jumlah tertentu (amount)
    public void RemoveAmount(CollectableType typeToRemove, int amount)
    {
        int remaining = amount;
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].type == typeToRemove && slots[i].count > 0)
            {
                int take = Mathf.Min(slots[i].count, remaining);
                slots[i].count -= take;
                remaining -= take;

                if (slots[i].count <= 0)
                {
                    slots[i].type = CollectableType.NONE;
                    slots[i].count = 0;
                }

                if (remaining <= 0) break;
            }
        }
        onInventoryChanged?.Invoke(); // 🔥 Update UI Inventory otomatis
    }

    // 3. Menambah item sebanyak jumlah tertentu (amount)
    public void AddAmount(CollectableType typeToAdd, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Add(typeToAdd);
        }
    }
}