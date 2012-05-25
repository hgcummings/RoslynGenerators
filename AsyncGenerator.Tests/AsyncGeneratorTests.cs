using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AsyncGenerator.Tests
{
  /// <summary>
  /// Summary description for UnitTest1
  /// </summary>
  [TestClass]
  public class AsyncGeneratorTests
  {
    [TestMethod]
    public void GenerateCodeAsString_SynchronousServiceInterface_GeneratesAsyncInterface()
    {
      // Arrange
      var asyncGenerator = new AsyncGenerator();

      // Act
      var outputFileContents = asyncGenerator.GenerateCodeAsString(InputFileContents);

      // Assert
      Assert.AreEqual(ExpectedOutputFileContents, outputFileContents);
    }

    private const string InputFileContents =
      @"using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WcfServiceLibrary1
{
    [ServiceContract]
    [XmlSerializerFormat]
    public interface IService1
    {
        [OperationContract]
        string GetData(int value);

        [OperationContract]
        OutputType GetComplexData(InputType value);
    }
}";

    private const string ExpectedOutputFileContents =
      @"using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WcfServiceLibrary1
{
    [ServiceContract]
    [XmlSerializerFormat]
    public interface IService1Async
    {
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetData(int value, AsyncCallback callback, object state);
        string EndGetData(IAsyncResult result);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetComplexData(InputType value, AsyncCallback callback, object state);
        OutputType EndGetComplexData(IAsyncResult result);
    }
}";
  }
}
