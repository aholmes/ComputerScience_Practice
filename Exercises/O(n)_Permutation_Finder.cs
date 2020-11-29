#region Usings
System.Collections.Generic
#endregion

void Main()
{
    var smallStr = "ABC";
    var bigStr = "ABCdefghijklmnopBCBAqrsBACtuCABvwxyz";
    
    var indexes = new List<int>();
    
    for(var i = 0; i < bigStr.Length; i++)
    {
        if (i + smallStr.Length > bigStr.Length) break;
        
        var s = bigStr.Substring(i, smallStr.Length);

        bool exists = false;
        for(var j = 0; j < smallStr.Length; j++)
        {
            var position = s.IndexOf(smallStr[j]);
            if (position == -1)
            {
                exists = false;
                break;
            }
            
            s = s.Remove(position, 1);
            exists = true;
        }

        if (exists) indexes.Add(i);
    }
    
    // LINQPad method
    indexes.Dump();
}
