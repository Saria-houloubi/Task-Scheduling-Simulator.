﻿
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using ThishreenUniversity.ParallelPro.Enums;
using Tishreen.ParallelPro.Core.Models;
namespace Tishreen.ParallelPro.Core
{
    /// <summary>
    /// The view model and the logic for the scoreboarding alog
    /// </summary>
    public class ScoreBoardingWindowViewModel : BaseViewModel
    {
        #region protected Members
        /// <summary>
        /// The name of this class algorithm
        /// </summary>
        protected string algoName = "ScoreBoarde";
        /// <summary>
        /// Holds the result for the student at each exam
        /// Only created if in exam mode
        /// </summary>
        protected ExamResultModel mExamResult;
        /// <summary>
        /// The list of instructions that came from the main window
        /// </summary>
        protected List<object> mInstructions;
        #endregion

        #region Properties
        /// <summary>
        /// The list that holds all the instructions that the user writes
        /// </summary>
        private List<InstructionWithStatusModel> _Instructions;
        public List<InstructionWithStatusModel> Instructions
        {
            get { return _Instructions; }
            set { SetProperty(ref _Instructions, value); }
        }
        /// <summary>
        /// The list of functional units that we have
        /// </summary>
        public List<FunctionalUnitWithStatusModel> FunctionalUnits { get; set; }
        /// <summary>
        /// The list that holds all the registers that the user writes
        /// </summary>
        public List<RegisterResultModel> Registers { get; set; }
        /// <summary>
        /// The clock cycles for each functional unit
        /// </summary>
        public Dictionary<FunctionsTypes, int> FunctionClockCycle { get; set; }
        /// <summary>
        /// The amount of integer units that can execute functions like Load(LD) Store(SD)
        /// </summary>
        protected int _numberOfLoadUnits = 1;
        public int NumberOfLoadUnits
        {
            get { return _numberOfLoadUnits; }
            set { SetProperty(ref _numberOfLoadUnits, value); }
        }
        /// <summary>
        /// The amount of add/sub units that can execute functions like ADD(LD) SUB(SD)
        /// </summary>
        protected int _numberOfAddUnits = 1;
        public int NumberOfAddUnits
        {
            get { return _numberOfAddUnits; }
            set { SetProperty(ref _numberOfAddUnits, value); }
        }
        /// <summary>
        /// The number of units that can execute the Multiplcation functions MULT
        /// </summary>
        protected int _numberOfMultiplyUnits = 2;
        public int NumberOfMultiplyUnits
        {
            get { return _numberOfMultiplyUnits; }
            set { SetProperty(ref _numberOfMultiplyUnits, value); }
        }
        /// <summary>
        /// The number of units that can execute the Divistion function DIVID
        /// </summary>
        protected int _numberOfDivideUnits = 1;
        public int NumberOfDivideUnits
        {
            get { return _numberOfDivideUnits; }
            set { SetProperty(ref _numberOfDivideUnits, value); }
        }
        /// <summary>
        /// The clock cycle that we are at 
        /// </summary>
        protected int _clockCycle = 0;
        public int ClockCycle
        {
            get { return _clockCycle; }
            set { SetProperty(ref _clockCycle, value); }
        }
        #region Flags
        /// <summary>
        /// A flag to till if we can go to the next clock cycle
        /// </summary>
        protected bool _canGoNextCycle = true;
        public bool CanGoNextCycle
        {
            get { return _canGoNextCycle; }
            set { SetProperty(ref _canGoNextCycle, value); }
        }
        #endregion

        #endregion

        #region Commands
        /// <summary>
        /// Fills the function units table for with the sent data
        /// </summary>
        public DelegateCommand FillFunctionalUnitsCommand { get; protected set; }
        /// <summary>
        /// The command to start scoreboading algorithm for one clock cycle
        /// </summary>
        public DelegateCommand MoveOneCycleCommand { get; protected set; }
        /// <summary>
        /// The command to scoreboard till the end of the algorithm
        /// </summary>
        public DelegateCommand MoveTillEndCommand { get; protected set; }
        /// <summary>
        /// The command that opens the information the user enterd
        /// </summary>
        public DelegateCommand ShowEnteredInformationCommand { get; set; }
        #endregion

