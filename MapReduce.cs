void Main()
{
	var data = new[]
	{
		"the","banana","orange","car","moose","cat","dog","the","jingle","zipper","zoom"
	};

	// 3:["the","dog","cat","the"] ...
	// Group strings by their length
	var mappedData = Map(data, (str) => new KeyValuePair<int, string>(str.Length, str));

	// 3:["the","dog","cat","the"] ...
	//   3 -> 4
	// Count the number of strings in each "length" group
	var reducedData = Reduce(mappedData, (kvp) => kvp.Value.Count);

	mappedData.Dump();
	reducedData.Dump();
}

Dictionary<K, V> Reduce<T,K,V>(Dictionary<K,T> data, Func<KeyValuePair<K,T>, V> func)
{
	var map = new Dictionary<K, V>();
	foreach (var item in data)
	{
		var value = func(item);
		map.Add(item.Key, value);
	}
	return map;
}

Dictionary<K, List<T>> Map<T,K>(IEnumerable<T> data, Func<T, KeyValuePair<K,T>> func)
{
	var map = new Dictionary<K,List<T>>();
	
	foreach(var item in data)
	{
		var itemMap = func(item);
		if (map.ContainsKey(itemMap.Key))
		{
			map[itemMap.Key].Add(itemMap.Value);
		}
		else
		{
			map[itemMap.Key] = new List<T> { itemMap.Value };
		}
	}

	return map;
}
