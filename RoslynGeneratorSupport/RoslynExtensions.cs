using System;
using System.Linq;
using Roslyn.Compilers.CSharp;

namespace RoslynGeneratorSupport
{
  public static class RoslynExtensions
  {
    public static AttributeSyntax Update(this AttributeSyntax originalAttribute,
      NameSyntax name = null,
      AttributeArgumentListSyntax argumentList = null)
    {
      return originalAttribute.Update(
        name ?? originalAttribute.Name,
        argumentList ?? originalAttribute.ArgumentList);
    }

    public static AttributeSyntax GetAttribute<TAttribute>(this MethodDeclarationSyntax method)
    {
      return GetAttribute<TAttribute>(method.Attributes);
    }

    public static AttributeSyntax GetAttribute<TAttribute>(this TypeDeclarationSyntax type)
    {
      return GetAttribute<TAttribute>(type.Attributes);
    }
    
    private static AttributeSyntax GetAttribute<TAttribute>(SyntaxList<AttributeDeclarationSyntax> attributeList)
    {
      var attributeType = typeof (TAttribute);
      return
        attributeList.SelectMany(a => a.Attributes).FirstOrDefault(
          a =>
          a.Name.PlainName.Split('.').Last() == attributeType.Name ||
          attributeType.Name.EndsWith("Attribute") &&
          a.Name.PlainName.Split('.').Last() ==
          attributeType.Name.Substring(0, attributeType.Name.Length - "Attribute".Length));
    }
  }
}
