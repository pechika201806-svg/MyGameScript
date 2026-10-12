using System.Collections.Generic;


[System.Serializable]
public class Func
{
    public string ReturnType;
    public string Name;

    public string Code;
    public List<string> ArgTypes = new();
    public List<string> ArgNames = new();

    public int BodyStartIndex;
    public int BodyEndIndex;
    public List<Token> BodyTokens = new();
}

