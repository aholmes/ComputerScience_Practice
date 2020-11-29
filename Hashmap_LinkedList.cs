#region Usings
System
System.Collections.Generic
System.Diagnostics
System.Linq
#endregion

void Main()
{
    Console.WriteLine("Running tests ...");
    RunTests();
    Console.WriteLine("Done!");
}

#region Tests
void RunTests()
{
    HashCollection<string, string> GetHc() => new HashCollection<string, string>();

    {
        Console.Write("Insert value, get value ... ");
        var hc = GetHc();
        hc.Add("one", "one-value");

        var value = hc.Get("one");
        Debug.Assert(value == "one-value");
        Console.WriteLine("Pass!");
    }
    {
        Console.Write("Change value, get value ... ");
        var hc = GetHc();
        hc.Add("one", "one-value");
        hc.Add("one", "one-value-change");

        var value = hc.Get("one");
        Debug.Assert(value == "one-value-change");
        Console.WriteLine("Pass!");
    }
    {
        Console.Write("Add 2 values, get 2 values ... ");
        var hc = GetHc();
        hc.Add("one", "one-value");
        hc.Add("two", "two-value");


        var value1 = hc.Get("one");
        var value2 = hc.Get("two");
        Debug.Assert(value1 == "one-value");
        Debug.Assert(value2 == "two-value");
        Console.WriteLine("Pass!");
    }
    {
        Console.Write("Add max bucket size values, get max bucket size values ... ");
        var hc = GetHc();
        const int count = 16; // hard-coded bucket size in class
        for (var i = 0; i < count; i++)
        {
            hc.Add(i.ToString(), $"{i}-value");
        }

        var values = new string[count];
        for (var i = 0; i < count; i++)
        {
            values[i] = hc.Get(i.ToString());
        }

        for (var i = 0; i < count; i++)
        {
            Debug.Assert(values[i] == $"{i}-value");
        }
        Console.WriteLine("Pass!");
    }
    {
        Console.Write("Add 2x max bucket size values, get 2x max bucket size values ... ");
        var hc = GetHc();
        const int count = 32; // 2x hard-coded bucket size in class
        for (var i = 0; i < count; i++)
        {
            hc.Add(i.ToString(), $"{i}-value");
        }

        var values = new string[count];
        for (var i = 0; i < count; i++)
        {
            values[i] = hc.Get(i.ToString());
        }

        for (var i = 0; i < count; i++)
        {
            Debug.Assert(values[i] == $"{i}-value");
        }
        Console.WriteLine("Pass!");
    }
    {
        Console.Write("Change 2x max bucket size values, get 2x max bucket size values ... ");
        var hc = GetHc();
        const int count = 32; // 2x hard-coded bucket size in class
        for (var i = 0; i < count; i++)
        {
            hc.Add(i.ToString(), $"{i}-value");
        }
        for (var i = 0; i < count; i++)
        {
            hc.Add(i.ToString(), $"{i}-value-change");
        }

        var values = new string[count];
        for (var i = 0; i < count; i++)
        {
            values[i] = hc.Get(i.ToString());
        }

        for (var i = 0; i < count; i++)
        {
            Debug.Assert(values[i] == $"{i}-value-change");
        }
        Console.WriteLine("Pass!");
    }
}
#endregion

public class HashCollection<K, V>
    where K: IEnumerable<char> // strings
{
    private const uint bucketSize = 16;
    private Node[] buckets = new Node[bucketSize];

    public HashCollection(uint capacity = bucketSize)
    {
        buckets = new Node[bucketSize];
    }

    public void Add(K key, V value)
    {
        var bucketNum = getBucketNum(key);
        var node = buckets[bucketNum]; // get a Node<K,V>

        var insertNode = new Node(key, value);

        if (node == null)
        {
            buckets[bucketNum] = insertNode;
        }
        else
        {
            // Check all linked nodes in the bucket
            var enumerator = node.GetEnumerator();
            while(enumerator.MoveNext())
            {
                if (enumerator.Current.Key.Equals(key))
                {
                    enumerator.Current.Value = value;
                    return;
                }
            }
	    //
            // The last node has no linked node
            // we have to set the value now
            if (enumerator.Current.Key.Equals(key))
            {
                enumerator.Current.Value = value;
            }
            // if the key is not in the linked list, then add a new node
            else
            {
                enumerator.Current.Next = insertNode;
            }
        }
    }

    public V Get(K key)
    {
        var bucketNum = getBucketNum(key);
        var node = buckets[bucketNum];

        foreach (var linkedNode in node)
        {
            if (linkedNode.Key.Equals(key)) return linkedNode.Value;
        }

        return default;
    }

    private long getBucketNum(K str) => return Math.Abs(getHash(str) % bucketSize);

    private long getHash(K str)
    {
        // polynomial rolling hash function
        // https://cp-algorithms.com/string/string-hashing.html
        var alpha_length_prime = 53;
        var mod = (long)1e9 + 9;
        long hash_value = 0;
        long prime_power = 1;

        foreach(var c in str)
        {
            var char_int = (c - 'a' + 1);
            hash_value = (hash_value + char_int * prime_power) % mod;
            prime_power = (prime_power * alpha_length_prime) % mod;
        }

        return hash_value;
    }

    private class Node
    {
        public K Key { get; private set; }
        public V Value { get; set; }
        public Node Next { get; set; }

        public Node(K key, V value, Node next = null)
        {
            Key = key;
            Value = value;
            Next = next;
        }

        public IEnumerator<Node> GetEnumerator()
        {
            var next = this;

            do
            {
                yield return next;
                next = next.Next;
            } while (next != null);

            yield break;
        }
    }
}
