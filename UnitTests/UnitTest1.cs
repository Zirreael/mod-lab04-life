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

[TestMethod]
public void TestFind()
{
            Board board;
            string name = "Board1.txt";
            Upload(name, 10, 10)
            Figure[] fig = Get_fig();
            Figure block = fig[0];
            int count = Find(block, board);
            Assert.AreEqual(count, 2);
}
[TestMethod]
public void TestAlive()
{
            Board board;
            string name = "Board1.txt";
            Upload(name, 10, 10)
            int count = board.AliveCount();
            Assert.AreEqual(count, 8);
}
[TestMethod]
public void TestSimm()
{
            Board board;
            string name = "Board1.txt";
            Upload(name, 10, 10)
            int count = Simm();
            Assert.AreEqual(count, 0);
}
}
}
