using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "Join Match Input Field Validator", menuName = "Join Match Input Field Validator")]
public class JoinMatchInputValidator : TMP_InputValidator
{
    public override char Validate(ref string text, ref int pos, char ch)
    {
        if (text.Length < 5 && char.IsLetterOrDigit(ch))
        {
            ch = char.ToUpper(ch);
            text = text.Insert(pos, ch.ToString());
            pos++;
            return ch;
        }
        else
        {
            return '\0';
        }
    }
}