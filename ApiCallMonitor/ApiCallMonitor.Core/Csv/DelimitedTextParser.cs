using System.Text;

namespace ApiCallMonitor.Core.Csv;

/// <summary>Minimal RFC4180-ish delimited-text parser: handles quoted fields (including embedded
/// delimiters, quotes, and newlines) and CRLF/LF line endings, without pulling in a third-party CSV
/// library. Auto-detects comma vs. tab as the delimiter from the first line, so pasting straight out
/// of Excel or an SSMS results grid (both tab-delimited) works the same as a plain .csv.</summary>
public static class DelimitedTextParser
{
    public static List<string[]> Parse(string text)
    {
        var rows = new List<string[]>();
        if (string.IsNullOrEmpty(text))
        {
            return rows;
        }

        var delimiter = DetectDelimiter(text);
        var currentRow = new List<string>();
        var field = new StringBuilder();
        var inQuotes = false;

        void EndField()
        {
            currentRow.Add(field.ToString());
            field.Clear();
        }

        void EndRow()
        {
            EndField();
            var isBlankLine = currentRow.Count == 1 && currentRow[0].Length == 0;
            if (!isBlankLine)
            {
                rows.Add(currentRow.ToArray());
            }

            currentRow = new List<string>();
        }

        var i = 0;
        while (i < text.Length)
        {
            var c = text[i];

            if (inQuotes)
            {
                if (c == '"')
                {
                    if (i + 1 < text.Length && text[i + 1] == '"')
                    {
                        field.Append('"');
                        i += 2;
                        continue;
                    }

                    inQuotes = false;
                    i++;
                    continue;
                }

                field.Append(c);
                i++;
                continue;
            }

            if (c == '"')
            {
                inQuotes = true;
                i++;
                continue;
            }

            if (c == delimiter)
            {
                EndField();
                i++;
                continue;
            }

            if (c == '\r')
            {
                i++;
                continue;
            }

            if (c == '\n')
            {
                EndRow();
                i++;
                continue;
            }

            field.Append(c);
            i++;
        }

        // Flush a final line that wasn't terminated by a trailing newline.
        if (field.Length > 0 || currentRow.Count > 0)
        {
            EndRow();
        }

        return rows;
    }

    private static char DetectDelimiter(string text)
    {
        var firstLine = text.Split('\n', 2)[0];
        return firstLine.Contains('\t') ? '\t' : ',';
    }
}
