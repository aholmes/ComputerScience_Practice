<Query Kind="Program" />

void Main()
{	
	// add items to a hashmap
	var hm = new MyHashSet<string>();
	hm.Add("Foo");
	hm.Add("Foo");
	hm.Add("Foo");
	hm.Add("Foo");
	hm.Add("Foo");
	hm.Add("Foo");
	hm.Add("Bar");
	hm.Add("Bar");
	/*
	hm.Add("Baz");
	hm.Add("FooBar");
	hm.Add("FooBar");
	hm.Add("FooBar");
	hm.Add("BarBaz");
	hm.Add("FooBarBaz");
	hm.Add("FooBarBaz");
	hm.Add("FooBarBaz");*/

	// get a value and test that it's correct
	var barBaz = hm.Get("BarBaz");
	//var a = barBaz.Value == "BarBaz";

	// get a value that doesn't exist
	var fooBarBazQux = hm.Get("FooBarBazQux");

	new[]
	{
		hm.Get("Foo"),
		hm.Get("Bar"),
		hm.Get("Baz"),
		hm.Get("FooBar"),
		hm.Get("BarBaz"),
		hm.Get("FooBarBaz"),
		fooBarBazQux
	}.Dump();

	hm.buckets.Dump();
}

class MyHashSet<T>
{
	private const int bucketSize = 16;
	// items are stored in buckets, and the bucket number is determined by the hash
	public Node<T>[] buckets = new Node<T>[bucketSize];

	// simple hash method
	private int getHash(T value)
	{
		return (value as string)?.Length ?? value.GetHashCode();
	}

	private int getBucketNumber(T value)
	{
		var hash = getHash(value);

		return Math.Abs(hash % bucketSize);
	}

	// add method - calculates a hash, adds a node to a list
	public void Add(T value)
	{
		// this needs to check if there is already an entry for the hash, then add to the linked list
		var bucket = getBucketNumber(value);

		var node = buckets[bucket]; // 3

		// bucket is empty, so never added
		if (node == null)
		{
			buckets[bucket] = new Node<T>
			{
				Value = value
			};
			return;
		}
		
		while (node.Next != null)
		{
			if (node.Value.Equals(value)) return;

			node = node.Next;
		}
		if (node.Value.Equals(value)) return;

		node.Next = new Node<T>
		{
			Value = value
		};
	}

	// get method - calculates a hash, looks for the hash value in the list, returns the value
	public T Get(T value)
	{
		var bucket = getBucketNumber(value);

		var node = buckets[bucket];

		if (node == null) return default(T);

		while (node != null)
		{
			if (node.Value.Equals(value)) return node.Value;
			node = node.Next;
		}

		return default(T);
	}
}

class Node<T>
{
	public T Value;

	public Node<T> Next;
}