void Main()
{
	// didn't need to be nodes, just need key,value
	var nodes = new Node<int, int>[]
	{
		new Node<int, int>(1,1),
		new Node<int, int>(-1,-1),
		new Node<int, int>(2,2),
		new Node<int, int>(10,10),
		new Node<int, int>(5,5),
		new Node<int, int>(-2,-2)
	};
	
	var bst = new BinarySearch<int, int>(new Node<int, int>(0,0));

	foreach (var node in nodes)
	{
		bst.Add(node.Key, node.Value);
	}
	
	bst.Get(2).Dump();
}

class BinarySearch<K, V>
{
	private Node<int,V> _tree;
	
	public BinarySearch(Node<int, V> tree)
	{
		_tree = tree;
	}
	
	// ignore value for now - just use key
	public void Add(int key, V value)
	{
		var newNode = new Node<int, V>(key, value);
		var node = _tree;
		
		do
		{
			// if the same, just insert to the right
			if (node.Key == newNode.Key)
			{
				if (node.Right == null)
				{
					node.Right = newNode;
					return;
				}
				else
					node = node.Right;
			}
			else if (node.Key < newNode.Key)
			{
				if (node.Right == null)
				{
					node.Right = newNode;
					return;
				}
				else
					node = node.Right;
			}
			else if (node.Key > newNode.Key)
			{
				if (node.Left == null)
				{
					node.Left = newNode;
					return;
				}
				else
					node = node.Left;
			}
		} while(node != null);
	}

	public Node<int, V> Get(int key)
	{
		var node = _tree;
		
		do
		{
			if (node.Key == key)
			{
				return node;
			}
			else if (node.Key < key)
			{
				node = node.Right;
			}
			else if (node.Key > key)
			{
				node = node.Left;
			}

		} while (node != null);
		
		return null;
	}
}

public class Node<K,V>
{ 
	public Node<K,V> Left {get; set;}
	public Node<K, V> Right { get; set; }
	public K Key { get; set;}
	public V Value {get;set;}
	
	public Node(K key, V value)
	{
		Key = key;
		Value = value;
	}
}
