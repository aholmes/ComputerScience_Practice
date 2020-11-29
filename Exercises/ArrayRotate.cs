<Query Kind="Program">
  <Namespace>System.CodeDom.Compiler</Namespace>
  <Namespace>System.ComponentModel</Namespace>
  <Namespace>System.Diagnostics.CodeAnalysis</Namespace>
  <Namespace>System.Globalization</Namespace>
  <Namespace>System.Runtime.Serialization</Namespace>
</Query>

void Main()
{
	var n = 5;
	var d = 4;
	var a = new[] {1,2,3,4,5};

	var result = Solution.rotLeft(a, d);

	result.Dump();
}

class Solution
{
    public static int[] rotLeft(int[] arr, int d)
	{
		var destArr = new int[arr.Length];
		for (var j = 0; j < arr.Length; j++)
		{
			int a;

			if (j-d < 0)
			{
				a = arr.Length - d + j;
			}
			else
			{
				a = j-d;
			}
			destArr[a] = arr[j];
		}

		return destArr;
	}
}