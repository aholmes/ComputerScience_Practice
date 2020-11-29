<Query Kind="Program" />

void Main()
{
	// setup
	var inbox = new[] {5,2,3,1,6,0};
	var inbox_sorted = inbox.Where(o => o != 0).OrderBy(o => o).ToArray();
	var grid = new int?[5,5];
	grid.Set(24, 0);
	
	// program
	var machine = new Machine(grid, inbox);
	
	try
	{
		machine.Exec();
	}
	catch (InvalidOperationException e) when (e.Message == "Inbox Empty.")
	{
	}

	// output
	Console.WriteLine(grid);
	Console.WriteLine(machine.outbox);
	Console.WriteLine(Enumerable.SequenceEqual(inbox_sorted, machine.outbox));
}

public class Machine
{
	private readonly int?[,] grid; // the grid
	private readonly Queue<int> inbox; // the inbox
	public readonly List<int> outbox = new List<int>();

	private int? item; // item person is carrying

	public Machine(int?[,] grid, int[] inbox)
	{
		this.grid = grid;
		this.inbox = new Queue<int>(inbox);
	}

	private void Add(int cellNumber) => item = item + grid.Get(cellNumber);
	private void AddIndirect(int cellNumber) => item = item + grid.GetIndirect(cellNumber);

	private void Sub(int cellNumber) => item = item - grid.Get(cellNumber);
	private void SubIndirect(int cellNumber) => item = item - grid.GetIndirect(cellNumber);

	private void BumpPlus(int cellNumber)
	{
		var value = grid.Get(cellNumber) + 1;
		grid.Set(cellNumber, value);
		item = value;
	}
	private void BumpPlusIndirect(int cellNumber)
	{
		var value = grid.GetIndirect(cellNumber) + 1;
		grid.SetIndirect(cellNumber, value);
		item = value;
	}

	private void BumpMinus(int cellNumber)
	{
		var value = grid.Get(cellNumber) - 1;
		grid.Set(cellNumber, value);
		item = value;
	}
	private void BumpMinusIndirect(int cellNumber)
	{
		var value = grid.GetIndirect(cellNumber) - 1;
		grid.SetIndirect(cellNumber, value);
		item = value;
	}

	private void CopyFrom(int cellNumber) => item = grid.Get(cellNumber);
	private void CopyFromIndirect(int cellNumber) => item = grid.GetIndirect(cellNumber);

	private void CopyTo(int cellNumber) => grid.Set(cellNumber, (int)item);
	private void CopyToIndirect(int cellNumber) => grid.SetIndirect(cellNumber, (int)item);

	private void Inbox()
	{
		try
		{
			item = inbox.Dequeue();
		}
		catch (InvalidOperationException e)
		{
			throw new InvalidOperationException("Inbox Empty.", e);
		}
	}
	private void Outbox()
	{
		outbox.Add((int)item);
		item = null;
	}

	public void Exec()
	{
		var counter = 24;
		var sequenceCounter = counter-1;
		var sequenceCounter2 = sequenceCounter-1;
		var placeholder = 20;

	#region Pull sequence from inbox
	PullSequenceFromInbox:
		Inbox();
		
		// JZ
		if (item == 0)
			goto Sort;

		CopyToIndirect(counter);
		BumpPlus(counter);
		CopyTo(sequenceCounter);

		goto PullSequenceFromInbox;
	#endregion
	
	#region Sort
	Sort:
		BumpMinus(sequenceCounter);
		
		// JZ
		if (item == 0)
			goto SendToOutbox;
		
		CopyTo(sequenceCounter2);
		BumpMinus(sequenceCounter2);
		
		CopyFromIndirect(sequenceCounter);
		SubIndirect(sequenceCounter2);
		
		// JN
		if (item < 0)
			goto Swap;
		
		goto SendToOutbox;
	#endregion
	
	#region Swap
	Swap:
		CopyFromIndirect(sequenceCounter);
		CopyTo(placeholder);
		CopyFromIndirect(sequenceCounter2);
		CopyToIndirect(sequenceCounter);
		CopyFrom(placeholder);
		CopyToIndirect(sequenceCounter2);
		
		//BumpMinus(sequenceCounter);
		
		goto Sort;
	#endregion

	#region Send to outbox
	SendToOutbox:
		BumpMinus(counter);
		// JN
		if (item < 0)
			goto PullSequenceFromInbox; // everything sent to the outbox, start over
		
		CopyFromIndirect(counter);
		Outbox();
		goto SendToOutbox;
	#endregion
	}
}

public static class IntExt
{
	public static int Get(this int?[,] grid, int cellNumber)
	{
		var (x, y) = (cellNumber / 5, cellNumber % 5);
		return (int)grid[x, y];
	}
	public static int GetIndirect(this int?[,] grid, int cellNumber) => grid.Get(grid.Get(cellNumber));

	public static void Set(this int?[,] grid, int cellNumber, int value)
	{
		var (x, y) = (cellNumber / 5, cellNumber % 5);
		grid[x, y] = value;
	}
	public static void SetIndirect(this int?[,] grid, int cellNumber, int value) => grid.Set(grid.Get(cellNumber), value);
}