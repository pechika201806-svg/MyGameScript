using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public TurnManager turnManager;
    public TMP_InputField codeInput;
    public TMP_Text PlayerMessage;
    public Button runButton;
    public Button turnButton;
    bool insideForUpdate = false;
    public bool clear = false;

    int _callDepth = 0;



    readonly Dictionary<string, int> variables = new();
    readonly Dictionary<string, int[]> arrays = new();
    readonly Dictionary<string, int[,]> arrays2D = new();

    List<Token> Tokenize(string src)
    {
        var toks = new List<Token>();
        int i = 0;
        int line = 1;
        int col = 1;

        void AddToken(TT t, string v, int l, int c) => toks.Add(new Token(t, v, l, c));

        while (i < src.Length)
        {
            char c = src[i];

            if (char.IsWhiteSpace(c))
            {
                if (c == '\n') { line++; col = 1; i++; continue; }
                col++; i++; continue;
            }

            if (c == '/' && i + 1 < src.Length && src[i + 1] == '/')
            {
                while (i < src.Length && src[i] != '\n') { i++; col++; }
                continue;
            }

            if (c == '/' && i + 1 < src.Length && src[i + 1] == '*')
            {
                int startLine = line, startCol = col;
                i += 2; col += 2;
                bool closed = false;
                while (i < src.Length)
                {
                    if (src[i] == '\n') { line++; col = 1; i++; continue; }
                    if (src[i] == '*' && i + 1 < src.Length && src[i + 1] == '/')
                    {
                        i += 2; col += 2; closed = true; break;
                    }
                    i++; col++;
                }
                if (!closed)
                {
                    Debug.LogError($"未終了のブロックコメント at {startLine}:{startCol}");
                    PlayerMessage.text += $"\n未終了のブロックコメント at {startLine}:{startCol}";
                }

                continue;
            }

            if (char.IsDigit(c))
            {
                int s = i, l = line, cc = col;
                while (i < src.Length && char.IsDigit(src[i])) { i++; col++; }
                AddToken(TT.Number, src.Substring(s, i - s), l, cc);
                continue;
            }

            if (char.IsLetter(c) || c == '_')
            {
                int s = i, l = line, cc = col;
                while (i < src.Length && (char.IsLetterOrDigit(src[i]) || src[i] == '_')) { i++; col++; }
                string w = src.Substring(s, i - s);
                TT t = w switch
                {
                    "int" => TT.Int,
                    "if" => TT.If,
                    "else" => TT.Else,
                    "for" => TT.For,
                    "while" => TT.While,
                    "return" => TT.Return,
                    _ => TT.Ident,
                };
                AddToken(t, w, l, cc);
                continue;
            }

            if (c == '"')
            {
                int l = line, cc = col;
                i++; col++;
                var sb = new System.Text.StringBuilder();
                bool closed = false;
                while (i < src.Length)
                {
                    char ch = src[i];
                    if (ch == '\n')
                    {
                        Debug.LogError($"文字列リテラル内に改行があります at {line}:{col}");
                        PlayerMessage.text += $"\n文字列リテラル内に改行があります at {line}:{col}";
                        break;
                    }
                    if (ch == '\\' && i + 1 < src.Length)
                    {
                        char esc = src[i + 1];
                        switch (esc)
                        {
                            case 'n': sb.Append('\n'); break;
                            case 't': sb.Append('\t'); break;
                            case '\\': sb.Append('\\'); break;
                            case '"': sb.Append('"'); break;
                            default: sb.Append(esc); break;
                        }
                        i += 2; col += 2;
                        continue;
                    }
                    if (ch == '"') { i++; col++; closed = true; break; }
                    sb.Append(ch);
                    i++; col++;
                }
                if (!closed)
                {
                    Debug.LogError($"未終了の文字列リテラル at {l}:{cc}");
                    PlayerMessage.text += $"\n未終了の文字列リテラル at {l}:{cc}";
                }
                AddToken(TT.String, sb.ToString(), l, cc);
                continue;
            }

            int curLine = line, curCol = col;
            switch (c)
            {
                case '+':
                    if (i + 1 < src.Length && src[i + 1] == '+') { AddToken(TT.PlusPlus, "++", curLine, curCol); i += 2; col += 2; break; }
                    if (i + 1 < src.Length && src[i + 1] == '=') { AddToken(TT.PlusEq, "+=", curLine, curCol); i += 2; col += 2; break; }
                    AddToken(TT.Plus, "+", curLine, curCol); i++; col++; break;
                case '-':
                    if (i + 1 < src.Length && src[i + 1] == '-') { AddToken(TT.MinusMinus, "--", curLine, curCol); i += 2; col += 2; break; }
                    if (i + 1 < src.Length && src[i + 1] == '=') { AddToken(TT.MinusEq, "-=", curLine, curCol); i += 2; col += 2; break; }
                    AddToken(TT.Minus, "-", curLine, curCol); i++; col++; break;
                case '*':
                    if (i + 1 < src.Length && src[i + 1] == '=') { AddToken(TT.StarEq, "*=", curLine, curCol); i += 2; col += 2; break; }
                    AddToken(TT.Star, "*", curLine, curCol); i++; col++; break;
                case '/':
                    if (i + 1 < src.Length && src[i + 1] == '=') { AddToken(TT.SlashEq, "/=", curLine, curCol); i += 2; col += 2; break; }
                    AddToken(TT.Slash, "/", curLine, curCol); i++; col++; break;
                case '%': AddToken(TT.Percent, "%", curLine, curCol); i++; col++; break;
                case '=':
                    if (i + 1 < src.Length && src[i + 1] == '=') { AddToken(TT.EqEq, "==", curLine, curCol); i += 2; col += 2; break; }
                    AddToken(TT.Assign, "=", curLine, curCol); i++; col++; break;
                case '!':
                    if (i + 1 < src.Length && src[i + 1] == '=') { AddToken(TT.NotEq, "!=", curLine, curCol); i += 2; col += 2; break; }
                    AddToken(TT.Not, "!", curLine, curCol); i++; col++; break;

                case '<':
                    if (i + 1 < src.Length && src[i + 1] == '=') { AddToken(TT.LtEq, "<=", curLine, curCol); i += 2; col += 2; break; }
                    AddToken(TT.Lt, "<", curLine, curCol); i++; col++; break;
                case '>':
                    if (i + 1 < src.Length && src[i + 1] == '=') { AddToken(TT.GtEq, ">=", curLine, curCol); i += 2; col += 2; break; }
                    AddToken(TT.Gt, ">", curLine, curCol); i++; col++; break;
                case '&':
                    if (i + 1 < src.Length && src[i + 1] == '&') { AddToken(TT.AndAnd, "&&", curLine, curCol); i += 2; col += 2; break; }
                    Debug.LogWarning($"Unknown char: {c} at {curLine}:{curCol}");
                    PlayerMessage.text += $"\nUnknown char: {c} at {curLine}:{curCol}";
                    i++; col++; break;
                case '|':
                    if (i + 1 < src.Length && src[i + 1] == '|') { AddToken(TT.OrOr, "||", curLine, curCol); i += 2; col += 2; break; }
                    Debug.LogWarning($"Unknown char: {c} at {curLine}:{curCol}");
                    PlayerMessage.text += $"\nUnknown char: {c} at {curLine}:{curCol}";
                    i++; col++; break;
                case '(': AddToken(TT.LParen, "(", curLine, curCol); i++; col++; break;
                case ')': AddToken(TT.RParen, ")", curLine, curCol); i++; col++; break;
                case '{': AddToken(TT.LBrace, "{", curLine, curCol); i++; col++; break;
                case '}': AddToken(TT.RBrace, "}", curLine, curCol); i++; col++; break;
                case '[': AddToken(TT.LBracket, "[", curLine, curCol); i++; col++; break;
                case ']': AddToken(TT.RBracket, "]", curLine, curCol); i++; col++; break;
                case ';': AddToken(TT.Semi, ";", curLine, curCol); i++; col++; break;
                case ',': AddToken(TT.Comma, ",", curLine, curCol); i++; col++; break;
                default:
                    Debug.LogWarning($"Unknown char: {c} at {curLine}:{curCol}");
                    PlayerMessage.text += $"\nUnknown char: {c} at {curLine}:{curCol}";
                    i++; col++;
                    break;
            }
        }

        AddToken(TT.EOF, "", line, col);
        return toks;
    }



    List<Token> _t;
    int _p;

    Token Cur => _t[_p];
    bool Is(TT t) => Cur.Type == t;


    Token Eat()
    {
        Token cur = _t[_p];
        if (_p < _t.Count - 1) _p++;
        return cur;
    }

    Token Eat(TT t)
    {
        if (!Is(t))
        {
            var cur = Cur;
            Debug.LogError($"期待 {t}, 実際 {cur} at {cur.Line}:{cur.Col}");
            PlayerMessage.text += $"\n期待 {t}, 実際 {cur} at {cur.Line}:{cur.Col}";
        }
        return Eat();
    }

    bool Try(TT t)
    {
        if (Is(t)) { Eat(); return true; }
        return false;
    }

    public void OnRunButtonClicked()
    {
        runButton.interactable = false;
        codeInput.interactable = false;
        turnManager.StopTimer();
        try
        {
            RunCode();
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
        }
        StartCoroutine(EnableTurnButtonWhenQueueDone());
    }

    IEnumerator EnableTurnButtonWhenQueueDone()
    {
        yield return new WaitUntil(() => !MagicSystem.Instance.IsRunning);
        turnButton.interactable = true;
    }


    public void RunCode()
    {
        _callDepth = 0;

        variables.Clear();
        arrays.Clear();
        arrays2D.Clear();

        _t = Tokenize(codeInput.text);
        _p = 0;

        int safety = 0;
        try
        {
            while (!Is(TT.EOF))
            {
                if (safety++ > 100000)
                {
                    Debug.LogError("無限ループ検出");
                    PlayerMessage.text += "\n無限ループ検出";
                    return;
                }
                ParseStmt();
            }
        }

        catch (ReturnException)
        {
            Debug.LogError("関数の外で return は使えません");
            PlayerMessage.text += "\n関数の外で return は使えません";
        }

    }


    void ParseStmt()
    {
        if (Try(TT.Semi)) return;
        if (Is(TT.Int)) { ParseDecl(); return; }
        if (Is(TT.If)) { ParseIf(); return; }
        if (Is(TT.For)) { ParseFor(); return; }
        if (Is(TT.Return)) { ParseReturn(); return; }
        if (Is(TT.While))
        {
            Debug.LogError("while 文はサポートされていません");
            PlayerMessage.text += "\nwhile 文はサポートされていません";
            SkipStmt();
            return;
        }

        if (Is(TT.LBrace)) { ParseBlock(); return; }
        if (Is(TT.Ident))
        {
            ExecExprStmtCore();


            if (!insideForUpdate)
            {
                if (!Try(TT.Semi))
                {
                    Token cur = Cur;
                    Debug.LogError($"セミコロンが必要です — {cur.Line}行目 {cur.Col}列目付近 (実際のトークン: {cur.Type} '{cur.Value}')");
                    PlayerMessage.text += $"\nセミコロンが必要です — {cur.Line}行目 {cur.Col}列目付近 (実際のトークン: {cur.Type} '{cur.Value}')";
                    SkipUntil(TT.Semi);
                    if (Is(TT.Semi)) Eat(TT.Semi);
                    return;
                }
            }

            return;
        }
        Debug.LogWarning("不明トークン: " + Cur);
        Eat();
    }

    void ParseBlock()
    {
        Eat(TT.LBrace);
        while (!Is(TT.RBrace) && !Is(TT.EOF)) ParseStmt();
        Eat(TT.RBrace);
    }

    void SkipBlock()
    {
        Eat(TT.LBrace);
        int depth = 1;
        while (depth > 0 && !Is(TT.EOF))
        {
            if (Is(TT.LBrace)) { depth++; Eat(); }
            else if (Is(TT.RBrace)) { depth--; Eat(); }
            else Eat();
        }
    }

    void SkipStmt()
    {
        if (Is(TT.LBrace))
        {
            SkipBlock();
            return;
        }

        if (Is(TT.If))
        {
            Eat(TT.If);
            Eat(TT.LParen);
            SkipUntilRParen();
            Eat(TT.RParen);
            SkipStmt();

            while (Is(TT.Else))
            {
                Eat(TT.Else);
                if (Is(TT.If))
                {
                    Eat(TT.If);
                    Eat(TT.LParen);
                    SkipUntilRParen();
                    Eat(TT.RParen);
                    SkipStmt();
                }
                else
                {
                    SkipStmt();
                    break;
                }
            }
            return;
        }

        if (Is(TT.For))
        {
            Eat(TT.For);
            Eat(TT.LParen);
            SkipUntil(TT.Semi); Eat(TT.Semi);
            SkipUntil(TT.Semi); Eat(TT.Semi);
            SkipUntilRParen(); Eat(TT.RParen);
            SkipStmt();
            return;
        }

        if (Is(TT.While))
        {
            Eat(TT.While);
            Eat(TT.LParen);
            SkipUntilRParen();
            Eat(TT.RParen);
            SkipStmt();
            return;
        }

        SkipUntil(TT.Semi);
        if (Is(TT.Semi)) Eat(TT.Semi);
    }

    void ParseDecl()
    {
        Eat(TT.Int);

        ParseOneDecl();

        while (Is(TT.Comma))
        {
            Eat(TT.Comma);
            ParseOneDecl();
        }

        Eat(TT.Semi);
    }

    void ParseOneDecl()
    {

        string name = Eat(TT.Ident).Value;

        if (Is(TT.LBracket))
        {
            Eat();
            int s1 = EvalExpr();
            Eat(TT.RBracket);

            if (s1 <= 0)
            {
                Debug.LogError($"配列サイズが不正: {name}[{s1}]");
                PlayerMessage.text += $"\n配列サイズが不正: {name}[{s1}]";
                return;
            }

            if (Is(TT.LBracket))
            {
                Eat();
                int s2 = EvalExpr();
                Eat(TT.RBracket);

                if (s2 <= 0)
                {
                    Debug.LogError($"2D配列サイズが不正: {name}[{s1}][{s2}]");
                    PlayerMessage.text += $"\n2D配列サイズが不正: {name}[{s1}][{s2}]";
                    return;
                }

                if (arrays2D.ContainsKey(name))
                {
                    Debug.LogError("再宣言: " + name);
                    PlayerMessage.text += $"\n再宣言: " + name;
                    return;
                }
                arrays2D[name] = new int[s1, s2];
                Debug.Log($"2D配列宣言: {name}[{s1}][{s2}]");
                PlayerMessage.text += $"\n2D配列宣言: {name}[{s1}][{s2}]";
                if (Try(TT.Assign))
                {
                    if (!Try(TT.LBrace))
                    {
                        Debug.LogError("配列の初期化は初期化子{ }を使用します");
                        PlayerMessage.text += "\n配列の初期化は初期化子{ }を使用します";
                        SkipUntil(TT.Semi);
                        return;
                    }
                    if (Is(TT.RBrace))
                    {
                        Eat(TT.RBrace);
                        return;
                    }
                    int i = 0;
                    int ele;
                    while (!Is(TT.RBrace))
                    {
                        if (!Try(TT.LBrace))
                        {
                            Debug.LogError("配列の列ごとの初期化は初期化子{ }を使用します");
                            PlayerMessage.text += "\n配列の列ごとの初期化は初期化子{ }を使用します";
                            SkipUntil(TT.Semi);
                            return;
                        }
                        if (Is(TT.RBrace))
                        {
                            Eat(TT.RBrace);
                            continue;
                        }

                        if (i >= s1)
                        {
                            Debug.LogError($"配列{name}と初期化子の列数が異なります");
                            PlayerMessage.text += $"\n配列{name}と初期化子の列数が異なります";
                            SkipUntil(TT.Semi);
                            return;
                        }
                        int j = 0;
                        while (!Is(TT.RBrace))
                        {
                            if (j >= s2)
                            {
                                Debug.LogError($"配列{name}と初期化子の要素数が異なります");
                                PlayerMessage.text += $"\n配列{name}と初期化子の要素数が異なります";
                                SkipUntil(TT.Semi);
                                return;
                            }
                            ele = EvalExpr();
                            arrays2D[name][i, j++] = ele;
                            if (Is(TT.Comma))
                            {
                                Eat(TT.Comma);
                                if (Is(TT.RBrace))
                                {
                                    if (j <= s2)
                                        break;
                                    else
                                    {
                                        Debug.LogError($"配列{name}と初期化子の要素数が異なります");
                                        PlayerMessage.text += $"\n配列{name}と初期化子の要素数が異なります";
                                        SkipUntil(TT.Semi);
                                        return;
                                    }
                                }
                            }
                        }
                        Eat(TT.RBrace);
                        i++;
                        if (Is(TT.Comma))
                        {
                            Eat(TT.Comma);
                            if (Is(TT.RBrace))
                            {
                                Eat(TT.RBrace);
                                if (i <= s1) return;
                                else
                                {
                                    Debug.LogError($"配列{name}と初期化子の要素数が異なります");
                                    PlayerMessage.text += $"\n配列{name}と初期化子の要素数が異なります";
                                    SkipUntil(TT.Semi);
                                    return;
                                }
                            }
                        }

                    }
                    if (Is(TT.RBrace))
                    {
                        Eat(TT.RBrace);
                        if (i <= s1) return;
                        else
                        {
                            Debug.LogError($"配列{name}と初期化子の要素数が異なります");
                            PlayerMessage.text += $"\n配列{name}と初期化子の要素数が異なります";
                            SkipUntil(TT.Semi);
                            return;
                        }
                    }

                }

                return;
            }

            if (arrays.ContainsKey(name))
            {
                Debug.LogError("再宣言: " + name);
                PlayerMessage.text += $"\n再宣言: " + name;
                return;
            }
            arrays[name] = new int[s1];
            Debug.Log($"配列宣言: {name}[{s1}]");
            PlayerMessage.text += $"\n配列宣言: {name}[{s1}]";
            if (Try(TT.Assign))
            {
                if (!Try(TT.LBrace))
                {
                    Debug.LogError("配列の初期化は初期化子{ }を使用します");
                    PlayerMessage.text += "\n配列の初期化は初期化子{ }を使用します";
                    SkipUntil(TT.Semi);
                    return;
                }
                if (Is(TT.RBrace))
                {
                    Eat(TT.RBrace);
                    return;
                }
                int ele;
                int leng = 0;
                while (!Is(TT.RBrace))
                {
                    if (leng >= s1)
                    {
                        Debug.LogError($"配列{name}と初期化子の要素数が異なります");
                        PlayerMessage.text += $"\n配列{name}と初期化子の要素数が異なります";
                        SkipUntil(TT.Semi);
                        return;
                    }
                    ele = EvalExpr();
                    arrays[name][leng++] = ele;
                    if (Is(TT.Comma))
                    {
                        Eat();
                        if (Is(TT.RBrace))
                        {
                            Eat(TT.RBrace);
                            if (leng <= s1) return;
                            else
                            {
                                Debug.LogError($"配列{name}と初期化子の要素数が異なります");
                                PlayerMessage.text += $"\n配列{name}と初期化子の要素数が異なります";
                                SkipUntil(TT.Semi);
                                return;
                            }
                        }

                    }
                }
                Eat(TT.RBrace);
                if (leng <= s1) return;
                else
                {
                    Debug.LogError($"配列{name}と初期化子の要素数が異なります");
                    PlayerMessage.text += $"\n配列{name}と初期化子の要素数が異なります";
                    SkipUntil(TT.Semi);
                    return;
                }
            }
            return;
        }

        else
        {
            int val = Try(TT.Assign) ? EvalExpr() : 0;
            if (variables.ContainsKey(name))
            {
                Debug.LogError("再宣言: " + name);
                PlayerMessage.text += $"\n再宣言: " + name;
                return;
            }
            variables[name] = val;
            Debug.Log($"変数宣言: {name} = {val}");
            PlayerMessage.text += $"\n変数宣言: {name} = {val}";
        }
    }

    void ParseForInit()
    {
        if (Is(TT.Int))
        {
            Eat();
            string name = Eat(TT.Ident).Value;
            int val = Try(TT.Assign) ? EvalExpr() : 0;
            variables[name] = val;
        }
        else
        {
            ExecExprStmtCore();
        }
    }

    void ParseIf()
    {
        Eat(TT.If);
        Eat(TT.LParen);
        bool cond = EvalCond();
        Eat(TT.RParen);

        if (cond) ParseStmt(); else SkipStmt();
        bool done = cond;

        while (Is(TT.Else))
        {
            Eat();
            if (Is(TT.If))
            {
                Eat();
                Eat(TT.LParen);
                bool c2 = EvalCond();
                Eat(TT.RParen);
                if (!done && c2) { ParseStmt(); done = true; }
                else SkipStmt();
            }
            else
            {
                if (!done) ParseStmt(); else SkipStmt();
                break;
            }
        }
    }

    void ParseFor()
    {
        Eat(TT.For);
        Eat(TT.LParen);

        if (!Is(TT.Semi)) ParseForInit();
        Eat(TT.Semi);

        int condStart = _p;
        bool hasCond = !Is(TT.Semi);
        SkipUntil(TT.Semi);
        Eat(TT.Semi);

        int updateStart = _p;
        bool hasUpdate = !Is(TT.RParen);
        SkipUntilRParen();
        Eat(TT.RParen);

        int bodyStart = _p;
        SkipStmt();
        int afterFor = _p;

        int safety = 0;
        while (true)
        {
            if (safety++ > 100000)
            {
                Debug.LogError("for 無限ループ");
                PlayerMessage.text += "\nfor 無限ループ";
                break;
            }

            if (hasCond)
            {
                _p = condStart;
                if (!EvalCond()) break;
            }

            _p = bodyStart;
            ParseStmt();

            if (hasUpdate)
            {
                _p = updateStart;

                insideForUpdate = true;
                try { ExecForUpdate(); }
                finally { insideForUpdate = false; }
            }

        }

        _p = afterFor;
    }

    void ParseExprStmt()
    {
        ExecExprStmtCore();
        Eat(TT.Semi);
    }

    void ExecForUpdate()
    {
        while (!Is(TT.RParen) && !Is(TT.EOF))
        {
            ExecExprStmtCore();
            Try(TT.Semi);
            if (Is(TT.Comma)) Eat(TT.Comma);
            else break;
        }
    }

    void ParseReturn()
    {
        Eat(TT.Return);

        int val = 0;
        if (!Is(TT.Semi))
            val = EvalExpr();
        Eat(TT.Semi);

        throw new ReturnException(val);
    }

    void ExecExprStmtCore()
    {

        if (!Is(TT.Ident))
        {
            EvalExpr();
            return;
        }

        string name = Eat().Value;


        if (name == "magic" && Is(TT.LParen))
        {
            Eat();
            int id = EvalExpr();
            if (!Is(TT.RParen))
            {
                Debug.LogError("magic の引数リストが閉じられていません");
                PlayerMessage.text += "\nmagic の引数リストが閉じられていません";
                SkipUntilRParen();
                if (Is(TT.RParen)) Eat(TT.RParen);
                return;
            }
            Eat(TT.RParen);
            MagicSystem.Instance.Magic(id);
            return;
        }

        if (StatusManager.Instance.tutorialCount > 5)
        {
            if (name == "shot" && Is(TT.LParen))
            {
                Eat();
                int id = EvalExpr();
                Eat(TT.Comma);
                int i = EvalExpr();
                Eat(TT.Comma);
                int j = EvalExpr();
                if (!Is(TT.RParen))
                {
                    Debug.LogError("shot の引数リストが閉じられていません");
                    PlayerMessage.text += "\nshot の引数リストが閉じられていません";
                    SkipUntilRParen();
                    if (Is(TT.RParen)) Eat(TT.RParen);
                    return;
                }
                Eat(TT.RParen);
                MagicSystem.Instance.Shot(id, i, j);
                return;
            }
        }

        if (StatusManager.Instance.tutorialCount > 7)
        {
            if (name == "scope" && Is(TT.LParen))
            {
                Eat(TT.LParen);
                int i = EvalExpr();
                Eat(TT.Comma);
                int j = EvalExpr();
                if (!Is(TT.RParen))
                {
                    Debug.LogError("scope の引数リストが閉じられていません");
                    PlayerMessage.text += "\nscope の引数リストが閉じられていません";
                    SkipUntilRParen();
                    if (Is(TT.RParen)) Eat(TT.RParen);
                    return;
                }
                Eat(TT.RParen);

                MagicSystem.Instance.Scope(i, j);
                return;
            }
        }

        if (StatusManager.Instance.tutorialCount > 19)
        {
            if (name == "exmagic" && Is(TT.LParen))
            {
                Eat();
                int id = EvalExpr();
                Eat(TT.Comma);
                int i = EvalExpr();
                Eat(TT.Comma);
                int j = EvalExpr();
                if (!Is(TT.RParen))
                {
                    Debug.LogError("exmagic の引数リストが閉じられていません");
                    PlayerMessage.text += "\nexmagic の引数リストが閉じられていません";
                    SkipUntilRParen();
                    if (Is(TT.RParen)) Eat(TT.RParen);
                    return;
                }
                Eat(TT.RParen);
                MagicSystem.Instance.Exmagic(id, i, j);
                return;
            }
        }

        if (StatusManager.Instance.tutorialCount > 20)
        {
            if (name == "combo" && Is(TT.LParen))
            {
                Eat(TT.LParen);
                int id1 = EvalExpr();
                Eat(TT.Comma);
                int id2 = EvalExpr();
                if (!Is(TT.RParen))
                {
                    Debug.LogError("combo の引数リストが閉じられていません");
                    PlayerMessage.text += "\ncombo の引数リストが閉じられていません";
                    SkipUntilRParen();
                    if (Is(TT.RParen)) Eat(TT.RParen);
                    return;
                }
                Eat(TT.RParen);

                ComboSystem.Instance.Combo(id1, id2);
                return;
            }
        }

        

        if (Is(TT.LParen))
        {
            var func = StatusManager.Instance.userFunctions.Find(f => f.Name == name);
            if (func != null)
            {
                Eat(TT.LParen);
                var args = ParseArgList();
                CallUserFunc(func, args);
                return;
            }
            Debug.LogError($"未定義の関数: {name}");
            Eat(TT.LParen);
            SkipUntilRParen();
            if (Is(TT.RParen)) Eat(TT.RParen);
            return;
        }

        bool is1D = false, is2D = false;
        int idx1 = 0, idx2 = 0;

        if (Is(TT.LBracket))
        {
            Eat();
            idx1 = EvalExpr();
            Eat(TT.RBracket);
            is1D = true;

            if (Is(TT.LBracket))
            {
                Eat();
                idx2 = EvalExpr();
                Eat(TT.RBracket);
                is2D = true;
            }
        }

        if (Try(TT.PlusPlus))
        {
            if (is2D)
            {
                if (!arrays2D.ContainsKey(name))
                {
                    Debug.LogError("未宣言 2D 配列: " + name);
                    PlayerMessage.text += $"\n未宣言 2D 配列: " + name;
                    return;
                }
                var arr2 = arrays2D[name];
                if (idx1 < 0 || idx1 >= arr2.GetLength(0) || idx2 < 0 || idx2 >= arr2.GetLength(1))
                {
                    Debug.LogError($"2D配列範囲外: {name}[{idx1}][{idx2}]");
                    PlayerMessage.text += $"\n2D配列範囲外: {name}[{idx1}][{idx2}]";
                    return;
                }
                arr2[idx1, idx2]++;
                return;
            }
            else if (is1D)
            {
                if (!arrays.ContainsKey(name))
                {
                    Debug.LogError("未宣言配列: " + name);
                    PlayerMessage.text += $"\n未宣言配列: " + name;
                    return;
                }
                var arr1 = arrays[name];
                if (idx1 < 0 || idx1 >= arr1.Length)
                {
                    Debug.LogError($"配列範囲外: {name}[{idx1}]");
                    PlayerMessage.text += $"\n配列範囲外: {name}[{idx1}]";
                    return;
                }
                arr1[idx1]++;
                return;
            }
            else
            {
                if (!variables.ContainsKey(name))
                {
                    Debug.LogError("未宣言変数: " + name);
                    PlayerMessage.text += $"\n未宣言変数: " + name;
                    return;
                }
                variables[name]++;
                return;
            }
        }
        if (Try(TT.MinusMinus))
        {
            if (is2D)
            {
                if (!arrays2D.ContainsKey(name))
                {
                    Debug.LogError("未宣言 2D 配列: " + name);
                    PlayerMessage.text += $"\n未宣言 2D 配列: " + name;
                    return;
                }
                var arr2 = arrays2D[name];
                if (idx1 < 0 || idx1 >= arr2.GetLength(0) || idx2 < 0 || idx2 >= arr2.GetLength(1))
                {
                    Debug.LogError($"2D配列範囲外: {name}[{idx1}][{idx2}]");
                    PlayerMessage.text += $"\n2D配列範囲外: {name}[{idx1}][{idx2}]";
                    return;
                }
                arr2[idx1, idx2]--;
                return;
            }
            else if (is1D)
            {
                if (!arrays.ContainsKey(name))
                {
                    Debug.LogError("未宣言配列: " + name);
                    PlayerMessage.text += $"\n未宣言配列: " + name;
                    return;
                }
                var arr1 = arrays[name];
                if (idx1 < 0 || idx1 >= arr1.Length)
                {
                    Debug.LogError($"配列範囲外: {name}[{idx1}]");
                    PlayerMessage.text += $"\n配列範囲外: {name}[{idx1}]";
                    return;
                }
                arr1[idx1]--;
                return;
            }
            else
            {
                if (!variables.ContainsKey(name))
                {
                    Debug.LogError("未宣言変数: " + name);
                    PlayerMessage.text += $"\n未宣言変数: " + name;
                    return;
                }
                variables[name]--;
                return;
            }
        }




        TT op = Cur.Type;
        if (op == TT.Assign || op == TT.PlusEq || op == TT.MinusEq ||
            op == TT.StarEq || op == TT.SlashEq)
        {
            Eat();


            if (op == TT.Assign)
            {
                if (Is(TT.LBrace))
                {
                    Debug.LogError("配列の代入に初期化子{ }は使用できません");
                    PlayerMessage.text += "\n配列の代入に初期化子{ }は使用できません";
                    SkipUntilRbrace();
                    if (Is(TT.RBrace)) Eat(TT.RBrace);
                    return;
                }
            }

            int rhs = EvalExpr();

            if (is2D)
            {
                if (!arrays2D.ContainsKey(name))
                {
                    Debug.LogError("未宣言 2D 配列: " + name);
                    PlayerMessage.text += $"\n未宣言 2D 配列: " + name;
                    return;
                }
                var arr2 = arrays2D[name];
                if (idx1 < 0 || idx1 >= arr2.GetLength(0) || idx2 < 0 || idx2 >= arr2.GetLength(1))
                {
                    Debug.LogError($"2D配列範囲外: {name}[{idx1}][{idx2}]");
                    PlayerMessage.text += $"\n2D配列範囲外: {name}[{idx1}][{idx2}]";
                    return;
                }
                arr2[idx1, idx2] = CalcAssign(op, arr2[idx1, idx2], rhs);
            }
            else if (is1D)
            {
                if (!arrays.ContainsKey(name))
                {
                    Debug.LogError("未宣言配列: " + name);
                    PlayerMessage.text += $"\n未宣言配列: " + name;
                    return;
                }
                var arr1 = arrays[name];
                if (idx1 < 0 || idx1 >= arr1.Length)
                {
                    Debug.LogError($"配列範囲外: {name}[{idx1}]");
                    PlayerMessage.text += $"\n配列範囲外: {name}[{idx1}]";
                    return;
                }
                arr1[idx1] = CalcAssign(op, arr1[idx1], rhs);
            }
            else
            {
                if (!variables.ContainsKey(name))
                {
                    Debug.LogError("未宣言変数: " + name);
                    PlayerMessage.text += $"\n未宣言変数: " + name;
                    return;
                }
                variables[name] = CalcAssign(op, variables[name], rhs);
            }

        }
        else
        {

        }
    }


    int CalcAssign(TT op, int cur, int rhs)
    {
        switch (op)
        {
            case TT.PlusEq: return cur + rhs;
            case TT.MinusEq: return cur - rhs;
            case TT.StarEq: return cur * rhs;
            case TT.SlashEq:
                if (rhs == 0)
                {
                    Debug.LogError("0除算");
                    PlayerMessage.text += "\n0除算";
                    return cur;
                }
                return cur / rhs;
            default: return rhs; // Assign
        }
    }


    bool EvalCond()
    {
        bool lhsBool = EvalSingleCond();

        while (Cur.Type == TT.AndAnd || Cur.Type == TT.OrOr)
        {
            TT logic = Cur.Type;
            Eat();

            if (logic == TT.AndAnd)
            {
                if (!lhsBool)
                {
                    SkipExprTokens();
                    lhsBool = false;
                    continue;
                }
                bool rhsBool = EvalSingleCond();
                lhsBool = lhsBool && rhsBool;
            }
            else
            {
                if (lhsBool)
                {
                    SkipExprTokens();
                    lhsBool = true;
                    continue;
                }
                bool rhsBool = EvalSingleCond();
                lhsBool = lhsBool || rhsBool;
            }
        }

        return lhsBool;
    }

    bool EvalSingleCond()
    {
        int lhs = EvalExpr();
        TT op = Cur.Type;
        if (op != TT.EqEq && op != TT.NotEq && op != TT.Lt &&
            op != TT.Gt && op != TT.LtEq && op != TT.GtEq)
            return lhs != 0;
        Eat();
        int rhs = EvalExpr();
        return op switch
        {
            TT.EqEq => lhs == rhs,
            TT.NotEq => lhs != rhs,
            TT.Lt => lhs < rhs,
            TT.Gt => lhs > rhs,
            TT.LtEq => lhs <= rhs,
            TT.GtEq => lhs >= rhs,
            _ => false,
        };
    }

    void SkipExprTokens()
    {
        int parenDepth = 0;
        int bracketDepth = 0;

        while (!Is(TT.EOF))
        {
            if (Is(TT.LParen))
            {
                parenDepth++;
                Eat();
            }
            else if (Is(TT.RParen))
            {
                if (parenDepth == 0) break;
                parenDepth--;
                Eat();
            }
            else if (Is(TT.LBracket))
            {
                bracketDepth++;
                Eat();
            }
            else if (Is(TT.RBracket))
            {
                if (bracketDepth == 0) break;
                bracketDepth--;
                Eat();
            }
            else
            {
                if (parenDepth == 0 && bracketDepth == 0 &&
                    (Is(TT.Semi) || Is(TT.Comma) || Is(TT.AndAnd) || Is(TT.OrOr)))
                {
                    break;
                }
                Eat();
            }
        }
    }


    int EvalExpr()
    {
        int lhs = EvalCompare();

        while (true)
        {
            TT op = Cur.Type;

            if (op == TT.Plus || op == TT.Minus)
            {
                Eat();
                int rhs = EvalCompare();

                lhs = (op == TT.Plus) ? lhs + rhs : lhs - rhs;
            }
            else break;
        }

        return lhs;
    }

    int EvalCompare()
    {
        int lhs = EvalTerm();

        while (true)
        {
            TT op = Cur.Type;

            if (op == TT.EqEq || op == TT.NotEq ||
                op == TT.Lt || op == TT.Gt ||
                op == TT.LtEq || op == TT.GtEq)
            {
                Eat();
                int rhs = EvalTerm();

                lhs = op switch
                {
                    TT.EqEq => (lhs == rhs) ? 1 : 0,
                    TT.NotEq => (lhs != rhs) ? 1 : 0,
                    TT.Lt => (lhs < rhs) ? 1 : 0,
                    TT.Gt => (lhs > rhs) ? 1 : 0,
                    TT.LtEq => (lhs <= rhs) ? 1 : 0,
                    TT.GtEq => (lhs >= rhs) ? 1 : 0,
                    _ => lhs
                };
            }
            else break;
        }

        return lhs;
    }


    int EvalTerm()
    {
        int val = EvalUnary();
        while (Is(TT.Star) || Is(TT.Slash) || Is(TT.Percent))
        {
            TT op = Cur.Type;
            Eat();
            int r = EvalUnary();
            if (op == TT.Star) val *= r;
            else if (op == TT.Percent) val %= r;
            else
            {
                if (r == 0)
                {
                    Debug.LogError("0除算");
                    PlayerMessage.text += "\n0除算";
                    return 0;
                }
                val /= r;
            }
        }
        return val;
    }

    int EvalUnary()
    {
        if (Try(TT.Plus))
            return EvalUnary();
        if (Try(TT.Minus))
            return -EvalUnary();
        if (Try(TT.Not))
        {
            int v = EvalUnary();
            return (v == 0) ? 1 : 0;
        }
        return EvalPrimary();
    }

    int EvalPrimary()
    {
        if (Try(TT.LParen))
        {
            int v = EvalExpr();
            Eat(TT.RParen);
            return v;
        }

        if (Is(TT.Number))
            return int.Parse(Eat().Value);

        if (Is(TT.Ident))
        {
            string name = Eat().Value;

            if (StatusManager.Instance.tutorialCount > 7)
            {
                if (name == "scope" && Is(TT.LParen))
                {
                    Eat(TT.LParen);
                    int i = EvalExpr();
                    Eat(TT.Comma);
                    int j = EvalExpr();
                    if (!Is(TT.RParen))
                    {
                        Debug.LogError("scope の引数リストが閉じられていません");
                        PlayerMessage.text += "\nscope の引数リストが閉じられていません";
                        SkipUntilRParen();
                        if (Is(TT.RParen)) Eat(TT.RParen);
                        return 0;
                    }
                    Eat(TT.RParen);

                    return MagicSystem.Instance.Scope(i, j);
                }
            }

            if (Is(TT.LBracket))
            {
                Eat();
                int i1 = EvalExpr();
                Eat(TT.RBracket);

                if (Is(TT.LBracket))
                {
                    Eat();
                    int i2 = EvalExpr();
                    Eat(TT.RBracket);

                    if (!arrays2D.ContainsKey(name))
                    {
                        Debug.LogError("未宣言 2D 配列: " + name);
                        PlayerMessage.text += $"\n未宣言 2D 配列: " + name;
                        return 0;
                    }
                    var arr2 = arrays2D[name];

                    if (i1 < 0 || i1 >= arr2.GetLength(0) ||
                        i2 < 0 || i2 >= arr2.GetLength(1))
                    {
                        Debug.LogError($"2D配列範囲外: {name}[{i1}][{i2}]");
                        PlayerMessage.text += $"\n2D配列範囲外: {name}[{i1}][{i2}]";
                        return 0;
                    }

                    return arr2[i1, i2];
                }

                if (!arrays.ContainsKey(name))
                {
                    Debug.LogError("未宣言配列: " + name);
                    PlayerMessage.text += $"\n未宣言配列: " + name;
                    return 0;
                }
                var arr1 = arrays[name];

                if (i1 < 0 || i1 >= arr1.Length)
                {
                    Debug.LogError($"配列範囲外: {name}[{i1}]");
                    PlayerMessage.text += $"\n配列範囲外: {name}[{i1}]";
                    return 0;
                }
                return arr1[i1];
            }

            if (Is(TT.LParen))
            {
                var func = StatusManager.Instance.userFunctions.Find(f => f.Name == name);
                if (func != null)
                {
                    Eat(TT.LParen);
                    var args = ParseArgList();
                    return CallUserFunc(func, args);
                }
                Debug.LogError($"未定義の関数: {name}");
                Eat(TT.LParen);
                SkipUntilRParen();
                if (Is(TT.RParen)) Eat(TT.RParen);
                return 0;
            }

            if (!variables.ContainsKey(name))
            {
                Debug.LogError("未宣言変数: " + name);
                PlayerMessage.text += $"\n未宣言変数: " + name;
                return 0;
            }

            return variables[name];
        }

        Debug.LogError("式解析エラー: " + Cur);
        PlayerMessage.text += $"\n式解析エラー: " + Cur;
        if (!Is(TT.EOF)) Eat();
        return 0;
    }





    void SkipUntil(TT t)
    {
        while (!Is(t) && !Is(TT.EOF)) Eat();
    }

    void SkipUntilRParen()
    {
        int depth = 0;
        while (!Is(TT.EOF))
        {
            if (Is(TT.LParen))
            {
                depth++;
                Eat();
            }
            else if (Is(TT.RParen))
            {
                if (depth == 0) break;
                depth--;
                Eat();
            }
            else
            {
                Eat();
            }
        }
    }

    void SkipUntilRbrace()
    {
        int depth = 0;
        while (!Is(TT.EOF))
        {
            if (Is(TT.LBrace))
            {
                depth++;
                Eat();
            }
            else if (Is(TT.RBrace))
            {
                if (depth == 0) break;
                depth--;
                Eat();
            }
            else
            {
                Eat();
            }
        }
    }

    int CallUserFunc(Func func, List<int> args)
    {
        if (_callDepth > 200)
        {
            Debug.LogError($"スタックオーバーフロー: 関数 {func.Name} の再帰が深すぎます");
            PlayerMessage.text += $"\nスタックオーバーフロー: 関数 {func.Name} の再帰が深すぎます";
            return 0;
        }

        if (args.Count != func.ArgNames.Count)
        {
            Debug.LogError($"関数 {func.Name}: 引数の数が違います (期待 {func.ArgNames.Count}, 実際 {args.Count})");
            PlayerMessage.text += $"\n関数 {func.Name}: 引数の数が違います (期待 {func.ArgNames.Count}, 実際 {args.Count})";
            return 0;
        }

        _callDepth++;

        var savedVars = new Dictionary<string, int>(variables);
        var savedArrays = new Dictionary<string, int[]>(arrays);
        var savedArrays2D = new Dictionary<string, int[,]>(arrays2D);
        for (int i = 0; i < func.ArgNames.Count; i++)
            variables[func.ArgNames[i]] = args[i];

        var savedTokens = _t;
        int savedPos = _p;
        _t = func.BodyTokens;
        _p = 0;

        int returnVal = 0;
        int safety = 0;

        try
        {
            while (!Is(TT.EOF))
            {
                if (safety++ > 100000) 
                {
                    Debug.LogError($"関数 {func.Name} で無限ループ検出");
                    PlayerMessage.text += $"\n関数 {func.Name} で無限ループ検出";
                    break;
                }

                ParseStmt();
            }
        }
        catch (ReturnException e)
        {
            returnVal = e.Value;
        }
        finally
        {
            _callDepth--;
            _t = savedTokens;
            _p = savedPos;
            variables.Clear();
            foreach (var kv in savedVars) variables[kv.Key] = kv.Value;
            arrays.Clear();
            foreach (var kv in savedArrays) arrays[kv.Key] = kv.Value;
            arrays2D.Clear();
            foreach (var kv in savedArrays2D) arrays2D[kv.Key] = kv.Value;
        }

        return returnVal;
    }

    List<int> ParseArgList()
    {
        var args = new List<int>();
        if (!Is(TT.RParen))
        {
            args.Add(EvalExpr());
            while (Try(TT.Comma))
                args.Add(EvalExpr());
        }
        Eat(TT.RParen);
        return args;
    }

}

public class ReturnException : System.Exception
{
    public readonly int Value;
    public ReturnException(int value) { Value = value; }
}
