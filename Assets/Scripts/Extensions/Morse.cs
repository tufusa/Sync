using System;
using System.Collections.Generic;
using System.Linq;

static class Morse
{
    // . = 1, - = 0
    public static List<int> GetMorse(this char ch)
    {
        string codeStr = char.ToUpper(ch) switch
        {
            'A' => ".-",
            'B' => "-...",
            'C' => "-.-.",
            'D' => "-..",
            'E' => ".",
            'F' => "..-.",
            'G' => "--.",
            'H' => "....",
            'I' => "..",
            'J' => ".---",
            'K' => "-.-",
            'L' => ".-..",
            'M' => "--",
            'N' => "-.",
            'O' => "---",
            'P' => ".--.",
            'Q' => "--.-",
            'R' => ".-.",
            'S' => "...",
            'T' => "-",
            'U' => "..-",
            'V' => "...-",
            'W' => ".--",
            'X' => "-..-",
            'Y' => "-.--",
            'Z' => "--..",
            '0' => "-----",
            '1' => ".----",
            '2' => "..---",
            '3' => "...--",
            '4' => "....-",
            '5' => ".....",
            '6' => "-....",
            '7' => "--...",
            '8' => "---..",
            '9' => "----.",
            _ => null
        };
        if (codeStr == null) return new List<int>();
        return codeStr.ToCharArray().ToList().ConvertAll(c => MorseToInt(c));
    }

    static int MorseToInt(char c)
    {
        if (c == '.') return 1;
        if (c == '-') return 0;
        throw new Exception("An invalid character was given.");
    }
}