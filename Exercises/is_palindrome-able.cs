<Query Kind="Program" />

// given a string, determine if it is a permutation of a palindrome

// -a palindrome repeats every character at least once, with a single pivot character (or no pivot character)
// -there must be even numbers of all characters except for the pivot character, which must be only appear an odd number of times (or 0 times)
// -if more than one letter appears and odd number of times, the word is not a palindrome
// -ignore whitespace, e.g. "taco cat" is "tacocat"

void Main()
{
	(string input, bool truth)[] testData = new[]
	{
		("", true),
		("z", true),
		("zz", true),
		("zzz", true),
		("abc", false),
		("abba", true),
		("taco cat", true),
		("palindrome", false)
	}.Select(o => (o.Item1.Scramble(), o.Item2)).ToArray();
	
	foreach(var test in testData)
	{
		$"'{test.input}' == {test.truth}: {StringCanBeAPalindrome(test.input)}".Dump();
	}
}

public bool StringCanBeAPalindrome(string input)
{
	input = input.Replace(" ", "");
	
	var characterCounts = new Dictionary<char, int>();
	
	foreach(var chr in input)
	{
		if (characterCounts.TryAdd(chr, 1) == false)
		{
			characterCounts[chr]++;
		}
	}
	
	var foundOddCount = false;
	foreach(var kvp in characterCounts)
	{
		if (kvp.Value % 2 != 0)
		{
			if (foundOddCount) return false;
			
			foundOddCount = true;
		}
	}
	
	return true;
}

static class StringExtensions
{
	public static string Scramble(this string str)
	{
		return new string(str.OrderBy(o => Guid.NewGuid()).ToArray());
	}
}