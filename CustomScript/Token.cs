[System.Serializable]
public struct Token
{
    public TT Type;
    public string Value;
    public int Line;
    public int Col;

    public Token(TT t, string v, int line = 0, int col = 0)
    {
        Type = t;
        Value = v;
        Line = line;
        Col = col;
    }

    public override string ToString() => $"[{Type}:{Value} @ {Line}:{Col}]";
}

