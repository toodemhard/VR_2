using System;
using System.Collections.Generic;
using System.Data.Common;
using NUnit.Framework;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;
using UnityEngine.UI;

enum CommandType {
    Move,
    Turn,
}

enum FunctionType {
    move,
    turn,
}


class Program {
    // public List<CommandType> Commands;
    // public List<float> Moves;
    // public List<float> Turns;

    public int current = 0;

    public Dictionary<string, int> BuiltinFunctions = new Dictionary<string, int>{
        {"move", 0},
        {"turn", 1},
        {"print", 2},
    };

    public Dictionary<string, int> ints = new Dictionary<string, int>();

    // public Dictionary<string, int> BuiltinFunctions = new Dictionary<string, int>{

    public string[] Code;
    public int CurrentLine = 0;

    enum Operator {
        Not,
        And,
        Or,
    }


    // char[] symbols = {
    //     '!',
    //     '&',
    //     '|',
    //     '+',
    //     '-',
    //     '*',
    //     '/',
    //     '{',
    //     '}'
    // };

    string[] operatorSymbols = {
        "!",
        "&&",
        "||",
    };

    public Program(string code) {
        Code = code.Split("\n", StringSplitOptions.None);
    }

    static bool IsAlpha(char c) {
        return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
    }
    bool IsSym(string sym) {
        for (int i = 0; i < operatorSymbols.Length; i++) {
            if (operatorSymbols[i] == sym) {
                return true;
            }
        }

        return false;
    }

