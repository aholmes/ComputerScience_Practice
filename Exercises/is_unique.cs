<Query Kind="Program" />

// determine if a string has all unique characters

// 1. get the nth character
// 2. scan from position N through string length
//   2a. check if any characters match and return false if so
// 3. scan from position N+(iteration) through string length
//   3a. repeat 2a, then 3
// 4. When position is string length -1, return true

void Main()
{
	(string value, bool truth)[] testData = new[]
	{
		("", true),
		("z", true),
		("zz", false),
		("zzz", false),
		("abc", true),
		("aac", false),
		("abcdefghijklmnopqrstuvwxyz", true),
		("zbcdefghijklmnopqrstuvwxyz", false),
		("abcdefghijklmnopqrstuvwxzz", false),
		("abcdefghijklmaopqrstuvwxyz", false),
		("abcdefghijklmnopqrstuvdxyz", false),
	};
	
	foreach(var test in testData)
	{
		$"'{test.value}' == {test.truth}: {StringHasOnlyUniqueCharacters(test.value)}".Dump();
	}
}

bool StringHasOnlyUniqueCharacters(string value)
{
	for (var position = 0; position < value.Length - 1; position++)
	{
		var c = value[position];
		for (var i = position + 1; i < value.Length; i++)
		{
			
			if (c == value[i]) return false;
		}
	}
	
	return true;
}