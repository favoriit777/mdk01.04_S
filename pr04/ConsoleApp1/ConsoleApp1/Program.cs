using System;
public class MyHashTable<TKey, TValue>
{
    private class Node
    {
        public TKey Key;
        public TValue Value;
        public Node Next;

        public Node(TKey key, TValue value)
        {
            Key = key;
            Value = value;
            Next = null;
        }
    }

    private readonly int size = 100;
    private Node[] buckets;

    public MyHashTable()
    {
        buckets = new Node[size];
    }

    private int GetHash(TKey key)
    {
        return Math.Abs(key.GetHashCode()) % size;
    }

    public void Add(TKey key, TValue value)
    {
        int index = GetHash(key);
        Node newNode = new Node(key, value);

        if (buckets[index] == null)
        {
            buckets[index] = newNode;
        }
        else
        {
            Node current = buckets[index];
            while (current.Next != null && !current.Key.Equals(key))
            {
                current = current.Next;
            }
            if (current.Key.Equals(key))
            {
                current.Value = value; // обновляем значение
            }
            else
            {
                current.Next = newNode;
            }
        }
    }

    public bool ContainsKey(TKey key)
    {
        int index = GetHash(key);
        Node current = buckets[index];
        while (current != null)
        {
            if (current.Key.Equals(key))
                return true;
            current = current.Next;
        }
        return false;
    }

    public TValue Get(TKey key)
    {
        int index = GetHash(key);
        Node current = buckets[index];
        while (current != null)
        {
            if (current.Key.Equals(key))
                return current.Value;
            current = current.Next;
        }
        throw new KeyNotFoundException("Ключ не найден");
    }

    public void Set(TKey key, TValue value)
    {
        int index = GetHash(key);
        Node current = buckets[index];
        while (current != null)
        {
            if (current.Key.Equals(key))
            {
                current.Value = value;
                return;
            }
            current = current.Next;
        }
        throw new KeyNotFoundException("Ключ не найден");
    }
}
class Program
{
    static void Main()
    {
        // Исходный массив голосов
        string[] votes = { "Иван", "Мария", "Иван", "Петр", "Мария", "Иван" };

        // Создаем хэш-таблицу (словарь) для подсчета
        var voteCount = new MyHashTable<string, int>();

        // Подсчет голосов
        foreach (var candidate in votes)
        {
            if (voteCount.ContainsKey(candidate))
            {
                int currentVotes = voteCount.Get(candidate);
                voteCount.Set(candidate, currentVotes + 1);
            }
            else
            {
                voteCount.Add(candidate, 1);
            }
        }

        // Вывод результатов
        Console.WriteLine("Результаты голосования:");
        foreach (var candidate in votes)
        {
            if (voteCount.ContainsKey(candidate))
            {
                Console.WriteLine($"{candidate}: {voteCount.Get(candidate)}");
            }
        }
    }
}