﻿using CSCompiler.Entities.CS.Tokens;
using CSCompiler.Entities.MachineCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSCompiler.Entities;
using CSCompiler.Exceptions;

namespace CSCompiler.Entities.CS
{
    public class CSProgram
    {
        // Constructor
        public CSProgram()
        {
            this.Commands = new List<Command>();
            this.Variables = new List<Variable>();
        }

        public string SourceCodeText;

        public IList<Command> Commands { get; set; }
        public IList<Variable> Variables { get; set; }

        // State Machine
        private enum States
        {
            None,
            TypeOrIdentifierTokenStarted,
            LiteralTokenStarted,
            //TokenEnd,
        }

        public IList<Token> ConvertSourceToTokens()
        {
            States currentState = States.None;

            var tokens = new List<Token>();
            var currentToken = "";
            for (int i = 0; i < this.SourceCodeText.Length; i++)
            {
                char currentChar = SourceCodeText.ElementAt(i);

                switch (currentState)
                {
                    case States.None:
                        if (Constants.IsIrrelevantChar(currentChar))
                        {
                            continue;
                        }
                        else
                        {
                            if (Char.IsLetter(currentChar))
                            {
                                currentState = States.TypeOrIdentifierTokenStarted;
                                currentToken = currentChar.ToString();
                            }
                            else if (Char.IsNumber(currentChar))
                            {
                                currentState = States.LiteralTokenStarted;
                                currentToken = currentChar.ToString();
                            }
                            else
                            {
                                CheckSingleTokens(tokens, currentChar);
                                
                                currentToken = "";
                            }
                        }
                        break;

                    case States.TypeOrIdentifierTokenStarted:
                        if (Char.IsLetterOrDigit(currentChar))
                        {
                            // Identifier continues
                            currentToken += currentChar;
                        }
                        else
                        {
                            // Identifier ends
                            if (Constants.IsValidType(currentToken))
                            {
                                tokens.Add(new TypeToken(currentToken));

                                //currentToken = "";
                            }
                            else if (Constants.IsCommand(currentToken))
                            {
                                tokens.Add(new CommandToken(currentToken));

                                //currentToken = "";
                            }
                            else
                            {
                                tokens.Add(new IdentifierToken(currentToken));
                            }

                            CheckSingleTokens(tokens, currentChar);

                            currentToken = "";

                            currentState = States.None;
                        }
                        break;

                    case States.LiteralTokenStarted:
                        if (Char.IsNumber(currentChar))
                        {
                            // Literal continues
                            currentToken += currentChar;
                        }
                        else
                        {
                            // Literal ends
                            tokens.Add(new LiteralToken(currentToken));

                            CheckSingleTokens(tokens, currentChar);

                            currentToken = "";
                            currentState = States.None;
                        }
                        break;
                }
            }

            return tokens;
        }

        private static void CheckSingleTokens(List<Token> tokens, char currentChar)
        {
            if (Constants.IsArithmeticSignal(currentChar))
            {
                tokens.Add(new ArithmeticSignalToken(currentChar.ToString()));
            }
            else
            {
                switch (currentChar)
                {
                    case '=':
                        tokens.Add(new EqualToken());
                        break;

                    case ';':
                        tokens.Add(new SemicolonToken());
                        break;

                    case '(':
                        tokens.Add(new OpenParenthesisToken());
                        break;

                    case ')':
                        tokens.Add(new CloseParenthesisToken());
                        break;

                    case ',':
                        tokens.Add(new CommaToken());
                        break;

                    default:
                        break;
                }
            }
        }

