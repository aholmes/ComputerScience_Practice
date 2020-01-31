void Main()
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

	var trie = new Trie(words);

	{
		foreach (var word in words)
		{
			Debug.Assert(trie.Search(word));
		}
	}
	{
		foreach (var word in words)
		{
			Debug.Assert(trie.Search(word[0..2], false));
		}
	}
	{
		foreach (var word in words)
		{
			Debug.Assert(trie.Search(word[0..2]) == false);
		}
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
			if (!tree.Nodes.ContainsKey(character))
			{
				var newNode = new TrieNode();
				tree.Nodes.Add(character, newNode);
				tree = newNode;
			}
			else
			{
				tree = tree.Nodes[character];
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
