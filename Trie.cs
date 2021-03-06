void Main()
{
	Console.WriteLine("Running tests ...");
	RunTests();
	Console.WriteLine("Done!");
}

void RunTests()
{
	var words = new[]
	{
		"apple",
		"ant",
		"banana",
		"boingo",
		"fish",
		"frog",
		"dog",
		"donkey",
		"zebra"
	};

	{
		Console.Write("Exact match matches full word ... ");
		var trie = new Trie(words);
		foreach (var word in words)
		{
			Debug.Assert(trie.Search(word));
		}
		Console.WriteLine("Pass!");
	}
	{
		Console.Write("Partial match matches partial word ... ");
		var trie = new Trie(words);
		foreach (var word in words)
		{
			Debug.Assert(trie.Search(word[0..2], false));
		}
		Console.WriteLine("Pass!");
	}
	{
		Console.Write("Exact match does not match partial word ... ");
		var trie = new Trie(words);
		foreach (var word in words)
		{
			Debug.Assert(trie.Search(word[0..2]) == false);
		}
		Console.WriteLine("Pass!");
	}
	{
		Console.Write("Exact match does not match non-existent word ... ");
		var trie = new Trie(words);
		Debug.Assert(trie.Search("DOESNT EXIST") == false);
		Console.WriteLine("Pass!");
	}
	{
		Console.Write("Partial match does not match non-existent word ... ");
		var trie = new Trie(words);
		Debug.Assert(trie.Search("DOESNT EXIST", false) == false);
		Console.WriteLine("Pass!");
	}
}

public class Trie
{
	public Trie(IEnumerable<string> words)
	{
		foreach(var word in words)
		{
			this.Insert(word);
		}
	}

	private TrieNode _root = new TrieNode();

	public void Insert(string word)
	{
		var tree = _root;

		foreach(var character in word)
		{
			if (tree.Nodes.ContainsKey(character))
			{
				tree = tree.Nodes[character];
			}
			else
			{
				var newNode = new TrieNode();
				tree.Nodes.Add(character, newNode);
				tree = newNode;
			}
		}

		tree.EndOfWord = true;
	}

	public bool Search(string key, bool exact = true)
	{
		var tree = _root;

		foreach (var character in key)
		{
			if (tree.Nodes.TryGetValue(character, out var node) == false)
				return false;

			tree = node;
		}

		return (tree != null && (!exact || tree.EndOfWord));
	}

	private class TrieNode
	{
		public Dictionary<char, TrieNode> Nodes { get; } = new Dictionary<char, TrieNode>();
		public bool EndOfWord { get; set; }
	}
}