        #region Command Metods
        /// <summary>
        /// The method that is called when the command is executed
        /// </summary>
        protected void FillFunctionalUnitsCommandMethod()
        {
            //Create the list
            FunctionalUnits = new List<FunctionalUnitWithStatusModel>();

            //A counter for the id of the functional units
            int conter = 1;

            //Fill the list with the specificed value that the user sent for each function
            for (int i = 1; i <= NumberOfLoadUnits; i++, conter++)
            {
                FunctionalUnits.Add(new FunctionalUnitWithStatusModel(conter, "Load " + i, new Dictionary<FunctionsTypes, bool> { { FunctionsTypes.LD, true }, { FunctionsTypes.SD, true } }));
            }

            for (int i = 1; i <= NumberOfMultiplyUnits; i++, conter++)
            {
                FunctionalUnits.Add(new FunctionalUnitWithStatusModel(conter, "Mult " + i, new Dictionary<FunctionsTypes, bool> { { FunctionsTypes.MULT, true } }));
            }

            for (int i = 1; i <= NumberOfAddUnits; i++, conter++)
            {
                FunctionalUnits.Add(new FunctionalUnitWithStatusModel(conter, "Add " + i, new Dictionary<FunctionsTypes, bool> { { FunctionsTypes.SUB, true }, { FunctionsTypes.ADD, true } }));
            }

            for (int i = 1; i <= NumberOfDivideUnits; i++, conter++)
            {
                FunctionalUnits.Add(new FunctionalUnitWithStatusModel(conter, "Divide " + i, new Dictionary<FunctionsTypes, bool> { { FunctionsTypes.DIV, true } }));
            }

            //If we are in exam mode
            if (IoC.Appliation.IsExamMode)
            {
                //Set the computer solution aside for compartion
                SetExamPropertiesAndStoreSolution();
            }

            RaisePropertyChanged(nameof(FunctionalUnits));
        }
        /// <summary>
        /// The function that will be executed once the <see cref="ShowEnteredInformationCommand"/> is invocked
        /// </summary>
        public void ShowEnteredInformationCommandExecute()
        {
            var functionUnitsCount = new Dictionary<FunctionalUnitsTypes, int>
            {
                { FunctionalUnitsTypes.Add, NumberOfAddUnits },
                { FunctionalUnitsTypes.Load, NumberOfLoadUnits },
                { FunctionalUnitsTypes.Divide, NumberOfDivideUnits },
                { FunctionalUnitsTypes.Multiply, NumberOfMultiplyUnits }
            };
            //The parametes to send
            var parameters = new List<object>
            {
                mInstructions,
                FunctionClockCycle,
                functionUnitsCount
            };

            IoC.UI.ShowWinodw(ApplicationPages.CodeInformationWindow, parameters);
        }
        #endregion

