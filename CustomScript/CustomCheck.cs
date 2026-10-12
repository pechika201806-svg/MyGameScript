using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class CustomCheck : MonoBehaviour
{
    public TMP_InputField codeInput;
    public TMP_Text funcName;
    public TMP_Text PlayerMessage;
    public int customID;
    

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
                    "void" => TT.Void,
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
                    if (i+1 < src.Length && src[i+1] == '+') { AddToken(TT.PlusPlus,"++",curLine,curCol); i+=2; col+=2; break; }
                    if (i+1 < src.Length && src[i+1] == '=') { AddToken(TT.PlusEq,"+=",curLine,curCol); i+=2; col+=2; break; }
                    AddToken(TT.Plus,"+",curLine,curCol); i++; col++; break;
                case '-':
                    if (i+1 < src.Length && src[i+1] == '-') { AddToken(TT.MinusMinus,"--",curLine,curCol); i+=2; col+=2; break; }
                    if (i+1 < src.Length && src[i+1] == '=') { AddToken(TT.MinusEq,"-=",curLine,curCol); i+=2; col+=2; break; }
                    AddToken(TT.Minus,"-",curLine,curCol); i++; col++; break;
                case '*':
                    if (i+1 < src.Length && src[i+1] == '=') { AddToken(TT.StarEq,"*=",curLine,curCol); i+=2; col+=2; break; }
                    AddToken(TT.Star,"*",curLine,curCol); i++; col++; break;
                case '/':
                    if (i+1 < src.Length && src[i+1] == '=') { AddToken(TT.SlashEq,"/=",curLine,curCol); i+=2; col+=2; break; }
                    AddToken(TT.Slash,"/",curLine,curCol); i++; col++; break;
                case '%': AddToken(TT.Percent,"%",curLine,curCol); i++; col++; break;
                case '=':
                    if (i+1 < src.Length && src[i+1] == '=') { AddToken(TT.EqEq,"==",curLine,curCol); i+=2; col+=2; break; }
                    AddToken(TT.Assign,"=",curLine,curCol); i++; col++; break;
                case '!':
                    if (i+1 < src.Length && src[i+1] == '=') { AddToken(TT.NotEq,"!=",curLine,curCol); i+=2; col+=2; break; }
                    AddToken(TT.Not,"!",curLine,curCol); i++; col++; break;

                case '<':
                    if (i+1 < src.Length && src[i+1] == '=') { AddToken(TT.LtEq,"<=",curLine,curCol); i+=2; col+=2; break; }
                    AddToken(TT.Lt,"<",curLine,curCol); i++; col++; break;
                case '>':
                    if (i+1 < src.Length && src[i+1] == '=') { AddToken(TT.GtEq,">=",curLine,curCol); i+=2; col+=2; break; }
                    AddToken(TT.Gt,">",curLine,curCol); i++; col++; break;
                case '&':
                    if (i+1 < src.Length && src[i+1] == '&') { AddToken(TT.AndAnd,"&&",curLine,curCol); i+=2; col+=2; break; }
                    Debug.LogWarning($"Unknown char: {c} at {curLine}:{curCol}");
                    PlayerMessage.text += $"\nUnknown char: {c} at {curLine}:{curCol}";
                    i++; col++; break;
                case '|':
                    if (i+1 < src.Length && src[i+1] == '|') { AddToken(TT.OrOr,"||",curLine,curCol); i+=2; col+=2; break; }
                    Debug.LogWarning($"Unknown char: {c} at {curLine}:{curCol}");
                    PlayerMessage.text += $"\nUnknown char: {c} at {curLine}:{curCol}";
                    i++; col++; break;
                case '(' : AddToken(TT.LParen,"(",curLine,curCol); i++; col++; break;
                case ')' : AddToken(TT.RParen,")",curLine,curCol); i++; col++; break;
                case '{' : AddToken(TT.LBrace,"{",curLine,curCol); i++; col++; break;
                case '}' : AddToken(TT.RBrace,"}",curLine,curCol); i++; col++; break;
                case '[' : AddToken(TT.LBracket,"[",curLine,curCol); i++; col++; break;
                case ']' : AddToken(TT.RBracket,"]",curLine,curCol); i++; col++; break;
                case ';' : AddToken(TT.Semi,";",curLine,curCol); i++; col++; break;
                case ',' : AddToken(TT.Comma,",",curLine,curCol); i++; col++; break;
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

 

    Func ParseFunction(List<Token> toks)
    {

        int i = 0;

        while (i < toks.Count)
        {
            if (toks[i].Type == TT.Int ||
                (toks[i].Type == TT.Void))
            {
                break;
            }
            i++;
        }
        if (i >= toks.Count) return null;

        string returnType = toks[i].Value;
        i++;

        if (toks[i].Type != TT.Ident)
        {
            Debug.LogError("関数名がありません");
            return null;
        }

        var func = new Func();
        func.ReturnType = returnType;
        func.Name = toks[i].Value;
        

        i++;
        if (toks[i].Type != TT.LParen)
        {
            Debug.LogError("関数定義に '(' がありません");
            return null;
        }

        i++;

        if (toks[i].Type == TT.RParen)
        {

        }
        else
        {
            while (i < toks.Count)
            {
                if (toks[i].Type != TT.Int && toks[i].Type != TT.Ident)
                {
                    Debug.LogError($"型が必要です: {toks[i]}");
                    return null;
                }
                string argType = toks[i].Value;
                i++;

                if (toks[i].Type != TT.Ident)
                {
                    Debug.LogError($"引数名が必要です: {toks[i]}");
                    return null;
                }
                string argName = toks[i].Value;
                i++;

                func.ArgTypes.Add(argType);
                func.ArgNames.Add(argName);

                if (toks[i].Type == TT.Comma)
                {
                    i++;
                    continue;
                }

                if (toks[i].Type == TT.RParen)
                {
                    break;
                }

                Debug.LogError($"不正なトークン: {toks[i]}");
                return null;
            }
        }
        i++;
        if (toks[i].Type != TT.LBrace)
        {
            Debug.LogError("関数本体に '{' がありません");
            return null;
        }

        func.BodyStartIndex = i;
        i++;

        int braceDepth = 1;

        while (i < toks.Count && braceDepth > 0)
        {
            if (toks[i].Type == TT.LBrace)
                braceDepth++;
            else if (toks[i].Type == TT.RBrace)
                braceDepth--;

            if (braceDepth > 0)
                func.BodyTokens.Add(toks[i]);

            i++;
        }

        if (braceDepth != 0) return null;

        func.BodyEndIndex = i - 1;

        if (i < toks.Count - 1)
        {
            Debug.LogError("関数の外に余計なトークンがあります");
            return null;
        }

        if (toks[toks.Count - 1].Type == TT.EOF)
            func.BodyTokens.Add(toks[toks.Count - 1]);

        return func;
    }
    public void OnClickApply()
    {
        var toks = Tokenize(codeInput.text);
        var func = ParseFunction(toks);

        if (func == null)
        {
            PlayerMessage.text =
            $"関数型："
            + $"\n\n引数数："
            + $"\n\n本体トークン数：";
            PlayerMessage.text += "\n\n関数の解析に失敗しました";
            return;
        }

        func.Code = codeInput.text;
        funcName.text = $"{func.Name}";

        if (func.BodyTokens.Count-1 > 100)
        {
            PlayerMessage.text =
            $"関数型：{func.ReturnType}"
            + $"\n\n引数数：{func.ArgNames.Count}"
            + $"\n\n本体トークン数：{func.BodyTokens.Count - 1}/100";
            PlayerMessage.text += "\n\nトークン数エラー";
            return;
        }

        if (func.Name == "magic" || func.Name == "shot" || func.Name == "scope" || func.Name == "exmagic" || func.Name == "combo")
        {
            PlayerMessage.text =
            $"関数型：{func.ReturnType}"
            + $"\n\n引数数：{func.ArgNames.Count}"
            + $"\n\n本体トークン数：{func.BodyTokens.Count - 1}/100";
            PlayerMessage.text += "\n\n魔法関数と名前が重複しています";
            return;
        }



        var sm = StatusManager.Instance;

        int dupIndex = sm.userFunctions.FindIndex(f => f != null && f.Name == func.Name);
        if (dupIndex != -1 && dupIndex != customID)
        {
            PlayerMessage.text =
            $"関数型：{func.ReturnType}"
            + $"\n\n引数数：{func.ArgNames.Count}"
            + $"\n\n本体トークン数：{func.BodyTokens.Count - 1}/100";
            Debug.LogError($"関数名が重複しています: {func.Name} (customID {dupIndex} で使用中)");
            PlayerMessage.text += $"\n\n関数名が重複しています: {func.Name}";
            return;
        }

        while (sm.userFunctions.Count <= customID)
        {
            sm.userFunctions.Add(null);
        }

        sm.userFunctions[customID] = func;
        PlayerMessage.text =
            $"関数型：{func.ReturnType}"
            + $"\n\n引数数：{func.ArgNames.Count}"
            + $"\n\n本体トークン数：{func.BodyTokens.Count - 1}/100";

        PlayerMessage.text += $"\n\n関数 {func.Name} 適用完了";
    }

}


