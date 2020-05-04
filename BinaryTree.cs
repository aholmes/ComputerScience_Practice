<Query Kind="Program" />

void Main()
{
	var data = new[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G'};//, 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
	//GetTree(data).Dump();
	var tree = GetTree(data);
	InOrderTraversal(tree, node => { Console.Write(node.Value); });
	Console.WriteLine();
	PreOrderTraversal(tree, node => { Console.Write(node.Value); });
	Console.WriteLine();
	PostOrderTraversal(tree, node => { Console.Write(node.Value); });
	Console.WriteLine();
}

public BinaryTreeNode GetTree(char[] values)
{
	BinaryTreeNode insertNode(int i)
	{
		if (i >= values.Length) return null;

		/*
		i = 0
		root:
		  A
		 / \
		 
			B.Left: insertNode(3); insertNode(4);
			  B
			 / \
			
			B.Right:
			  C
			 / \
		*/

		return new BinaryTreeNode
		{
			Value = values[i],
			Left = insertNode(2 * i + 1),
			Right = insertNode(2 * i + 2)
		};
	}

	return insertNode(0);
}

// In a binary search tree, this visits nodes in ascending order
public void InOrderTraversal(BinaryTreeNode node, Action<BinaryTreeNode> visit)
{
	if (node == null) return;
	InOrderTraversal(node.Left, visit);
	visit(node);
	InOrderTraversal(node.Right, visit);
}

// Visits the root node first
public void PreOrderTraversal(BinaryTreeNode node, Action<BinaryTreeNode> visit)
{
	if (node == null) return;
	visit(node);
	PreOrderTraversal(node.Left, visit);
	PreOrderTraversal(node.Right, visit);
}

// Visits the root node last
public void PostOrderTraversal(BinaryTreeNode node, Action<BinaryTreeNode> visit)
{
	if (node == null) return;
	PostOrderTraversal(node.Left, visit);
	PostOrderTraversal(node.Right, visit);
	visit(node);
}

public class BinaryTreeNode
{
	public char Value;
	public BinaryTreeNode Left {get;set;}
	public BinaryTreeNode Right {get;set;}
}