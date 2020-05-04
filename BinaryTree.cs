<Query Kind="Program" />

void Main()
{
	// These are all ordered to A - G (ascending)
	var tree = GetTree(new[] { 'D', 'B', 'F', 'A', 'C', 'E', 'G'});
	InOrderTraversal(tree, node => { Console.Write(node.Value); });
	Console.WriteLine();
	
	tree = GetTree(new[] { 'A', 'B', 'E', 'C', 'D', 'F', 'G' });
	PreOrderTraversal(tree, node => { Console.Write(node.Value); });
	Console.WriteLine();
	
	tree = GetTree(new[] { 'G', 'C', 'F', 'A', 'B', 'D', 'E' });
	tree.Dump();
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
/*
ex:

     D
   /   \
  B     F
 / \   / \
A   C E   G

A B C D E F G
*/
public void InOrderTraversal(BinaryTreeNode node, Action<BinaryTreeNode> visit)
{
	if (node == null) return;
	InOrderTraversal(node.Left, visit);
	visit(node);
	InOrderTraversal(node.Right, visit);
}

// Visits the root node first
/*
ex:

     A
   /   \
  B     E
 / \   / \
C   D F   G

A B C D E F G
*/
public void PreOrderTraversal(BinaryTreeNode node, Action<BinaryTreeNode> visit)
{
	if (node == null) return;
	visit(node);
	PreOrderTraversal(node.Left, visit);
	PreOrderTraversal(node.Right, visit);
}

// Visits the root node last
/*
ex:

     G
   /   \
  C     F
 / \   / \
A   B D   E

A B C D E F G
*/
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