﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CSCompiler.Entities.Compiler;
using System.Collections.Generic;

namespace CSCompiler.Test.Unit
{
    [TestClass]
    public class FullCompileTests
    {
        [TestMethod]
        public void Test_FullCompile_Simple_VarDefinitionInstruction_1()
        {
            // Arrange
            var source = "byte myVar = 17;";


            // Act
            var machineCodeProgram = Compiler.Compile(source);


            // Assert
            Assert.AreEqual(65536, machineCodeProgram.Bytes.Count);

            var expected = new List<byte>(new byte[] { 0x04, 0x00, 17, 0x05, 0x00, 0xce, 0x05, 0x80, 0x20, 0x2c, 0x00, 0x00 });
            var actual = ((List<byte>)machineCodeProgram.Bytes).GetRange(32768, expected.Count);
            CollectionAssert.AreEqual(expected, actual);

            var stringOutput = machineCodeProgram.GetBytesAsString(32768, expected.Count);
            Assert.AreEqual("04 00 11 05 00 ce 05 80 20 2c 00 00 ", stringOutput);
        }

        [TestMethod]
        public void Test_FullCompile_Two_VarDefinitionInstructions()
        {
            // Arrange
            var source = "byte myVar = 17;" + Environment.NewLine +
                         "byte myVar2 = 88;";


            // Act
            var machineCodeProgram = Compiler.Compile(source);


            // Assert
            Assert.AreEqual(65536, machineCodeProgram.Bytes.Count);

            var expected = new List<byte>(new byte[] { 
                0x04, 0x00, 17, 0x05, 0x00, 0xce, 0x05, 0x80, 0x20, 0x2c, 0x00, 0x00,
                0x04, 0x00, 88, 0x05, 0x00, 0xce, 0x05, 0x80, 0x21, 0x2c, 0x00, 0x00 
            });
            var actual = ((List<byte>)machineCodeProgram.Bytes).GetRange(32768, expected.Count);
            CollectionAssert.AreEqual(expected, actual);
        }

        // TODO: many tests here (copy from _old)

        [TestMethod]
        public void Test_TokensToMachineCode_IfInstruction_2()
        {
            // Arrange
            var source = "byte myVar = 65;" + Environment.NewLine +
                         "byte myVar2 = 65;" + Environment.NewLine +
                         "if(myVar == myVar2) {" + Environment.NewLine +
                         "  myVar = 27;" + Environment.NewLine +
                         "}";


            // Act
            var machineCodeProgram = Compiler.Compile(source);


            // Assert
            Assert.AreEqual(65536, machineCodeProgram.Bytes.Count);

            var expected = new List<byte>(new byte[] {
                0x04, 0x00, 65,     // LD A, 65     // byte myVar = 65;
                0x05, 0x00, 0xce,   // LD H, 0xce
                0x05, 0x80, 0x20,   // LD L, 0x20
                0x2c, 0x00, 0x00,   // ST [HL], A

                0x04, 0x00, 65,     // LD A, 65     // byte myVar = 65;
                0x05, 0x00, 0xce,   // LD H, 0xce
                0x05, 0x80, 0x21,   // LD L, 0x21
                0x2c, 0x00, 0x00,   // ST [HL], A

                0x05, 0x00, 0xce,   // LD H, 0xce   // if(myVar == myVar2) {
                0x05, 0x80, 0x20,   // LD L, 0x20
                0x10, 0x00, 0x00,   // LD A, [HL]
                0x05, 0x00, 0xce,   // LD H, 0xce
                0x05, 0x80, 0x21,   // LD L, 0x21
                0x12, 0x00, 0x00,   // LD C, [HL]
                0x84, 0x40, 0x00,   // SUB A, C     
                0x18, 0x80, 0x33,   // JP Z, 0x8033
                0x14, 0x80, 0x3f,   // JP 0x803f

                0x04, 0x00, 27,     // LD A, 27     // myVar = 27;
                0x05, 0x00, 0xce,   // LD H, 0xce
                0x05, 0x80, 0x20,   // LD L, 0x20
                0x2c, 0x00, 0x00,   // ST [HL], A
            });
            var actual = ((List<byte>)machineCodeProgram.Bytes).GetRange(32768, expected.Count);
            CollectionAssert.AreEqual(expected, actual);

            //var stringOutput = machineCodeProgram.GetBytesAsString(32768, expected.Count);
            //Assert.AreEqual("04 00 41 05 00 ce 05 80 20 2c 00 00 05 00 ce 05 80 20 10 00 00 44 00 00 ", stringOutput);
        }
    }
}
