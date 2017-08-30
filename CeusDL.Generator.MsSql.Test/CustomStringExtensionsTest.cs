using Microsoft.VisualStudio.TestTools.UnitTesting;
using KDV.CeusDL.Generator;

namespace KDV.CeusDL.Generator.MsSql.Test
{
    [TestClass]
    public class CustomStringExtensionsTest
    {
        [TestMethod]
        public void TestIndent()
        {
            string baseStr = "Hallo Welt\nNa wie gehts\n...";
            string targetStr = "   Hallo Welt\n   Na wie gehts\n   ...";

            string resultStr = baseStr.Indent("   ");

            Assert.AreEqual(targetStr, resultStr);
            Assert.AreNotEqual(targetStr, baseStr);
        }
    }
}
