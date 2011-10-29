using System;
using System.Linq;
using Roslyn.Compilers.CSharp;

namespace RoslynGeneratorSupport
{
  public static class RoslynExtensions
  {
    public static NamespaceDeclarationSyntax Update(this NamespaceDeclarationSyntax originalNamespace,
      SyntaxToken? namespaceKeyword = null,
      NameSyntax name = null,
      SyntaxToken? openBraceToken = null,
      SyntaxList<ExternAliasDirectiveSyntax>? externs = null,
      SyntaxList<UsingDirectiveSyntax>? usings = null,
      SyntaxList<MemberDeclarationSyntax>? members = null,
      SyntaxToken? closeBraceToken = null,
      SyntaxToken? semicolonTokenOpt = null)
    {
      return originalNamespace.Update(
        namespaceKeyword ?? originalNamespace.NamespaceKeyword,
        name ?? originalNamespace.Name,
        openBraceToken ?? originalNamespace.OpenBraceToken,
        externs ?? originalNamespace.Externs,
        usings ?? originalNamespace.Usings,
        members ?? originalNamespace.Members,
        closeBraceToken ?? originalNamespace.CloseBraceToken,
        semicolonTokenOpt ?? originalNamespace.SemicolonTokenOpt);
    }

    public static InterfaceDeclarationSyntax Update(this InterfaceDeclarationSyntax originalInterface,
      SyntaxList<AttributeDeclarationSyntax>? attributes = null,
      SyntaxTokenList? modifiers = null,
      SyntaxToken? keyword = null,
      SyntaxToken? identifier = null,
      TypeParameterListSyntax typeParameterListOpt = null,
      BaseListSyntax baseListOpt = null,
      SyntaxList<TypeParameterConstraintClauseSyntax>? constraintClauses = null,
      SyntaxToken? openBraceToken = null,
      SyntaxList<MemberDeclarationSyntax>? members = null,
      SyntaxToken? closeBraceToken = null,
      SyntaxToken? semicolonTokenOpt = null)
    {
      return originalInterface.Update(
        attributes ?? originalInterface.Attributes,
        modifiers ?? originalInterface.Modifiers,
        keyword ?? originalInterface.Keyword,
        identifier ?? originalInterface.Identifier,
        typeParameterListOpt ?? originalInterface.TypeParameterListOpt,
        baseListOpt ?? originalInterface.BaseListOpt,
        constraintClauses ?? originalInterface.ConstraintClauses,
        openBraceToken ?? originalInterface.OpenBraceToken,
        members ?? originalInterface.Members,
        closeBraceToken ?? originalInterface.CloseBraceToken,
        semicolonTokenOpt ?? originalInterface.SemicolonTokenOpt);
    }

    public static MethodDeclarationSyntax Update(this MethodDeclarationSyntax originalMethod,
      SyntaxList<AttributeDeclarationSyntax>? attributes = null,
      SyntaxTokenList? modifiers = null,
      TypeSyntax returnType = null,
      ExplicitInterfaceSpecifierSyntax explicitInterfaceSpecifierOpt = null,
      SyntaxToken? identifier = null,
      TypeParameterListSyntax typeParameterListOpt = null,
      ParameterListSyntax parameterList = null,
      SyntaxList<TypeParameterConstraintClauseSyntax>? constraintClauses = null,
      BlockSyntax bodyOpt = null,
      SyntaxToken? semicolonTokenOpt = null)
    {
      return originalMethod.Update(
        attributes ?? originalMethod.Attributes,
        modifiers ?? originalMethod.Modifiers,
        returnType ?? originalMethod.ReturnType,
        explicitInterfaceSpecifierOpt ?? originalMethod.ExplicitInterfaceSpecifierOpt,
        identifier ?? originalMethod.Identifier,
        typeParameterListOpt ?? originalMethod.TypeParameterListOpt,
        parameterList ?? originalMethod.ParameterList,
        constraintClauses ?? originalMethod.ConstraintClauses,
        bodyOpt ?? originalMethod.BodyOpt,
        semicolonTokenOpt ?? originalMethod.SemicolonTokenOpt);
    }

    public static AttributeSyntax Update(this AttributeSyntax originalAttribute,
      NameSyntax name = null,
      AttributeArgumentListSyntax argumentListOpt = null)
    {
      return originalAttribute.Update(
        name ?? originalAttribute.Name,
        argumentListOpt ?? originalAttribute.ArgumentListOpt);
    }

    public static AttributeSyntax GetAttribute(this MethodDeclarationSyntax method, Type attributeType)
    {
      return GetAttribute(method.Attributes, attributeType);
    }

    public static AttributeSyntax GetAttribute(this TypeDeclarationSyntax type, Type attributeType)
    {
      return GetAttribute(type.Attributes, attributeType);
    }

    private static AttributeSyntax GetAttribute(SyntaxList<AttributeDeclarationSyntax> attributeList, Type attributeType)
    {
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
