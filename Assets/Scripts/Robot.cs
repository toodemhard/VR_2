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
    print,
}

class Program {
    // public List<CommandType> Commands;
    // public List<float> Moves;
    // public List<float> Turns;

    public int current = 0;

    public Dictionary<string, FunctionType> BuiltinFunctions = new Dictionary<string, FunctionType>{
        {"move", FunctionType.move},
        {"turn", FunctionType.turn},
        {"print", FunctionType.print},
    };

    public Dictionary<string, int> ints = new Dictionary<string, int>();

    // public Dictionary<string, int> BuiltinFunctions = new Dictionary<string, int>{

    List<bool> ScopeReturn = new List<bool>();
    List<int> ReturnStack = new List<int>();

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

    string[] symbols = {
        "\"",
        "=",
        "}",
        "//"
    };

    public Program(string code) {
        Code = code.Split("\n", StringSplitOptions.None);
    }

    static bool IsAlpha(char c) {
        return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
    }

    static bool IsNum(char c) {
        return (c >= '0' && c <= '9');
    }

    bool IsSym(string sym) {
        for (int i = 0; i < operatorSymbols.Length; i++) {
            if (operatorSymbols[i] == sym) {
                return true;
            }

        }
        for (int i = 0; i < symbols.Length; i++) {
            if (symbols[i] == sym) {
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

        int index = 0;
        // while (index < line.Length) {
        string token = NextToken(line, index, out index);

        bool notKeyword = false;
        switch (token) {
            case "if": {
                int exprStart = index;
                int exprEnd = exprStart;
                while (exprEnd < line.Length && line[exprEnd] != '{') {
                    exprEnd++;
                }

                string expr = line.Substring(exprStart, exprEnd - exprStart);

                string condition = string.Join("", expr.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
                Debug.Log($"Condition: \"{condition}\"");

                if (!EvaluateBoolExpr(expr)) {
                    while(RemoveWhiteSpace(Code[CurrentLine]) != "}") {
                        CurrentLine++;
                    }
                } else {
                    ScopeReturn.Add(false);
                }
            }
            // while (exprStart < line.Length && char.IsWhiteSpace(line[exprStart]))
            // {
            //     exprStart++;
            // }

            break;

            case "while": {
                int exprStart = index;
                int exprEnd = exprStart;
                while (exprEnd < line.Length && line[exprEnd] != '{') {
                    exprEnd++;
                }

                string expr = line.Substring(exprStart, exprEnd - exprStart);

                if (!EvaluateBoolExpr(expr)) {
                    while(RemoveWhiteSpace(Code[CurrentLine]) != "}") {
                        CurrentLine++;
                    }
                } else {
                    ScopeReturn.Add(true);
                    ReturnStack.Add(CurrentLine);
                }
            }




            break;

            case "int": 
            string varName = NextToken(line, index, out index);
            if (NextToken(line, index, out index) != "=") {
                Debug.LogError("syntax error");
            }

            int value;
            string stringValue = NextToken(line, index, out index);
            if (!int.TryParse(stringValue, out value)) {
                Debug.LogError($"not int: {stringValue}");
            }

            ints.Add(varName, value);

            break;

            case "}": {
                if (ScopeReturn[ScopeReturn.Count - 1]) {
                    CurrentLine = ReturnStack[ReturnStack.Count - 1] - 1; 
                    ReturnStack.RemoveAt(ReturnStack.Count - 1);
                }
                ScopeReturn.RemoveAt(ScopeReturn.Count - 1);
            }
            break;

            case "//": { }
            break;

            default:
                notKeyword = true;
            break;

        }

        FunctionType functionType;
        if (notKeyword && BuiltinFunctions.TryGetValue(token, out functionType)) {
            int parenStart = index;
            while (line[parenStart] != '(') {
                parenStart++;
            }

            int parenEnd = parenStart;

            while (line[parenEnd] != ')') {
                parenEnd++;
            }

            string args = line.Substring(parenStart+1, parenEnd-parenStart-1);

            switch (functionType) {
                case FunctionType.move: {
                    robot.Move(float.Parse(args));
                }
                break;
                case FunctionType.turn: {
                    robot.Turn(float.Parse(args));
                }
                break;
                case FunctionType.print: {
                    Debug.Log(EvaluateStringExpr(args));
                }
                break;

            }
        }

        // for (int i = 0; i < line.Length; i++) {
        //     if (IsAlpha(line[i])) {
        //         int nameStart = i;
        //         int nameEnd = nameStart;

        //         while (IsAlpha(line[nameEnd])) {
        //             nameEnd++;
        //         }

        //         string word = line.Substring(nameStart, nameEnd - nameStart);


        //         break;
        //     }
        // }

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
                } else if (IsNum(line[wordStart])) {
                    int wordEnd = wordStart; 
                    while (wordEnd < line.Length) {
                        if (!IsNum(line[wordEnd])) {
                            break;
                        }
                        wordEnd++;
                    }

                    // var d = false!;
                    end = wordEnd;
                    return line.Substring(wordStart, wordEnd - wordStart);
                } else {
                    int wordEnd = wordStart; 
                    while (wordEnd <= line.Length) {
                        var token = line.Substring(wordStart, wordEnd - wordStart);
                        if (IsSym(token)) {
                            end = wordEnd;
                            return token;
                        }

                        wordEnd++;
                    }
                }

            }

            wordStart++;
        }

        end = wordStart;
        return "";
    }


