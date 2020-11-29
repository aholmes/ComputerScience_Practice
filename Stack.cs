<Query Kind="Program" />

void Main()
{
	var s = new Stack<int>();
	s.Push(1);
	s.Push(2);
	s.Push(3);
	s.Push(4);
	s.Push(5);
	
	while(!s.IsEmpty())
	{
		s.Pop().Dump();
	}
}

public class Stack<T>
{
	private class StackNode
	{
		public T Data;
		public StackNode Next;
		
		public StackNode(T data)
		{
			Data = data;
		}
	}

	private StackNode top;

	public void Push(T data)
	{
		var node = new StackNode(data);
		node.Next = top;
		top = node;

	}

	public T Pop()
	{
		if (IsEmpty()) throw new Exception("Stack is empty.");
		
		var data = top.Data;
		top = top.Next;
		return data;
	}

	public T Peek()
	{
		if (IsEmpty()) throw new Exception("Stack is empty.");
		
		return top.Data;
	}
	
	public bool IsEmpty()
	{
		return top == null;
	}
}