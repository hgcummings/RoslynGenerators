# Roslyn Generators

This is a project contains some supporting classes for creating Roslyn-based single file code generators as Visual Studio extensions, and includes an example generator for asynchronous WCF service interfaces.

For more information on Roslyn, see http://msdn.microsoft.com/en-gb/roslyn

## Usage

 * Install the custom generator
   * Build all
   * Locate AsyncGenerator.vsix in the output of the AsyncGenerator project and double-click it
 * Use the custom generator
   * Open a new instance of VisualStudio and create/open a synchronous WCF service contract interface
   * Select properties on the interface file and enter 'AsyncGenerator' in the 'Custom Tool' field
   * A file containing a corresponding asynchronous version of the interface should be generated
   * Make a change to the original file and save it
   * The generated file should update immediately
 * Create a new Roslyn-based generator
   * Drop RoslynGeneratorTemplate.zip into [UserDirectory]\Documents\Visual Studio 2010\Templates\ProjectTemplates\Visual C#
   * Open the RoslynGenerators solution file in Visual Studio
   * Add a new project from the template (note that the name of your generator class will default to the project name)
   * Open source.extension.vsix manifest in the XML Editor (*not* the Designer) and set the description, author etc. as desired
   * Implement ComputeNewRootNode to perform the desired Syntax tree transformation
   * When ready to test in Visual Studio, follow the instructions above for installing and using the AsyncGenerator, adapting them to your generator where necessary

 * Hints for writing your custom generator
   * Familiarise yourself with the extension methods in RoslynGeneratorSupport, in case they can be helpful
   * Consider writing a test like the one in AsyncGenerators.Tests to help you debug

## Description
This project contains three class libraries:

### RoslynGeneratorSupport

Supporting classes for creating a code generator using Roslyn.

 * Makes use of most of the single file generator boilerplate from http://code.msdn.microsoft.com/sfgdd
 * Adds some useful extension methods for dealing with Roslyn syntax tress.
 * Provides an abstract RoslynGenerator base class with a single abstract method to override, for converting from one syntax tree to another
 

### AsyncGenerator

An example extension for generating an async WCF service interface from the corresponding synchronous interface at design time.

### AsyncGenerator.Tests

A very simple test for the AsyncGenerator. Makes use of the public GenerateCodeAsString test hook method in the RoslynGenerator abstract base class.