    string EvaluateStringExpr(string expr) {
        int index = 0;
        string token = NextToken(expr, index, out index);

        int intValue;
        if (ints.TryGetValue(token, out intValue)) {
            return intValue.ToString();
        }

        // if (token == "\"") {

        //     while ('')

        // }

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
    Button RunButton;
    [SerializeField]
    Button ResetButton;
    [SerializeField]
    TMP_InputField CodeField;
    [SerializeField]
    RectTransform InstructionPointer;
    [SerializeField]
    Transform robotStart;
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

    void Reset() {
        isRunning = false;
        isRunningCommand = false;
        program.CurrentLine = 0;

        var pos = robotStart.position;
        transform.position = pos;
        Debug.Log(robotStart.position.y);
        transform.rotation = robotStart.rotation;
    }

    void Awake() {
        // transform.gameObject.SetActive(false);
        startY = InstructionPointer.localPosition.y;

        program = new Program(CodeField.text);

        RunButton.onClick.AddListener(() => {
            if (!isRunning) {
                Debug.Log("KYS");
                program = new Program(CodeField.text);
                isRunning = true;
                RunButton.GetComponentInChildren<TMP_Text>().text = "Reset";
            } else {
                // Reset();
                RunButton.GetComponentInChildren<TMP_Text>().text = "Run";
            }
        });
        // ResetButton.onClick.AddListener(() => {
        //     if (!isRunning) {
        //         Debug.Log("KYS");
        //         program = new Program(CodeField.text);
        //         isRunning = true;
        //     } else {
        //         isRunning = false;
        //     }
        // });
        Debug.Log("slkhfkjladh");
        Reset();
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

        if (isRunning && !isRunningCommand) {
            timeAccumulator += Time.deltaTime;

            while (program.CurrentLine < program.Code.Length && timeAccumulator >= instructionPeriod && !isRunningCommand) {
                timeAccumulator -= instructionPeriod;
                program.Run(this);
            }
        }

        // if (Input.GetKeyDown(KeyCode.R) && program.CurrentLine < program.Code.Length) {
        //     program.Run(this);
        // }

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

    public void Move(float distance) {
        currentCommand = CommandType.Move;
        moveValue = distance;
        moveAccumulator = 0;
        isRunningCommand = true;
    }

    public void Turn(float angle) {
        currentCommand = CommandType.Turn;
        turnValue = angle;
        turnAccumulator = 0;
        isRunningCommand = true;
    }
}
