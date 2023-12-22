using Sprache;

namespace Tools;

public static class ParseEx
{
    public static Parser<string> Word { get; } = Parse.Letter.Many().Text();
}
