using System.IO;
using Moq;
using FileSystemLib;
using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileSystemTests
{
    [TestClass]
    public class FileSystemVisitorValidationTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestFakeDependency()
        {
            var x = new Mock<IFakeClass>();
            x.Setup(l => l.ThrowException).Returns(true);
            var fileSystemVisitor = new FileSystemVisitor(x.Object);
            var result = fileSystemVisitor.GetFilesRecursive(new DirectoryInfo("")).ToList();
        }

        #region Old tests

        /* [Test]
         public void ItemValidation_FilterIsNull_ItemFilteredEventDoesNotCalled()
         {
             // Arrange
             FileSystemInfo fileSystemInfo = fileSystemInfoMock.Object;
             int expectedIndicatorValue = 1;
             int actualIndicatorValue = 0;

             // Act
             validation.Validate(fileSystemInfo, null, (s, eventArgs) => actualIndicatorValue++, (s, eventArgs) => actualIndicatorValue++);

             //Assert
             Assert.AreEqual(expectedIndicatorValue, actualIndicatorValue, "Wrong count of indicator if filter is null.");
         }

         [Test]
         public void ItemValidation_ItemFindedEventIsNull_ItemFindedEventDoesNotCalled()
         {
             // Arrange
             FileSystemInfo fileSystemInfo = fileSystemInfoMock.Object;
             int expectedIndicatorValue = 1;
             int actualIndicatorValue = 0;

             // Act
             validation.Validate(fileSystemInfo, fileInfo => true, null, (s, eventArgs) => actualIndicatorValue++);

             //Assert
             Assert.AreEqual(expectedIndicatorValue, actualIndicatorValue, "Wrong count of indicator if Item Finded event is null.");
         }

         [Test]
         public void ItemValidation_ItemFilteredEventIsNull_ItemFilteredEventDoesNotCalled()
         {
             // Arrange
             FileSystemInfo fileSystemInfo = fileSystemInfoMock.Object;
             int expectedIndicatorValue = 1;
             int actualIndicatorValue = 0;

             // Act
             validation.Validate(fileSystemInfo, fileInfo => true, (s, eventArgs) => actualIndicatorValue++, null);

             //Assert
             Assert.AreEqual(expectedIndicatorValue, actualIndicatorValue, "Wrong count of indicator if Item Filtered event is null.");
         }

         [Test]
         public void ItemValidation_AllFilesPassFilter_AllEventsCalled()
         {
             // Arrange
             FileSystemInfo fileSystemInfo = fileSystemInfoMock.Object;
             int expectedIndicatorValue = 2;
             int actualIndicatorValue = 0;

             // Act
             validation.Validate(fileSystemInfo, fileInfo => true, (s, eventArgs) => actualIndicatorValue++, (s, eventArgs) => actualIndicatorValue++);

             //Assert
             Assert.AreEqual(expectedIndicatorValue, actualIndicatorValue, "Wrong count of indicator if all files pass through filter.");
         }

         [Test]
         public void ItemValidation_AllFilesDoNotPassFilter_ItemFilteredEventDoesNotCalled()
         {
             // Arrange
             FileSystemInfo fileSystemInfo = fileSystemInfoMock.Object;
             int expectedIndicatorValue = 1;
             int actualIndicatorValue = 0;

             // Act
             validation.Validate(fileSystemInfo, fileInfo => false, (s, eventArgs) => actualIndicatorValue++, (s, eventArgs) => actualIndicatorValue++);

             //Assert
             Assert.AreEqual(expectedIndicatorValue, actualIndicatorValue, "Wrong count of indicator if all files don't pass through filter.");
         }

         [Test]
         public void ItemValidation_ItemFindedEvent_ContinueActionReturned()
         {
             // Arrange
             FileSystemInfo fileSystemInfo = fileSystemInfoMock.Object;

             // Act
             var actualAction = validation.Validate(fileSystemInfo, fileInfo => true, (s, eventArgs) => { }, null);

             //Assert
             Assert.AreEqual(ActionType.Continue, actualAction, "Wrong action type if all files pass through filter.");
         }

         [Test]
         public void ItemValidation_ItemFilteredEvent_ContinueActionReturned()
         {
             // Arrange
             FileSystemInfo fileSystemInfo = fileSystemInfoMock.Object;

             // Act
             var actualAction = validation.Validate(fileSystemInfo, fileInfo => true, (s, eventArgs) => { }, (s, eventArgs) => { });

             //Assert
             Assert.AreEqual(ActionType.Continue, actualAction, "Wrong action type if all files pass through filter.");
         }

         [Test]
         public void ItemValidation_ItemFindedEvent_SkipActionReturned()
         {
             // Arrange
             FileSystemInfo fileSystemInfo = fileSystemInfoMock.Object;
             int expectedIndicatorValue = 1;
             int actualIndicatorValue = 0;

             // Act
             var actualAction = validation.Validate(fileSystemInfo, fileInfo => true,
                 (s, eventArgs) =>
                 {
                     actualIndicatorValue++;
                     eventArgs.ActionType = ActionType.Skip;
                 }, 
                 (s, eventArgs) => actualIndicatorValue++);

             //Assert
             Assert.AreEqual(ActionType.Skip, actualAction, "Wrong action type if all files pass through filter.");
             Assert.AreEqual(expectedIndicatorValue, actualIndicatorValue, "Wrong count of indicator with Skip action type for ItemFinded Event.");
         }

         [Test]
         public void ItemValidation_ItemFilteredEvent_SkipActionReturned()
         {
             // Arrange
             FileSystemInfo fileSystemInfo = fileSystemInfoMock.Object;
             int expectedIndicatorValue = 2;
             int actualIndicatorValue = 0;

             // Act
             var actualAction = validation.Validate(fileSystemInfo, fileInfo => true,
                 (s, eventArgs) => actualIndicatorValue++,
                 (s, eventArgs) =>
                 {
                     actualIndicatorValue++;
                     eventArgs.ActionType = ActionType.Skip;
                 });

             //Assert
             Assert.AreEqual(ActionType.Skip, actualAction, "Wrong action type if all files pass through filter.");
             Assert.AreEqual(expectedIndicatorValue, actualIndicatorValue, "Wrong count of indicator with Skip action type for ItemFiltered Event.");
         }

         [Test]
         public void ItemValidation_ItemFindedEvent_StopActionReturned()
         {
             // Arrange
             FileSystemInfo fileSystemInfo = fileSystemInfoMock.Object;
             int expectedIndicatorValue = 1;
             int actualIndicatorValue = 0;

             // Act
             var actualAction = validation.Validate(fileSystemInfo, fileInfo => true,
                 (s, eventArgs) =>
                 {
                     actualIndicatorValue++;
                     eventArgs.ActionType = ActionType.Stop;
                 },
                 (s, eventArgs) => actualIndicatorValue++);

             //Assert
             Assert.AreEqual(ActionType.Stop, actualAction, "Wrong action type if all files pass through filter.");
             Assert.AreEqual(expectedIndicatorValue, actualIndicatorValue, "Wrong count of indicator with Stop action type for ItemFinded Event.");
         }

         [Test]
         public void ItemValidation_ItemFilteredEvent_StopActionReturned()
         {
             // Arrange
             FileSystemInfo fileSystemInfo = fileSystemInfoMock.Object;
             int expectedIndicatorValue = 2;
             int actualIndicatorValue = 0;

             // Act
             var actualAction = validation.Validate(fileSystemInfo, fileInfo => true,
                 (s, eventArgs) => actualIndicatorValue++,
                 (s, eventArgs) =>
                 {
                     actualIndicatorValue++;
                     eventArgs.ActionType = ActionType.Stop;
                 });

             //Assert
             Assert.AreEqual(ActionType.Stop, actualAction, "Wrong action type if all files pass through filter.");
             Assert.AreEqual(expectedIndicatorValue, actualIndicatorValue, "Wrong count of indicator with Stop action type for ItemFiltered Event.");
         }*/

        #endregion 
    }
}