        #region Constructer
        /// <summary>
        /// Default Constructer 
        /// </summary>
        /// <param name="instructionList">The list of instruction that the usere enterted</param>
        /// <param name="functionClockCycle">The clock cycles for each function unit</param>
        public ScoreBoardingWindowViewModel(List<object> instructionList, Dictionary<FunctionsTypes, int> functionClockCycle,string algoName = "ScoreBoarde")
        {
            mInstructions = instructionList;
            //If exam mode save the instructions that the student entered
            if (IoC.Appliation.IsExamMode)
            {
                //Set the start of exam data
                mExamResult = new ExamResultModel
                {
                    AlgorithmName = algoName,
                };
                //Save the instructions
                instructionList.ForEach(item => mExamResult.Instructions.Add((InstructionModel)item));
            }
            //Call on the start to fill the instruction list
            FillInstructionList(instructionList);
            FillRegisterList();
            FunctionClockCycle = functionClockCycle;
            //Create the commands
            FillFunctionalUnitsCommand = new DelegateCommand(FillFunctionalUnitsCommandMethod, () => ExamClockCycle >= 0).ObservesProperty(() => ExamClockCycle);
            MoveOneCycleCommand = new DelegateCommand(StartScoreBoardOneStep);
            MoveTillEndCommand = new DelegateCommand(StartScoreBoardTillTheEnd);
            CorrectExamAndMoveToNextCommand = new DelegateCommand(CorrectAndMoveToNextTestCommandMethod);
            CorrectExamAndEndCommand = new DelegateCommand(EndTestsAndShowResultsCommandMethod);
            ShowEnteredInformationCommand = new DelegateCommand(ShowEnteredInformationCommandExecute);
        }
        /// <summary>
        /// Fills the instruction with status list on the start of the window
        /// </summary>
        /// <param name="instructionList">The instruction list from the user</param>
        protected void FillInstructionList(List<object> instructionList)
        {
            Instructions = new List<InstructionWithStatusModel>();

            if (instructionList == null)
            {
                return;
            }
            //Loop throw the list...
            foreach (var item in instructionList)
            {
                var instruction = (InstructionModel)item;
                //Add it to the instruction with status list
                Instructions.Add(new InstructionWithStatusModel(instruction.ID, instruction.Name, instruction.TargetRegistery, instruction.SourceRegistery01, instruction.SourceRegistery02));
            }
        }
        /// <summary>
        /// Fills the register list
        /// </summary>
        protected void FillRegisterList()
        {
            //Create and fill the list
            Registers = new List<RegisterResultModel>();

            var registerNames = Enum.GetValues(typeof(RegisteriesAndMemory));

            foreach (var item in registerNames)
            {
                var stringValue = Enum.GetName(typeof(RegisteriesAndMemory), item);
                //If it is a registery spot add it to target
                if ((int)item < 31)
                {
                    Registers.Add(new RegisterResultModel(stringValue));
                }
            }
        }
        #endregion

        #region ScroeBorading Algorithm
        /// <summary>
        /// ScoreBoards the instructions to a specified clock cycle
        /// </summary>
        protected void StartScoreBoardTillClockCycle(List<InstructionWithStatusModel> instructions, List<FunctionalUnitWithStatusModel> functionalUnits, List<RegisterResultModel> registers, int toClockCylce)
        {
            while (!ScoreBoard(instructions, functionalUnits, registers, toClockCylce))
            {
                ;
            }
        }
        /// <summary>
        /// ScoreBoard the instruction until it ends
        /// </summary>
        protected void StartScoreBoardTillTheEnd()
        {
            while (!ScoreBoard(Instructions, FunctionalUnits, Registers))
            {
                ;
            }
        }
        /// <summary>
        /// only one clock cycle for the algorithm
        /// </summary>
        protected void StartScoreBoardOneStep() => ScoreBoard(Instructions, FunctionalUnits, Registers);
        /// <summary>
        /// The score board algorithm 
        /// </summary>
        protected bool ScoreBoard(List<InstructionWithStatusModel> instructions, List<FunctionalUnitWithStatusModel> functionalUnits, List<RegisterResultModel> registers, int toClockCycle = -1)
        {
            //If we arreived into the wanted clock cylce end operation
            if (ClockCycle == toClockCycle)
            {
                return true;
            }

            ClockCycle++;

            var instructionLenght = instructions.Count;
            for (int i = 0; i < instructionLenght; i++)
            {
                var instruction = instructions[i];
                if (instruction.IssueCycle == null)
                {
                    //Issue the instruction if all hazerds are gone
                    IssueIfApproved(instruction, functionalUnits, registers);

                    //Restart from top when we issue a new instruction
                    break;
                }
                else if (instruction.ReadCycle == null)
                {
                    //Check if we can read the instruction
                    ReadIfApproved(instruction, functionalUnits, registers);
                }
                else if (instruction.WriteBackCycle != null)
                {
                    //If the value was just ended up from the past clock cycle
                    if (instruction.WriteBackCycle == ClockCycle - 1)
                    {
                        //Clear the units after a write back
                        FreeUpRegisters(registers.SingleOrDefault(item => item.InstructionReservedRegiseter.SingleOrDefault(inst => inst.InstructionId == instruction.ID) != null), instruction.ID);
                        RecheckIfRegistersAreFree(functionalUnits, registers);
                        if (CheckIfDone(instructions))
                        {
                            //Free up all registers       
                            registers.ForEach(item => item.IsBusy = false);
                            //Get the cycle back to the right value
                            ClockCycle--;
                            CanGoNextCycle = false;
                            return true;
                        }
                    }
                }
                else
                {
                    //Execute the instruction
                    ExecuteInstrution(instruction, Instructions, functionalUnits, registers);
                }
            }

            return false;
        }

