using iTextSharp.text.pdf.parser;
using iTextSharp.text.pdf;

namespace WindowsGrep.Core;
public static class PdfUtils
{
    #region Methods..
    public static string ReadPdf(string filepath)
    {
        using (PdfReader reader = new PdfReader(filepath))
        {
            string text = string.Empty;
            for (int i = 1; i <= reader.NumberOfPages; i++)
                text += PdfTextExtractor.GetTextFromPage(reader, i);

            return text;
        }
    }
    #endregion Methods..
}
