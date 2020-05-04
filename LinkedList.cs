void Main()
{
	var list = new LinkedList();
	list.Add("A");
	list.Add("B");
	list.Add("C");
	list.Add("D");

	var output = GetListOutput(list);
	Console.Write(output.Equals("A -> B -> C -> D -> (null)"));
	Console.WriteLine($" | {output}");
/*
	list.Remove("C");
	output = GetListOutput(list);
	Console.Write(output.Equals("A -> B -> D -> (null)"));
	Console.WriteLine($" | {output}");
*/
// ignore - full of edge cases. moving on to reverse.
	/*list.Swap("D", "A");
	output = GetListOutput(list);
	Console.Write(output.Equals("A -> B -> C -> D -> (null)"));
	Console.WriteLine($" | {output}");*/

	list.Reverse();
	output = GetListOutput(list);
	Console.Write(output.Equals("D -> C -> B -> A -> (null)"));
	Console.WriteLine($" | {output}");
}

string GetListOutput(LinkedList list)
{
	var output = "";
	var node = list.Get();
	do
	{
		if (node.Next == null) output += $"{node.Value} -> (null)";
		else output += $"{node.Value} -> ";
		node = node.Next;
	} while (node != null);
	
	return output;
}

// A linked list consists of a sequence of nodes, with each node pointing to the next in a list.
	// The linked list contains a "head" (pointer to the first node) and a "tail" (pointer to the current last node)
	// Linked lists support operations like: add(value), remove(value), swap(value1, value2), reverse()
	//
	/*
	node <--------\
		value     |
		next node-/
	*/
public class LinkedList
{ 
	public class Node
	{
		public string Value = null;
		public Node Next = null;
	}
	
	private Node _head = null;
	private Node _tail = null;
	
	public Node Get()
	{
		return _head;
	}
	
	// add(value)
	// get the current tail and point its next node to the new node
	// point tail to the next tail (the one just added)
	public void Add(string value)
	{
		var node = new Node { Value = value };
		
		// first one added
		if (_tail == null)
		{
			_head = _tail = node;
		}
		else
		{
			_tail.Next = node;
			_tail = _tail.Next;
		}
	}

	// remove(value)
	// walk the linked list until value is found
	// point the previous node's next to the found node's next
	// Example: remove b
	/*
	node
	value: a
	next:
		node
		value: b
		next:
			node
			value:c
			next:
	*/
	//
	// previous node: null
	// current node: value a
	// a != b
	// set previous node = current node
	// set current node = current node -> next
	// current node: value b
	// b == b
	// set previous node.next = current node.next
	// -end-


	public void Remove(string value)
	{
		Node previous = null;
		Node current = _head;
		
		do {
			if (current.Value.Equals(value))
			{
				// edge case - value is the first one in the list
				if (previous == null)
				{
					_head = current.Next;
					return;
				}
				
				var refToCurrent = current;
				previous.Next = current.Next;
				
				if (refToCurrent.Next == null)
				{
					_tail = previous;
				}
				
				refToCurrent.Next = null; // remove the reference for GC
				
				return;
			}
			else
			{
				previous = current;
				current = current.Next;
			}
		} while(current != null);
	}

	// swap(value1, value2)
	// walk the list looking for value1 and value2
	// point the first node found's parent node.next with the second node found
	// point the second node's next to the first node's next
	// point the first node's next to the second node's next (before the swap above happens)
	//
	public void Swap(string value1, string value2)
	{
		Node previous = null;
		Node current = _head;
		
		Node firstNodesParent = null;
		Node firstFoundNode = null;
		Node secondFoundNode = null;
		
		// True if both nodes found
		bool setFoundNode()
		{
			if (firstFoundNode == null) firstFoundNode = current;
			else if (secondFoundNode == null) secondFoundNode = current;
			
			if (firstNodesParent == null) firstNodesParent = previous;
			
			return firstFoundNode != null && secondFoundNode != null;
		}
		
		do {
			// edge case: value1 and value2 are the same or the value is in the list more than once
			if (current.Value.Equals(value1) || current.Value.Equals(value2))
			{
				if (setFoundNode()) break;
			}
			
			previous = current;
			current = current.Next;
		} while(current != null);
		
		// edge case: one of the found nodes is the first node
		
		if (object.ReferenceEquals(firstFoundNode, _head))
		{
			_head = secondFoundNode;
		}
		else if (object.ReferenceEquals(secondFoundNode, _head))
		{
			_head = firstFoundNode;
		}
		
		if (object.ReferenceEquals(firstFoundNode, _tail))
		{
			_tail = secondFoundNode;
		}
		else if (object.ReferenceEquals(secondFoundNode, _tail))
		{
			_tail = firstFoundNode;
		}

		if (firstNodesParent != null)
		{
			firstNodesParent.Next /*B.Next*/ = secondFoundNode; /*C*/
			/*
				  /--------v
			A -> B    D -> C -> (null)
			*/
		}

// second.Next (D) needs to point to first.Next (B) NOT (A)

		var secondNodeNextRef = secondFoundNode.Next; /*null*/
		secondFoundNode.Next /*null*/ = firstFoundNode.Next; /*D*/
		/*
		      /--------v
		A -> B    D -> C
		           ^--/
		*/

		firstFoundNode.Next /*C*/ = secondNodeNextRef; /*null*/
		/*
		          /---------\
		      /---|----v     v
		A -> B    D    C    (null)
		           ^--/
		*/
		
		// because the loop breaks above when both nodes are found,
		// `previous` will always be the parent of the second node found
		if (firstFoundNode.Next == null)
		{
			previous.Next = firstFoundNode;
		}
	}
	
	public void Reverse()
	{
		Node @ref = _head;

		while (@ref.Next != null)
		{
			//@ref = _head;

			// [A] -> B -> C -> D

			// point next node.next to head (and to @ref)
			// [A] <-> B -> C -> D
			var danglingRef = @ref.Next.Next; // C -> D
			@ref.Next.Next = _head;

			// point head to new head
			_head = @ref.Next;


			// point current.Next to head of dangling
			@ref.Next = danglingRef;
			/*
			[A] <- B    C -> D
			 \----------^
			*/
		}
	}
}




