        #region Which is the right functional unit
        /// <summary>
        /// Checks if the function that is sent can work on the mult functional unit
        /// </summary>
        /// <param name="name">The name of the function</param>
        /// <returns></returns>
        protected bool CanUseMultUnit(string function) => function == FunctionsTypes.MULT.ToString();
        /// <summary>
        /// Checks if the functions can work on the add functional unit
        /// </summary>
        /// <param name="function">The name of the function</param>
        /// <returns></returns>
        protected bool CanUseAddUnit(string function) => (function == FunctionsTypes.ADD.ToString() || function == FunctionsTypes.SUB.ToString());
        /// <summary>
        /// Checks if the functions can work on the integer functional unit
        /// </summary>
        /// <param name="function">The name of the function</param>
        /// <returns></returns>
        protected bool CanUseIntegerUnit(string function) => (function == FunctionsTypes.LD.ToString() || function == FunctionsTypes.SD.ToString());
        /// <summary>
        /// Checks if the functions can work on the divide functional unit
        /// </summary>
        /// <param name="function">The name of the function</param>
        /// <returns></returns>
        protected bool CanUseDivideUnit(string function) => (function == FunctionsTypes.DIV.ToString());
        #endregion

        /// <summary>
        /// Checks if all the hazerds are gone and sets the instruction into issue
        /// </summary>
        /// <param name="instruction">The instruction that we want to issue</param>
        /// <returns></returns>
        protected bool IssueIfApproved(InstructionWithStatusModel instruction, List<FunctionalUnitWithStatusModel> functionalUnits, List<RegisterResultModel> registers)
        {
            foreach (var unit in functionalUnits)
            {

                if (unit.Function.TryGetValue(instruction.Name, out bool value))
                {
                    //Issuee only when functional unit is not busy structural hazard and...
                    if (!unit.IsBusy)
                    {
                        //The target register to write in is free WAW hazard
                        RegisterResultModel registerToTarget;
                        if (instruction.Name == FunctionsTypes.SD)
                        {
                            registerToTarget = registers.SingleOrDefault(reg => reg.Name == instruction.SourceRegistery01);
                        }
                        else
                        {
                            registerToTarget = registers.SingleOrDefault(reg => reg.Name == instruction.TargetRegistery);
                        }

                        instruction.IssueCycle = ClockCycle;

                        //Set it to busy
                        unit.IsBusy = true;
                        //Assign it the operation values
                        unit.WorkingInstructionID = instruction.ID;
                        unit.Operation = instruction.Name.ToString();
                        unit.SourceRegistery01 = instruction.SourceRegistery01;
                        unit.SourceRegistery02 = instruction.SourceRegistery02;
                        unit.TargetRegistery = instruction.TargetRegistery;

                        var operationOnSource01 = registers.SingleOrDefault(reg => reg.Name == instruction.SourceRegistery01);
                        var operationOnSource02 = registers.SingleOrDefault(reg => reg.Name == instruction.SourceRegistery02);
                        //The is null for the load as it loads from memory 
                        if (operationOnSource01 is null || !operationOnSource01.IsBusy)
                        {
                            unit.IsSource01Ready = true;
                        }
                        else
                        {
                            unit.IsSource01Ready = false;
                            unit.WaitingOperationForSource01 = operationOnSource01.Operation;
                        }
                        if (operationOnSource02 is null)
                        {
                            unit.IsSource02Ready = false;
                        }
                        else if (!operationOnSource02.IsBusy)
                        {
                            unit.IsSource02Ready = true;
                        }
                        else
                        {
                            unit.IsSource02Ready = false;
                            unit.WaitingOperationForSource02 = operationOnSource02.Operation;
                        }

                        //Set the register as busy
                        registerToTarget.Operation = unit.Operation == "LD" ? string.Format("F[{0}]", unit.SourceRegistery01) : string.Format("F[{0}]", unit.Name);
                        registerToTarget.IsBusy = true;
                        if (registerToTarget.InstructionReservedRegiseter.Any())
                        {
                            registerToTarget.InstructionReservedRegiseter.LastOrDefault().LastIssued = false;
                        }

                        //Add it to the reserved instructions
                        registerToTarget.InstructionReservedRegiseter.Add(new InstructionRegisterReservationModel
                        {
                            InstructionId = instruction.ID,
                            LastIssued = true
                        });
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// Reads the instruction data when all conditions are meet
        /// </summary>
        /// <param name="instruction">The instruction that we want to read</param>
        /// <returns></returns>
        protected bool ReadIfApproved(InstructionWithStatusModel instruction, List<FunctionalUnitWithStatusModel> functionalUnits, List<RegisterResultModel> registers)
        {
            //Get the functional unit that the instruction is working on
            var unit = functionalUnits.SingleOrDefault(item => item.WorkingInstructionID == instruction.ID);

            //If both Sources are ready to be read
            if (unit.IsSource01Ready &&
                (unit.IsSource02Ready || string.IsNullOrEmpty(unit.SourceRegistery02)))
            {

                //Read the values and set the clock cycle
                instruction.ReadCycle = ClockCycle;

                //Set the amount of the clokc cycles
                unit.Time = FunctionClockCycle.SingleOrDefault(function => function.Key.ToString() == unit.Operation).Value;

                return true;
            }
            return false;
        }
        /// <summary>
        /// executes the instruction each clock cycle until end
        /// </summary>
        protected virtual void ExecuteInstrution(InstructionWithStatusModel instruction, List<InstructionWithStatusModel> instructions, List<FunctionalUnitWithStatusModel> functionalUnits, List<RegisterResultModel> registers)
        {
            //Get the unit that the instruction is working on
            var unit = functionalUnits.SingleOrDefault(item => item.WorkingInstructionID == instruction.ID);

            if (--unit.Time == 0)
            {
                //Set the clock cycle that is completed executing on
                instruction.ExecuteCompletedCycle = ClockCycle;
            }
            else if (unit.Time < 0)
            {
                unit.Time = 0;
                //Get the register the instruction is working on
                var register = registers.SingleOrDefault(reg => reg.InstructionReservedRegiseter.SingleOrDefault(inst => inst.InstructionId == instruction.ID) != null);
                //If the insruction it self is the one reserving the register
                if (register.InstructionReservedRegiseter.FirstOrDefault().InstructionId == instruction.ID)
                {
                    //Then it can write back on it
                    //Else it has to wait unitl it is freed up from the past instruction
                    instruction.WriteBackCycle = ClockCycle;
                    ClearUnitFunction(FunctionalUnits.SingleOrDefault(item => item.WorkingInstructionID == instruction.ID), registers);
                }

            }

        }
        /// <summary>
        /// Frees up the register for WAR WAW hazerds 
        /// </summary>
        /// <param name="register"></param>
        protected void FreeUpRegisters(RegisterResultModel register, int instructionId)
        {
            var instruction = register.InstructionReservedRegiseter.SingleOrDefault(item => item.InstructionId == instructionId);

            //Remove the instruction that is reserving the register
            register.InstructionReservedRegiseter.Remove(instruction);
            //If there is any more items
            if (register.InstructionReservedRegiseter.Any())
            {
                //then assign the working insruction as the first one
                register.IsBusy = true;
            }
            else
            {
                //Free up the registery
                register.IsBusy = false;
            }

        }
        /// <summary>
        /// Clears up the unit after the instruction writes back
        /// </summary>
        /// <param name="unit"></param>
        protected void ClearUnitFunction(FunctionalUnitWithStatusModel unit, List<RegisterResultModel> registers)
        {
            unit.JustFreedUp = true;
            unit.Time = null;
            unit.Operation = null;
            unit.IsBusy = false;
            unit.WorkingInstructionID = 0;
            unit.Operation = null;
            unit.SourceRegistery01 = null;
            unit.SourceRegistery02 = null;
            unit.TargetRegistery = null;
            unit.WaitingOperationForSource01 = null;
            unit.WaitingOperationForSource02 = null;
            unit.IsSource01Ready = false;
            unit.IsSource02Ready = false;
        }
        /// <summary>
        /// Checks if the register are free so we can start reading 
        /// loops on them each time a result is writen back
        /// </summary>
        protected void RecheckIfRegistersAreFree(List<FunctionalUnitWithStatusModel> functionalUnits, List<RegisterResultModel> registers)
        {
            foreach (var unit in functionalUnits)
            {
                if (unit.SourceRegistery01 != null && unit.IsSource01Ready == false)
                {
                    var registery = registers.SingleOrDefault(reg => reg.Name == unit.SourceRegistery01);
                    if (!registery.IsBusy ||
                        (registery.IsBusy && registery.InstructionReservedRegiseter.FirstOrDefault().InstructionId == unit.WorkingInstructionID))
                    {
                        unit.IsSource01Ready = true;
                        unit.WaitingOperationForSource01 = null;
                    }
                }
                if (unit.SourceRegistery02 != null && unit.IsSource02Ready == false)
                {
                    var registery = registers.SingleOrDefault(reg => reg.Name == unit.SourceRegistery02);

                    if (!registery.IsBusy ||
                        (registery.IsBusy && registery.InstructionReservedRegiseter.FirstOrDefault().InstructionId == unit.WorkingInstructionID))
                    {
                        unit.IsSource02Ready = true;
                        unit.WaitingOperationForSource02 = null;
                    }
                }
            }
        }
        /// <summary>
        /// Checks if all instructions has been executed and wrote there values back
        /// </summary>
        /// <returns></returns>
        protected bool CheckIfDone(List<InstructionWithStatusModel> instructions)
        {
            foreach (var instruction in instructions)
            {
                if (instruction.WriteBackCycle == null)
                {
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region Exam mode properties and functions

        #region Properties
        /// <summary>
        /// The clock cycle the student chooses to exam to 
        /// </summary>
        protected int _examClockCycle = 0;
        public int ExamClockCycle
        {
            get { return _examClockCycle; }
            set { SetProperty(ref _examClockCycle, value); }
        }

        #region List Computer holds his solution to compare with the student solution
        /// <summary>
        /// The list that holds all the instructions that the computer solves and is compared with the user input
        /// to set the exam mark
        /// </summary>
        public List<InstructionWithStatusModel> InstructionsExamResult = new List<InstructionWithStatusModel>();
        /// <summary>
        /// The list of functional units that the computer solves and is compared with the user input
        /// to set the exam mark
        /// </summary>
        public List<FunctionalUnitWithStatusModel> FunctionalUnitsExamResult = new List<FunctionalUnitWithStatusModel>();
        /// <summary>
        /// The list that holds all the registers that the computer solves and is compared with the user input
        /// to set the exam mark
        /// </summary>
        public List<RegisterResultModel> RegistersExamResults = new List<RegisterResultModel>();

        #endregion

        #endregion

        #region Exam Commands
        /// <summary>
        /// Corrects the student answears and sets his mark
        /// </summary>
        public DelegateCommand CorrectExamAndMoveToNextCommand { get; protected set; }
        /// <summary>
        /// Corrects the current test and ends the exam
        /// </summary>
        public DelegateCommand CorrectExamAndEndCommand { get; protected set; }
        #endregion

        #region Command methods

        /// <summary>
        /// Corrects and sets this exam mark
        /// +1  for evert correct value
        /// -0.5 for filling wanted nulls
        /// 0 for noncorrect
        /// </summary>
        protected void CorrectExamAndSetMark()
        {
            //The start mark
            float studentMark = 0;
            var fullMark = GetFullMark();

            //Correct instrucitons
            //Get the number of elements inside the list 
            int instructionCount = Instructions.Count;
            //loop throw the instructions
            for (int i = 0; i < instructionCount; i++)
            {
                studentMark += InstructionsExamResult[i].CompareInstructions(Instructions[i]);
            }

            //Correct functional units
            //Get the number of elements inside the list 
            int functionalUnitsCount = FunctionalUnits.Count;
            //loop throw the functional units
            for (int i = 0; i < functionalUnitsCount; i++)
            {
                studentMark += FunctionalUnitsExamResult[i].CompareFunctionUnits(FunctionalUnits[i]);
            }
            //Correct registers
            //Get the number of elements inside the list 
            int RegisterCount = Registers.Count;
            //loop throw the registers
            for (int i = 0; i < RegisterCount; i++)
            {
                studentMark += RegistersExamResults[i].CompareRegisters(Registers[i]);
            }

            //Attach the marks with the exam report 
            mExamResult.StudentMark = studentMark;
            mExamResult.FullMark = fullMark;
            mExamResult.EndTimeInner = DateTime.Now;
            //Attach the exam result to the student 
            IoC.ExamInfo.Results.Add(mExamResult);
        }

        /// <summary>
        /// Corrects the exam and sets the studnet to a new exam
        /// invoked by <see cref="CorrectExamAndMoveToNextCommand"/>
        /// </summary>
        protected void CorrectAndMoveToNextTestCommandMethod()
        {
            //Correct this exam
            CorrectExamAndSetMark();
        }
        /// <summary>
        /// Ends the tests and opens the result window for the student 
        /// invoked by <see cref="CorrectExamAndEndCommand"/>
        /// </summary>
        protected void EndTestsAndShowResultsCommandMethod()
        {
            //Correct the mark first
            CorrectExamAndSetMark();
            //Set the end time
            IoC.ExamInfo.EndTime = DateTime.Now;
            //Show the result window
            IoC.UI.ShowWinodw(ApplicationPages.ResultWindow);
        }
        #endregion

        #region Functions
        /// <summary>
        /// Sets the properties of the exam information for the student
        /// and solves to the clock cycle that is provided by the student 
        /// so it compares it with the student solution
        /// </summary>
        protected void SetExamPropertiesAndStoreSolution()
        {
            //Set the exam clock cycle
            mExamResult.ChoosenClockCycle = ExamClockCycle;
            mExamResult.NumberOfAddUnits = NumberOfAddUnits;
            mExamResult.NumberOfDivideUnits = NumberOfDivideUnits;
            mExamResult.NumberOfLoadUnits = NumberOfLoadUnits;
            mExamResult.NumberOfMultiplyUnits = NumberOfMultiplyUnits;
            mExamResult.StartTimeInner = DateTime.Now;

            //Set the startup values
            Instructions.ForEach(item => InstructionsExamResult.Add(new InstructionWithStatusModel(item)));
            FunctionalUnits.ForEach(item => FunctionalUnitsExamResult.Add(new FunctionalUnitWithStatusModel(item.ID, item.Name, item.Function)));
            Registers.ForEach(item => RegistersExamResults.Add(new RegisterResultModel(item.Name, item.Operation)));
            //Scrore board the instruction to the wanted clockCycle
            StartScoreBoardTillClockCycle(InstructionsExamResult, FunctionalUnitsExamResult, RegistersExamResults, ExamClockCycle);
        }
        /// <summary>
        /// Gets the full mark from the 
        /// <see cref="InstructionsExamResult"/>
        /// <see cref="FunctionalUnitsExamResult"/>
        /// <see cref="RegistersExamResults"/>
        /// </summary>
        /// <returns></returns>
        protected int GetFullMark()
        {
            int fullMark = 0;

            #region Get instrution marks
            foreach (var item in InstructionsExamResult)
            {
                if (item.IssueCycle != null)
                {
                    fullMark++;
                    if (item.ReadCycle != null)
                    {
                        fullMark++;
                        if (item.ExecuteCompletedCycle != null)
                        {
                            fullMark++;
                            if (item.WriteBackCycle != null)
                            {
                                fullMark++;
                            }
                        }
                    }
                }
            }
            #endregion

            #region Get Functional Units marks
            foreach (var item in FunctionalUnitsExamResult)
            {
                //If the item is busy then we can take marks from it
                //In other words there is something to correct in it
                if (item.IsBusy)
                {
                    //Because when the unit is busy at least six fileds must be corrected excpets the source02 if there was not any
                    fullMark += 6;
                    //If there is a second source 
                    if (item.SourceRegistery02 != null)
                    {
                        //Add two more marks on is souce avaible and the name of the function hloding the source
                        fullMark += 2;
                    }
                }
            }
            #endregion

            #region Get Registery Units marks
            foreach (var item in RegistersExamResults)
            {
                //If there is an opeartion in the registery then add a mark on it
                if (item.Operation != null)
                {
                    fullMark++;
                }
            }
            #endregion

            return fullMark;
        }
        #endregion

        #endregion
    }
}