        public MachineCodeProgram ConvertTokensToMachineCode(IList<Token> tokens)
        {
            var machineCodeProgram = new MachineCodeProgram();
            var currentProgramAddr = Constants.BASE_ADDR_PROGRAM;
            var currentVariableAddr = Constants.BASE_ADDR_VARIABLES;

            var currentCommandTokens = new List<Token>();
            var lastToken = tokens.Last();
            foreach (var token in tokens)
            {
                currentCommandTokens.Add(token);
                if (token is SemicolonToken || token == lastToken)
                {
                    // Test whether is a Var Definition Instruction
                    if (currentCommandTokens.Count == 5
                        && currentCommandTokens[0] is TypeToken
                        && currentCommandTokens[1] is IdentifierToken
                        && currentCommandTokens[2] is EqualToken
                        && currentCommandTokens[3] is LiteralToken
                        && currentCommandTokens[4] is SemicolonToken)
                    {
                        var variableName = currentCommandTokens[1].Text;
                        var variableValue = currentCommandTokens[3].Text;


                        if (this.Variables.Where(x => x.Name == variableName).Count() > 0)
                        {
                            throw new VariableAlreadyDefinedException(variableName);
                        }


                        if (int.Parse(variableValue) > 255)
                        {
                            throw new VariableOutsideOfRangeException(variableName);
                        }


                        var command = new VarDefinitionInstruction();
                        command.csProgram = this;
                        command.Tokens = currentCommandTokens;

                        // add bytes of program
                        var bytesOfCommand = command.MachineCode();
                        currentProgramAddr = AddBytesOfProgram(machineCodeProgram, currentProgramAddr, bytesOfCommand);



                        var variable = new Variable();
                        variable.Name = variableName;
                        variable.Address = currentVariableAddr;
                        this.Variables.Add(variable);
                        currentVariableAddr++; // TODO: check type of var and increment it according to size of the type



                        this.Commands.Add(command);

                        // NO intruction change memory var area in compile-time
                        //machineCodeProgram.Bytes[this.GetNextVariableAddress()] = Convert.ToByte(variableValue);
                    }
                    // Test whether is an Atribution Instruction
                    else if (currentCommandTokens.Count == 4
                        && currentCommandTokens[0] is IdentifierToken
                        && currentCommandTokens[1] is EqualToken
                        && currentCommandTokens[2] is OperandToken
                        && currentCommandTokens[3] is SemicolonToken)
                    {
                        var variableDestinyName = currentCommandTokens[0].Text;





                        var variableDestiny = this.Variables.Where(x => x.Name == variableDestinyName).FirstOrDefault();
                        if (variableDestiny == null)
                        {
                            throw new UndefinedVariableException(variableDestinyName);
                        }


                        if (currentCommandTokens[2] is LiteralToken)
                        {
                            var literalValue = currentCommandTokens[2].Text;
                            
                            if (int.Parse(literalValue) > 255)
                            {
                                throw new VariableOutsideOfRangeException(variableDestinyName);
                            }

                            var command = new AtributionFromLiteralInstruction();
                            command.csProgram = this;
                            command.Tokens = currentCommandTokens;
                            command.VariableResult = variableDestiny;


                            // add bytes of program
                            var bytesOfCommand = command.MachineCode();
                            currentProgramAddr = AddBytesOfProgram(machineCodeProgram, currentProgramAddr, bytesOfCommand);

                            this.Commands.Add(command);
                            
                            // Atribution intruction DON'T change memory var area!
                            //machineCodeProgram.Bytes[this.GetNextVariableAddress()] = Convert.ToByte(literalValue);
                        }
                        else if (currentCommandTokens[2] is IdentifierToken)
                        {
                            var variableSourceName = currentCommandTokens[2].Text;

                            var variableSource = this.Variables.Where(x => x.Name == variableSourceName).FirstOrDefault();
                            if (variableSource == null)
                            {
                                throw new UndefinedVariableException(variableSourceName);
                            }

                            var command = new AtributionFromVarInstruction();
                            command.csProgram = this;
                            command.Tokens = currentCommandTokens;
                            command.VariableSource = variableSource;
                            command.VariableDestiny = variableDestiny;


                            // add bytes of program
                            var bytesOfCommand = command.MachineCode();
                            currentProgramAddr = AddBytesOfProgram(machineCodeProgram, currentProgramAddr, bytesOfCommand);

                            this.Commands.Add(command);

                            // Atribution intruction DON'T change memory var area!
                            //machineCodeProgram.Bytes[this.GetNextVariableAddress()] = Convert.ToByte(literalValue);
                        }
                    }
                    // Test whether is an Increment Instruction
                    else if (currentCommandTokens.Count == 4
                        && currentCommandTokens[0] is IdentifierToken
                        && currentCommandTokens[1] is ArithmeticSignalToken
                        && currentCommandTokens[2] is ArithmeticSignalToken
                        && currentCommandTokens[3] is SemicolonToken)
                    {
                        var variableName = currentCommandTokens[0].Text;
                        var arithmeticSignal1 = currentCommandTokens[1].Text;
                        var arithmeticSignal2 = currentCommandTokens[2].Text;

                        var variable = this.Variables.Where(x => x.Name == variableName).FirstOrDefault();
                        if (variable == null)
                        {
                            throw new UndefinedVariableException(variableName);
                        }

                        var command = new IncrementInstruction();
                        command.csProgram = this;
                        command.Tokens = currentCommandTokens;
                        command.VariableOperand = variable;
                        if (arithmeticSignal1 == "+" && arithmeticSignal2 == "+")
                        {
                            command.IncrementOperation = EnumIncrementOperation.Increment;
                        }
                        else if (arithmeticSignal1 == "-" && arithmeticSignal2 == "-")
                        {
                            command.IncrementOperation = EnumIncrementOperation.Decrement;
                        }


                        // add bytes of program
                        var bytesOfCommand = command.MachineCode();
                        currentProgramAddr = AddBytesOfProgram(machineCodeProgram, currentProgramAddr, bytesOfCommand);

                        this.Commands.Add(command);
                    }
                    // Test whether is an Arithmetic Instruction
                    else if (currentCommandTokens.Count == 6
                        && currentCommandTokens[0] is IdentifierToken
                        && currentCommandTokens[1] is EqualToken
                        && currentCommandTokens[2] is OperandToken
                        && currentCommandTokens[3] is ArithmeticSignalToken
                        && currentCommandTokens[4] is OperandToken
                        && currentCommandTokens[5] is SemicolonToken)
                    {
                        var variableDestinyName = currentCommandTokens[0].Text;





                        var variableDestiny = this.Variables.Where(x => x.Name == variableDestinyName).FirstOrDefault();
                        if (variableDestiny == null)
                        {
                            throw new UndefinedVariableException(variableDestinyName);
                        }


                        if (currentCommandTokens[2] is IdentifierToken && currentCommandTokens[4] is LiteralToken)
                        {
                            //TODO:
                            //var literalValue = currentCommandTokens[2].Text;

                            //if (int.Parse(literalValue) > 255)
                            //{
                            //    throw new VariableOutsideOfRangeException(variableDestinyName);
                            //}

                            //var command = new AtributionFromLiteralInstruction();
                            //command.csProgram = this;
                            //command.Tokens = currentCommandTokens;
                            //command.VariableResult = variableDestiny;


                            //// add bytes of program
                            //var bytesOfCommand = command.MachineCode();
                            //currentProgramAddr = AddBytesOfProgram(machineCodeProgram, currentProgramAddr, bytesOfCommand);

                            //this.Commands.Add(command);

                            //// Atribution intruction DON'T change memory var area!
                            ////machineCodeProgram.Bytes[this.GetNextVariableAddress()] = Convert.ToByte(literalValue);
                        }
                        else if (currentCommandTokens[2] is IdentifierToken && currentCommandTokens[4] is IdentifierToken)
                        {
                            var variableLeftOperandName = currentCommandTokens[2].Text;
                            var variableRightOperandName = currentCommandTokens[4].Text;
                            var arithmeticOperation = currentCommandTokens[3].Text;

                            var variableLeftOperand = this.Variables.Where(x => x.Name == variableLeftOperandName).FirstOrDefault();
                            if (variableLeftOperand == null)
                            {
                                throw new UndefinedVariableException(variableLeftOperandName);
                            }

                            var variableRightOperand = this.Variables.Where(x => x.Name == variableRightOperandName).FirstOrDefault();
                            if (variableRightOperand == null)
                            {
                                throw new UndefinedVariableException(variableRightOperandName);
                            }

                            var command = new ArithmeticInstruction();
                            command.csProgram = this;
                            command.Tokens = currentCommandTokens;
                            command.VariableLeftOperand = variableLeftOperand;
                            command.VariableRightOperand = variableRightOperand;
                            command.VariableDestiny = variableDestiny;
                            switch (arithmeticOperation)
                            {
                                case "+":
                                    command.ArithmeticOperation = EnumArithmeticOperation.Addition;
                                    break;

                                case "-":
                                    command.ArithmeticOperation = EnumArithmeticOperation.Subtraction;
                                    break;

                                default:
                                    break;
                            }

                            // add bytes of program
                            var bytesOfCommand = command.MachineCode();
                            currentProgramAddr = AddBytesOfProgram(machineCodeProgram, currentProgramAddr, bytesOfCommand);

                            this.Commands.Add(command);
                        }
                    }
                    // Test whether is an Command Instruction
                    else if (currentCommandTokens.Count == 7
                        && currentCommandTokens[0] is CommandToken
                        && currentCommandTokens[1] is OpenParenthesisToken
                        && currentCommandTokens[2] is LiteralToken
                        && currentCommandTokens[3] is CommaToken
                        && currentCommandTokens[4] is IdentifierToken
                        && currentCommandTokens[5] is CloseParenthesisToken
                        && currentCommandTokens[6] is SemicolonToken
                        )
                    {
                        var variableName = currentCommandTokens[4].Text;

                        var variable = this.Variables.Where(x => x.Name == variableName).FirstOrDefault();
                        if (variable == null)
                        {
                            throw new UndefinedVariableException(variableName);
                        }

                        var command = new CommandInstruction();
                        command.csProgram = this;
                        command.Tokens = currentCommandTokens;
                        command.VariableOperand = variable;


                        // add bytes of program
                        var bytesOfCommand = command.MachineCode();
                        currentProgramAddr = AddBytesOfProgram(machineCodeProgram, currentProgramAddr, bytesOfCommand);

                        this.Commands.Add(command);
                    }
                    else
                    {
                        IList<string> items = currentCommandTokens.Select(x => x.Text).ToList();
                        string instruction = items.Aggregate((i, j) => i + " " + j);

                        throw new InvalidInstructionFormatException(instruction);
                    }
                    currentCommandTokens.Clear();
                }
            }

            return machineCodeProgram;
        }

        private static int AddBytesOfProgram(MachineCodeProgram machineCodeProgram, int currentProgramAddr, IList<byte> bytesOfCommand)
        {
            var j = 0;
            for (var i = currentProgramAddr; i < currentProgramAddr + bytesOfCommand.Count; i++)
            {
                machineCodeProgram.Bytes[i] = bytesOfCommand[j++];
            }
            currentProgramAddr = currentProgramAddr + bytesOfCommand.Count;
            return currentProgramAddr;
        }

        private int GetNextVariableAddress()
        {
            // TODO: this should sum the sizes of each variable
            return Constants.BASE_ADDR_VARIABLES + this.Variables.Count - 1;
        }

    }
}
