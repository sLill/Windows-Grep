using iTextSharp.text.pdf.parser;
using iTextSharp.text.pdf;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

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

        using (WordprocessingDocument document = WordprocessingDocument.Open(filepath, false))
        {
            var paragraphs = document.MainDocumentPart.Document.Body.Elements<Paragraph>();
            foreach (var paragraph in paragraphs)
            {
                text.AppendLine(string.Join(" ", paragraph.Elements<Run>()
                    .Select(run => run.Elements<Text>())
                    .SelectMany(texts => texts.Select(t => t.Text))));
            }
        }

        return text.ToString();
    }
    #endregion Methods..
}
