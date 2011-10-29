# Roslyn Generators

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
 * Create a new Roslyn-based generator (TODO:HGC test these steps from scratch)
   * Create a new project of type 'VSIX' project
   * Edit the project file and set IncludeAssemblyInVSIXContainer to true
   * Reference RoslynGeneratorSupport and inherit from Roslyn generator
   * Override ComputeNewRootNode to perform the desired Syntax tree transformation
   * Make use of the extension methods in RoslynExtensions 
   * Edit the vsixmanifest file and add the following to the end:
      <Content>
        <Assembly AssemblyName="RoslynGeneratorSupport, PublicKeyToken=27d5caa3a27a807f"/>
      </Content>

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