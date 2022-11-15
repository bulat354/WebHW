using System.Text;
using HtmlEngineLibrary.TemplateRendering;

namespace HtmlEngineLibrary
{
    public class HtmlEngineService : IEngineHtmlService
    {
        public string GetHtml(string template, object model)
        {
            var renderer = new TemplateRenderer();
            return renderer.Render(template, new StatementVariables(model));
        }

        public string GetHtml(Stream stream, object model)
        {
            var renderer = new TemplateRenderer();

            using (var reader = new StreamReader(stream))
            {
                var template = reader.ReadToEnd();
                return renderer.Render(template, new StatementVariables(model));
            }
        }

        public string GetHtml(byte[] bytes, object model)
        {
            var renderer = new TemplateRenderer();

            var template = Encoding.UTF8.GetString(bytes);
            return renderer.Render(template, new StatementVariables(model));
        }

        public byte[] GetHtmlInBytes(string template, object model)
        {
            return Encoding.UTF8.GetBytes(GetHtml(template, model));
        }

        public byte[] GetHtmlInBytes(Stream stream, object model)
        {
            return Encoding.UTF8.GetBytes(GetHtml(stream, model));
        }

        public byte[] GetHtmlInBytes(byte[] bytes, object model)
        {
            return Encoding.UTF8.GetBytes(GetHtml(bytes, model));
        }

        public Stream GetHtmlInStream(string template, object model)
        {
            return new MemoryStream(GetHtmlInBytes(template, model));
        }

        public Stream GetHtmlInStream(Stream stream, object model)
        {
            return new MemoryStream(GetHtmlInBytes(stream, model));
        }

        public Stream GetHtmlInStream(byte[] bytes, object model)
        {
            return new MemoryStream(GetHtmlInBytes(bytes, model));
        }

        public void GenerateAndSaveInDirectory(string outputPath, string outputFileName, string template, object model)
        {
            File.WriteAllText(Path.Combine(Path.GetFullPath(outputPath), outputFileName), GetHtml(template, model));
        }

        public void GenerateAndSaveInDirectory(string outputPath, string outputFileName, Stream stream, object model)
        {
            File.WriteAllText(Path.Combine(Path.GetFullPath(outputPath), outputFileName), GetHtml(stream, model));
        }

        public void GenerateAndSaveInDirectory(string outputPath, string outputFileName, byte[] bytes, object model)
        {
            File.WriteAllText(Path.Combine(Path.GetFullPath(outputPath), outputFileName), GetHtml(bytes, model));
        }

        public Task GenerateAndSaveInDirectoryAsync(string outputPath, string outputFileName, string template, object model)
        {
            return File.WriteAllTextAsync(Path.Combine(Path.GetFullPath(outputPath), outputFileName), GetHtml(template, model));
        }

        public Task GenerateAndSaveInDirectoryAsync(string outputPath, string outputFileName, Stream stream, object model)
        {
            return File.WriteAllTextAsync(Path.Combine(Path.GetFullPath(outputPath), outputFileName), GetHtml(stream, model));
        }

        public Task GenerateAndSaveInDirectoryAsync(string outputPath, string outputFileName, byte[] bytes, object model)
        {
            return File.WriteAllTextAsync(Path.Combine(Path.GetFullPath(outputPath), outputFileName), GetHtml(bytes, model));
        }
    }
}
