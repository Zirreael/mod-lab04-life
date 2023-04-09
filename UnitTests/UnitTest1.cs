using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using cli_life;

namespase UnitTests
{
[TestClass]
public class UnitTest1
{
[TestMethod]
public void TestRead()
{
            string name = "Block.txt";
            static Board board;
            Upload(name, 2, 2);
            Assert.IsTrue(board.Cells[0, 0].IsAlive);
            Assert.IsTrue(board.Cells[0, 1].IsAlive);
            Assert.IsTrue(board.Cells[1, 0].IsAlive);
            Assert.IsTrue(board.Cells[1, 1].IsAlive);
            }
}
}
