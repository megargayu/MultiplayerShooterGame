using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "Username Input Field Validator", menuName = "Username Input Field Validator")]
public class UsernameInputFieldValidator : TMP_InputValidator
{
    public override char Validate(ref string text, ref int pos, char ch)
    {
        if (ch == ' ')
        {
            ch = '_';
        }

        if ((char.IsLetterOrDigit(ch) || (ch == '_' && pos != 0 && text.ToCharArray()[pos - 1] != '_')) && text.Length < 12)
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