using iTextSharp.text.pdf.parser;
using iTextSharp.text.pdf;
using DocumentFormat.OpenXml.Packaging;
using OpenXmlPowerTools;

namespace WindowsGrep.Core;
public static class FileUtils
{
    #region Methods..
    public static string ReadPdf(string filepath)
    {
        StringBuilder text = new StringBuilder();

        using (PdfReader reader = new PdfReader(filepath))
        {
            for (int i = 1; i <= reader.NumberOfPages; i++)
                text.Append(PdfTextExtractor.GetTextFromPage(reader, i));
        }

        return text.ToString();
    }

    public static string ReadDocX(string filepath)
    {
        StringBuilder text = new StringBuilder();

        byte[] byteArray = File.ReadAllBytes(filepath);
        using (var memoryStream = new MemoryStream(byteArray))
        using (WordprocessingDocument document = WordprocessingDocument.Open(memoryStream, false))
        {
            var xDocument = document.MainDocumentPart.GetXDocument();

            var paragraphs = xDocument.Descendants(W.p);
            foreach (var paragraph in paragraphs)
            {
                var paragraphText = paragraph.Descendants(W.t)
                    .Select(t => t.Value)
                    .StringConcatenate();

                text.AppendLine(paragraphText);
            }
        }

        return text.ToString();
    }
    #endregion Methods..
}
