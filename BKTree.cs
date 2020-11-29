/// <summary>
/// A BK Tree for searching closely matching strings within a specified tolerance.
/// </summary>
/// <remarks>
/// These articles were referenced to write the BK Tree below.
/// No code has been directly copied.
/// https://en.wikipedia.org/wiki/Levenshtein_distance
/// https://www.csharpstar.com/csharp-string-distance-algorithm
/// https://www.geeksforgeeks.org/bk-tree-introduction-implementation/
/// http://blog.notdot.net/2007/4/Damn-Cool-Algorithms-Part-1-BK-Trees
/// https://nullwords.wordpress.com/2013/03/13/the-bk-tree-a-data-structure-for-spell-checking/
/// </remarks>
public class BkTree
{
	private BkNode _root;

	/// <summary>
	/// Add a word to the tree's dictionary.
	/// </summary>
	/// <param name="value"></param>
	public void Add(string value)
	{
	    if(_root == null)
	    {
		_root = new BkNode(value);
		return;
	    }

	    int distance;
	    var currentNode = _root;
	    do
	    {
		distance = LevenshteinDistance(currentNode.Value, value);
		if(distance == 0) return;

		currentNode = currentNode[distance] ?? currentNode;
	    } while(currentNode.ContainsKey(distance));

	    currentNode.AddChild(distance, value);
	}

	/// <summary>
	/// Search for a word in the tree's dictionary.
	/// </summary>
	/// <param name="value"></param>
	/// <param name="distanceTolerance">The number of transformations that would occur to the `value` parameter to match a dictionary word.</param>
	/// <returns></returns>
	public List<string> Search(string value, int distanceTolerance)
	{
	    if(string.IsNullOrEmpty(value)) throw new ArgumentNullException(nameof(value));
	    if(_root == null) throw new InvalidOperationException("Cannot search when no words have been added to the BK Tree.");

	    static void search(BkNode node, List<string> result, string value, int distanceTolerance)
	    {
		var currentDistance = LevenshteinDistance(node.Value, value);

		var minDistance = (long)currentDistance - distanceTolerance;
		var maxDistance = (long)currentDistance + distanceTolerance;

		if(currentDistance <= distanceTolerance)
		    result.Add(node.Value);

		var edges = node.Edges.Where(edge => minDistance <= edge && edge <= maxDistance);
		foreach(var edge in edges)
		{
		    search(node[edge], result, value, distanceTolerance);
		}
	    }

	    var result = new List<string>();

	    search(_root, result, value, distanceTolerance);

	    return result;
	}

	/// <summary>
	/// Determine the number of insertions, deletions, and substitions that
	/// would occur to `source` to match `target`.
	/// </summary>
	/// <param name="source"></param>
	/// <param name="target"></param>
	/// <returns></returns>
	public static int LevenshteinDistance(string source, string target)
	{
	    if(source.Length == 0) return target.Length;
	    if(target.Length == 0) return source.Length;

	    var vector0 = Enumerable.Range(0, target.Length + 1).ToArray();
	    var vector1 = new int[target.Length + 1];
	    vector0[vector0.Length - 1] = 0;

	    for(var i = 0; i < source.Length; i++)
	    {
		vector1[0] = i + 1;

		for(var j = 0; j < target.Length; j++)
		{
		    vector1[j + 1] = new[]
		    {
			vector0[j + 1] + 1,
			vector1[j] + 1,
			source[i] == target[j] ? vector0[j] : vector0[j] + 1
		    }.Min();
		}

		var temp = vector0;
		vector0 = vector1;
		vector1 = temp;
	    }

	    return vector0[target.Length];
	}

	private class BkNode
	{
	    public readonly string Value;
	    private Dictionary<int, BkNode> Children = new Dictionary<int, BkNode>();

	    public BkNode(string value) => Value = value;
	    public BkNode this[int edge] => Children.TryGetValue(edge, out BkNode node) ? node : null;
	    public ICollection<int> Edges => Children.Keys;
	    public bool ContainsKey(int edge) => Children.ContainsKey(edge);
	    public void AddChild(int edge, string value) => Children[edge] = new BkNode(value);

	}
}
