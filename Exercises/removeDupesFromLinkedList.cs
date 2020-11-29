<Query Kind="Program" />

void Main()
{
	var ll = new LinkedList<int>();
	ll.Add(1);
	ll.Add(1);
	ll.Add(2);
	ll.Add(4);
	ll.Add(3);
	ll.Add(1);
	ll.Add(1);
	ll.Add(4);
	ll.Add(5);
	ll.Add(1);
	ll.Add(3);

	ll.RemoveDupes();

	ll.Dump();
}

public class LinkedList<T>
{
	public class Node
	{
		public T Value;
		public Node Next;
	}
	
	public Node _head;
	public Node _tail;
	
	public void Add(T value)
	{
		if (_head == null)
		{
			_head = new Node { Value = value };
			_tail = _head;
			return;
		}
		
		_tail = _tail.Next = new Node { Value = value };
	}
	
	public void Delete(T value)
	{
		if (_head == null) return;

		var head = _head;
		if (head.Value.Equals(value))
		{
			_head = head.Next;
			return;
		}
		
		while(head.Next != null)
		{
			if (head.Next.Value.Equals(value))
			{
				head.Next = head.Next.Next;
				return;
			}
			head = head.Next;
		}
	}

	public Node Get(T value)
	{
		var head = _head;
		
		if (head.Value.Equals(value)) return head;
		
		while(head.Next != null)
		{
			if (head.Next.Value.Equals(value)) return head.Next;
			
			head = head.Next;
		}
		
		return null;
	}
	
	public Node GetHead()
	{
		return _head;
	}
	
	public void RemoveDupes()
	{
		var head = _head;
		
		// remove duplicates
		// 1. get first node
		// 2. check every node for that value
		// 3. if that node value exists elsewhere, remove it
		// 4. don't remove the value if it exists one time only
		while (head != null)
		{
			var headNext = head.Next;
			var prev = head;
			while (headNext != null)
			{
				if (head.Value.Equals(headNext.Value))
				{
					prev.Next = headNext.Next;
					headNext = prev.Next;
				}
				else
				{
					prev = headNext;
					headNext = headNext.Next;
				}
			}

			_tail = head;
			head = head.Next;
		}
	}

}