using System;

/* if you've come here and are wondering "Why didn't you just use Char.GetNumericValue() method?" then let me tell you that,
* THAT function did not work for the love of god and always returned the value of -1, so I just got fed up and created my own class
*/

public enum CharList
{
    q, w, e, r, t, y, u, i, o, p, a, s, d, f, g, h, j, k, l, z, x, c, v, b, n, m
}

public class CharValues
{
    public static ushort GetValue(char key)
    {
        ushort value = 0;
        foreach (int i in Enum.GetValues(typeof(CharList)))
        {
            if (key.ToString() == Enum.GetName(typeof(CharList), i))
            {
                value = (ushort)i;
                break;
            }
        }
        return value;
    }

    public static ushort GetValueOfString(string text)
    {
        int i = 0;
        ushort value = 0;
        for (i = 0; i < text.Length; i++)
        {
            value += GetValue(text[i]);
        }
        return value;
    }
}