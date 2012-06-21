using System;
using System.Text;
using Microsoft.Samples.VisualStudio.GeneratorSample;
using Roslyn.Compilers.CSharp;
using Roslyn.Services;

namespace RoslynGeneratorSupport
{
  public abstract class RoslynGenerator : BaseCodeGeneratorWithSite
  {
    protected override byte[] GenerateCode(string inputFileContent)
    {
      return ConvertToByteArray(GenerateCodeAsString(inputFileContent));
    }

    public string GenerateCodeAsString(string inputFileContent)
    {
      var tree = SyntaxTree.ParseCompilationUnit(inputFileContent);
      return ComputeNewRootNode(tree.GetRoot()).Format().GetFormattedRoot().GetText();
    }
    
    private static byte[] ConvertToByteArray(string content)
    {
      //Get the preamble (byte-order mark) for our encoding
      byte[] preamble = Encoding.UTF8.GetPreamble();
      int preambleLength = preamble.Length;

      byte[] body = Encoding.UTF8.GetBytes(content);

      //Prepend the preamble to body (store result in resized preamble array)
      Array.Resize(ref preamble, preambleLength + body.Length);
      Array.Copy(body, 0, preamble, preambleLength, body.Length);

      //Return the combined byte array
      return preamble;
    }

    protected abstract SyntaxNode ComputeNewRootNode(SyntaxNode rootNode);
  }
}
