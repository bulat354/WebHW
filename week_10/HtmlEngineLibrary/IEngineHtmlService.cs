namespace HtmlEngineLibrary
{
    public interface IHtmlEngineService
    {
        string GetHtml(string template, object model);
        string GetHtml(Stream template, object model);
        string GetHtml(byte[] bytes, object model);
        Stream GetHtmlInStream(string template, object model);
        Stream GetHtmlInStream(Stream template, object model);
        Stream GetHtmlInStream(byte[] bytes, object model);
        byte[] GetHtmlInBytes(string template, object model);
        byte[] GetHtmlInBytes(Stream template, object model);
        byte[] GetHtmlInBytes(byte[] bytes, object model);
        void GenerateAndSaveInDirectory(string outputPath, string outputFileName, string template, object model);
        void GenerateAndSaveInDirectory(string outputPath, string outputFileName, Stream template, object model);
        void GenerateAndSaveInDirectory(string outputPath, string outputFileName, byte[] bytes, object model);
        Task GenerateAndSaveInDirectoryAsync(string outputPath, string outputFileName, string template, object model);
        Task GenerateAndSaveInDirectoryAsync(string outputPath, string outputFileName, Stream template, object model);
        Task GenerateAndSaveInDirectoryAsync(string outputPath, string outputFileName, byte[] bytes, object model);
    }
}