    static string RemoveWhiteSpace(string str) {
        return string.Join("", str.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
    }

    public void Run(Robot robot) {
        var line = Code[CurrentLine];

        for (int i = 0; i < line.Length; i++) {
            if (IsAlpha(line[i])) {
                int nameStart = i;
                int nameEnd = nameStart;

                while (IsAlpha(line[nameEnd])) {
                    nameEnd++;
                }

                string word = line.Substring(nameStart, nameEnd - nameStart);
                bool notKeyword = false;
                switch (word) {
                    case "if":
                    int exprStart = nameEnd;
                    // while (exprStart < line.Length && char.IsWhiteSpace(line[exprStart]))
                    // {
                    //     exprStart++;
                    // }

                    int exprEnd = exprStart;
                    while (exprEnd < line.Length && line[exprEnd] != '{')
                    {
                        exprEnd++;
                    }

                    string expr = line.Substring(exprStart, exprEnd - exprStart);

                    string condition = string.Join("", expr.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
                    Debug.Log($"Condition: \"{condition}\"");

                    if (!EvaluateBoolExpr(expr)) {
                        while(RemoveWhiteSpace(Code[CurrentLine]) != "}") {
                            CurrentLine++;
                        }
                    }

                    break;

                    default:
                        notKeyword = true;
                    break;

                }

                int funcID;
                if (notKeyword && BuiltinFunctions.TryGetValue(word, out funcID)) {
                    int parenStart = nameEnd;
                    while (line[parenStart] != '(') {
                        parenStart++;
                    }

                    int parenEnd = parenStart;

                    while (line[parenEnd] != ')') {
                        parenEnd++;
                    }

                    string arg = line.Substring(parenStart+1, parenEnd-parenStart-1);

                    switch (funcID) {
                        case 0: {
                        }
                        break;
                        case 1: {
                        }
                        break;
                        case 2: {
                            Debug.Log(arg);
                        }
                        break;

                    }
                }

                break;
            }
        }

        CurrentLine++;
    }

    string NextToken(string line, int wordStart, out int end) {
        // int wordEnd = wordStart;
        while (wordStart < line.Length) {
            if (!char.IsWhiteSpace(line[wordStart])) {
                if (IsAlpha(line[wordStart])) {
                    int wordEnd = wordStart; 
                    while (wordEnd < line.Length) {
                        if (!IsAlpha(line[wordEnd])) {
                            break;
                        }
                        wordEnd++;
                    }

                    // var d = false!;
                    end = wordEnd;
                    return line.Substring(wordStart, wordEnd - wordStart);
                } else {
                    for (int i = 0; i < 2; i++) {
                        var token = line.Substring(wordStart, i + 1);
                        if (IsSym(token)) {
                            end = wordStart + i + 1;
                            return token;
                        }
                    }
                }

            }

            wordStart++;
        }

        end = wordStart;
        return "";
    }

    bool EvaluateBoolExpr(string expr) {
        List<bool> valueStack = new List<bool>();
        List<Operator> operatorStack = new List<Operator>();
        int wordStart = 0;

        int index = 0;
        while (index < expr.Length) {
            string token = NextToken(expr, index, out index);
            Debug.Log($"token: {token}");

            if (token == "false") {
                valueStack.Add(false);
            } else if (token == "true") {
                valueStack.Add(true);
            } else {
            for (int i = 0; i < operatorSymbols.Length; i++) {
                if (token == operatorSymbols[i]) {
                    operatorStack.Add((Operator)i);
                }
            }
            }
        }

        bool sumValue = valueStack[0];
        int valueIndex = 1;
        for (int i = 0; i < operatorStack.Count; i++) {
            switch (operatorStack[i]) {
                case Operator.Not:
                // valueStack[valueIndex] = !valueStack[valueIndex];
                sumValue = !sumValue;
                break;
                case Operator.And:
                sumValue = sumValue && valueStack[valueIndex];

                valueIndex++;
                break;

                case Operator.Or:
                sumValue = sumValue || valueStack[valueIndex];

                valueIndex++;
                break;
            }

        }
        // if wordStart(I)

        return sumValue;
    }
}

// class Program2 {

// }

public class Robot : MonoBehaviour
{
    [SerializeField]
    float MoveSpeed = 2;
    [SerializeField]
    float TurnSpeed = 90;
    [SerializeField]
    float instructionPeriod = 0.25f;

    [SerializeField]
    Button RunBtn;
    [SerializeField]
    TMP_InputField CodeField;
    [SerializeField]
    RectTransform InstructionPointer;
    float startY;

    bool isRunningCommand = false;

    float timeAccumulator = 0;

    

    CommandType currentCommand;
    float commandStartTime;
    float commandDuration;

    


    float moveValue = 0;
    float moveAccumulator = 0;

    float turnValue = 0;
    float turnAccumulator = 0;

    // string[] program = {};
    Program program;
    int programCounter = 0;
    bool isRunning = false;

    void Awake() {
        startY = InstructionPointer.localPosition.y;

        program = new Program(CodeField.text);
    }
    void Start()
    {
        // program = "move 2 turn 90 move 3 turn -45 move -10".Split();

        // RunBtn.onClick.AddListener(() => {
        //     Debug.Log("KYS");
        //     program = CodeField.text.Split();
        //     programCounter = 0;
        //     // isRunning = true;
        // });

        // var kys = new Program("print(asdf)    print(vhb)");
        // kys.Run(this);
        

    }

    // Update is called once per frame
    void Update()
    {

        // var time = Time.time;

        // if (Input.GetKeyDown(KeyCode.E)) {
        //     program.CurrentLine--;
        // }

        // if (Input.GetKeyDown(KeyCode.R)) {
        //     program.CurrentLine++;
        // }

        // if (!isRunningCommand) {
        //     timeAccumulator += Time.deltaTime;

        //     while (program.CurrentLine < program.Code.Length && timeAccumulator >= instructionPeriod && !isRunningCommand) {
        //         timeAccumulator -= instructionPeriod;
        //     }
        // }

        if (Input.GetKeyDown(KeyCode.R) && program.CurrentLine < program.Code.Length) {
            program.Run(this);

            float y = 0;
            if (program.CurrentLine < CodeField.textComponent.textInfo.lineCount) {
                var lineInfo = CodeField.textComponent.textInfo.lineInfo[program.CurrentLine];
                y = (lineInfo.ascender + lineInfo.descender) / 2;
            } else if (CodeField.textComponent.textInfo.lineCount > 0){
                var lineInfo = CodeField.textComponent.textInfo.lineInfo[program.CurrentLine-1];
                y = lineInfo.descender - 8;
            } else {
                y = CodeField.textComponent.textBounds.min.y;
            }
            // var y = CodeField.textComponent.textInfo.lineInfo[program.CurrentLine].ascender;
            var pos = InstructionPointer.localPosition;
            pos.y = y;
            InstructionPointer.anchoredPosition = pos;
        }


        if (isRunningCommand) {
            switch (currentCommand) {
                case CommandType.Move: {
                    var move = MoveSpeed * Time.deltaTime;
                    if (moveValue < 0) {
                        move *= -1;
                    }

                    transform.position += transform.forward * move;
                    moveAccumulator += move;

                    if (Mathf.Abs(moveAccumulator) >= Mathf.Abs(moveValue)) {
                        isRunningCommand = false;
                    }
                    break;
                }
                case CommandType.Turn: {
                    var turn = TurnSpeed * Time.deltaTime;
                    if (turnValue < 0) {
                        turn *= -1;
                    }
                    transform.Rotate(Vector3.up * turn);
                    turnAccumulator += turn;

                    if (Mathf.Abs(turnAccumulator) >= MathF.Abs(turnValue)) {
                        isRunningCommand = false;

                    }
                    // transform.position += transform.forward * MoveSpeed * Time.deltaTime;

                    break;
                }
            }
            // Debug.Log(Time.time);
            // if (Time.time - commandStartTime >= commandDuration)
            // {
            //     isRunningCommand = false;
            // }
        }
        
    }
    void Run3(Program program) {

    }

    void Run2(string[] program) {
        while (!isRunningCommand && programCounter < program.Length) {
            switch (program[programCounter]) {
                case "if":
                case "move":{
                    currentCommand = CommandType.Move;
                    moveValue = float.Parse(program[programCounter+1]);
                    moveAccumulator = 0;
                    // commandStartTime = Time.time;
                    // commandDuration = moveDistance / MoveSpeed;
                    isRunningCommand = true;
                    programCounter++;
                    break;
                }
                case "turn": {
                    currentCommand = CommandType.Turn;
                    turnValue = float.Parse(program[programCounter+1]);
                    turnAccumulator = 0;
                    // commandStartTime = Time.time;
                    // commandDuration = turnAmount / TurnSpeed;
                    isRunningCommand = true;
                    programCounter++;
                    break;
                }
            }
            programCounter++;
        }
    }

    // void Run(Program program) {
    //     int move_index = 0;
    //     int turn_index = 0;
    //     foreach (var commandType in program.Commands) {
    //         switch (commandType) {
    //             case CommandType.Move: {
    //                 transform.po
    //                 Time.time


    //                 break;
    //             }

    //         }

    //     }

    // }
}
