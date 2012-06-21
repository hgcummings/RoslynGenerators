using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceModel;
using Microsoft.VisualStudio.Shell;
using Roslyn.Compilers.CSharp;
using VSLangProj80;
using RoslynGeneratorSupport;

namespace AsyncGenerator
{
  /// <summary>
  /// When setting the 'Custom Tool' property of a C# project item to 'AsyncGenerator', the
  /// ComputeNewRootNode function will get called and the result we be used to generate a new file
  /// </summary>
  [ComVisible(true)]
  [Guid("d2c3268e-2b20-472b-a301-7598a98dc061")]
  [CodeGeneratorRegistration(typeof(AsyncGenerator), "C# AsyncGenerator", vsContextGuids.vsContextGuidVCSProject, GeneratesDesignTimeSource = true)]
  [ProvideObject(typeof(AsyncGenerator))]
  public class AsyncGenerator : RoslynGenerator
  {
    // ReSharper disable InconsistentNaming
#pragma warning disable 0414
    //The name of this generator (use for 'Custom Tool' property of project item)
    internal static string name = "AsyncGenerator";
#pragma warning restore 0414
    // ReSharper restore InconsistentNaming

    protected override SyntaxNode ComputeNewRootNode(SyntaxNode rootNode)
    {
      var namespaceDeclarations = rootNode.DescendantNodes().OfType<NamespaceDeclarationSyntax>();
      return rootNode.ReplaceNodes(namespaceDeclarations, (n1, n2) => ComputeNewNamespaceDeclarationNode(n1));
    }

    /// <summary>
    ///   Takes a namespace declaration and returns a new namespace declaration containing only
    ///   the ServiceContract interfaces, converted to an asynchronous version
    /// </summary>
    /// <param name="originalNamespace">The namespace declaration to replace</param>
    /// <returns></returns>
    private static NamespaceDeclarationSyntax ComputeNewNamespaceDeclarationNode(NamespaceDeclarationSyntax originalNamespace)
    {
      var serviceContractInterfaces =
        originalNamespace.DescendantNodes().OfType<InterfaceDeclarationSyntax>().Where(
          i => i.GetAttribute<ServiceContractAttribute>() != null);

      return originalNamespace.WithMembers(Syntax.List<MemberDeclarationSyntax>(serviceContractInterfaces.Select(ComputeNewServiceContractInterfaceNode)));
    }

    /// <summary>
    ///   Takes a synchronous ServiceContract interface and returns an asynchronous version
    /// </summary>
    /// <param name="originalInterface"></param>
    /// <returns></returns>
    private static InterfaceDeclarationSyntax ComputeNewServiceContractInterfaceNode(InterfaceDeclarationSyntax originalInterface)
    {
      var newMembers = new List<MemberDeclarationSyntax>(
        originalInterface.Members.OfType<MethodDeclarationSyntax>()
          .Where(m => m.GetAttribute<OperationContractAttribute>() != null)
          .SelectMany(ComputeNewOperationContractNodes));

      return originalInterface
        .WithIdentifier(Syntax.Identifier(originalInterface.Identifier.ValueText + "Async"))
        .WithMembers(Syntax.List<MemberDeclarationSyntax>(newMembers));
    }

    /// <summary>
    ///   Takes a synchronous OperationContract method and returns a corresponding asynchronous method pair
    /// </summary>
    /// <param name="originalMethod"></param>
    /// <returns></returns>
    private static IEnumerable<MemberDeclarationSyntax> ComputeNewOperationContractNodes(MethodDeclarationSyntax originalMethod)
    {
      // Turn the original method into a 'Begin' method with additional Async parameters..
      IEnumerable<ParameterSyntax> extraParameters =
        new List<ParameterSyntax>
        {
          Syntax.Parameter(Syntax.Identifier("callback")).WithType(Syntax.IdentifierName(typeof (AsyncCallback).Name)),
          Syntax.Parameter(Syntax.Identifier("state")).WithType(Syntax.PredefinedType(Syntax.Token(SyntaxKind.ObjectKeyword)))
        };

      var parameters = originalMethod.ParameterList == null ? extraParameters : originalMethod.ParameterList.Parameters.Concat(extraParameters);
      var seperators = Enumerable.Range(0, parameters.Count() - 1).Select(i => Syntax.Token(SyntaxKind.CommaToken));

      var beginMethod = originalMethod
        .WithReturnType(Syntax.IdentifierName(typeof(IAsyncResult).Name))
        .WithIdentifier(Syntax.Identifier("Begin" + originalMethod.Identifier.ValueText))
        .WithParameterList(Syntax.ParameterList(Syntax.SeparatedList(parameters, seperators)));

      // Alter the OperationContractAttribute to specify AsyncPattern = true
      var operationContractAttribute = beginMethod.GetAttribute<OperationContractAttribute>();
      beginMethod = beginMethod.ReplaceNodes(new[] { operationContractAttribute }, (x, y) => ComputeNewOperationContractAttributeNode(x));

      // Create the 'End' method
      var endMethod = Syntax.MethodDeclaration(originalMethod.ReturnType, Syntax.Identifier("End" + originalMethod.Identifier.ValueText))
        .WithParameterList(
          Syntax.ParameterList(Syntax.SeparatedList(Syntax.Parameter(Syntax.Identifier("result")).WithType(Syntax.IdentifierName(typeof(IAsyncResult).Name)))))
        .WithSemicolonToken(Syntax.Token(SyntaxKind.SemicolonToken));

      return new[] { beginMethod, endMethod };
    }

    /// <summary>
    ///   Takes an OperationContract attribute node and returns a new node with 'AsyncPattern = true'
    /// </summary>
    /// <param name="originalAttribute"></param>
    /// <returns></returns>
    private static AttributeSyntax ComputeNewOperationContractAttributeNode(AttributeSyntax originalAttribute)
    {
      var newAttributeArguments = new List<AttributeArgumentSyntax>
                              {
                                Syntax.AttributeArgument(Syntax.LiteralExpression(SyntaxKind.TrueLiteralExpression))
                                  .WithNameEquals(Syntax.NameEquals(Syntax.IdentifierName("AsyncPattern"), Syntax.Token(SyntaxKind.EqualsToken)))
                              };

      SeparatedSyntaxList<AttributeArgumentSyntax> newAttributeArgumentList;

      if (originalAttribute.ArgumentList == null)
      {
        newAttributeArgumentList = Syntax.SeparatedList(
          newAttributeArguments,
          Enumerable.Empty<SyntaxToken>());
      }
      else
      {
        newAttributeArgumentList = Syntax.SeparatedList(
          originalAttribute.ArgumentList.Arguments.Concat(newAttributeArguments),
          Enumerable.Range(0, originalAttribute.ArgumentList.Arguments.SeparatorCount + 1).Select(i => Syntax.Token(SyntaxKind.CommaToken)));
      }

      return originalAttribute.WithArgumentList(Syntax.AttributeArgumentList(newAttributeArgumentList));
    }
  }
}