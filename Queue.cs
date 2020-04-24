<Query Kind="Program" />

void Main()
{
	var s = new Queue<int>();
	s.Add(1);
	s.Add(2);
	s.Add(3);
	s.Add(4);
	s.Add(5);

	while (!s.IsEmpty())
	{
		s.Remove().Dump();
	}
}

public class Queue<T>
{
	private class QueueNode
	{
		public T Data;
		public QueueNode Next;

		public QueueNode(T data)
		{
			Data = data;
		}
	}

	private QueueNode first;
	private QueueNode last;

	public void Add(T data)
	{
		var node = new QueueNode(data);
		if (last != null) last.Next = node;
		last = node;
		if (first == null) first = last;
	}
	
	public T Remove()
	{
		if (IsEmpty()) throw new Exception("Queue is empty.");

		var data = first.Data;
		first = first.Next;
		if (first == null) last = null;
		return data;
	}

	public T Peek()
	{
		if (IsEmpty()) throw new Exception("Queue is empty.");

		return first.Data;
	}

	public bool IsEmpty()
	{
		return first == null;
	}
}