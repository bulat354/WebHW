using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.CodeDom;
using Microsoft.CSharp;

namespace HtmlEngineLibrary
{
    public class HtmlEngineService : IHtmlEngineService
    {

        public string GetHtml(string template, object model)
        {
            var renderer = new TemplateRenderer();
            return renderer.Render(template, new StatementVariables(model));
        }

        #region
        public void GenerateAndSaveInDirectory(string outputPath, string outputFileName, string template, object model)
        {
            throw new NotImplementedException();
        }

        public void GenerateAndSaveInDirectory(string outputPath, string outputFileName, Stream template, object model)
        {
            throw new NotImplementedException();
        }

        public void GenerateAndSaveInDirectory(string outputPath, string outputFileName, byte[] bytes, object model)
        {
            throw new NotImplementedException();
        }

        public Task GenerateAndSaveInDirectoryAsync(string outputPath, string outputFileName, string template, object model)
        {
            throw new NotImplementedException();
        }

        public Task GenerateAndSaveInDirectoryAsync(string outputPath, string outputFileName, Stream template, object model)
        {
            throw new NotImplementedException();
        }

        public Task GenerateAndSaveInDirectoryAsync(string outputPath, string outputFileName, byte[] bytes, object model)
        {
            throw new NotImplementedException();
        }

        public string GetHtml(Stream template, object model)
        {
            throw new NotImplementedException();
        }

        public string GetHtml(byte[] bytes, object model)
        {
            throw new NotImplementedException();
        }

        public byte[] GetHtmlInBytes(string template, object model)
        {
            throw new NotImplementedException();
        }

        public byte[] GetHtmlInBytes(Stream template, object model)
        {
            throw new NotImplementedException();
        }

        public byte[] GetHtmlInBytes(byte[] bytes, object model)
        {
            throw new NotImplementedException();
        }

        public Stream GetHtmlInStream(string template, object model)
        {
            throw new NotImplementedException();
        }

        public Stream GetHtmlInStream(Stream template, object model)
        {
            throw new NotImplementedException();
        }

        public Stream GetHtmlInStream(byte[] bytes, object model)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
