using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.WSA;

enum CommandType {
    Move,
    Turn,
}

enum FunctionType {
    move,
    turn,
    set_speed,
    sensor_detected,
    print,
    is_laser_on,
}

class Program {
    // public List<CommandType> Commands;
    // public List<float> Moves;
    // public List<float> Turns;

    public int current = 0;

    Robot robot;

    public Dictionary<string, FunctionType> BuiltinFunctions = new Dictionary<string, FunctionType>{
        {"move", FunctionType.move},
        {"turn", FunctionType.turn},
        {"set_speed", FunctionType.set_speed},
        {"sensor_detected", FunctionType.sensor_detected},
        {"print", FunctionType.print},
        {"is_laser_on", FunctionType.is_laser_on},
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

    string[] boolOperators = {
        "!",
        "&&",
        "||",
    };

    string[] compareOperators = {
        "<",
        ">",
        "==",
    };

    string[] intOperators = {
        "+",
        "-",
    };

    string[] symbols = {
        "\"",
        "=",
        "}",
        "//"
    };

    public Program(string code, Robot _robot) {
        Code = code.Split("\n", StringSplitOptions.None);
        robot = _robot;
    }

    static bool IsAlpha(char c) {
        return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c == '_');
    }

    static bool IsNum(char c) {
        return (c >= '0' && c <= '9');
    }

    bool IsSym(string sym) {
        for (int i = 0; i < boolOperators.Length; i++) {
            if (boolOperators[i] == sym) {
                return true;
            }
        }
        for (int i = 0; i < compareOperators.Length; i++) {
            if (compareOperators[i] == sym) {
                return true;
            }
        }
        for (int i = 0; i < intOperators.Length; i++) {
            if (intOperators[i] == sym) {
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

    void GoToMathcingClosingBrace() {
        int openingBraces = 0;

        do {
            if (Code[CurrentLine].Contains('{')) {
                openingBraces++;
            }
            if (Code[CurrentLine].Contains('}')) {
                openingBraces--;
            }
            CurrentLine++;
        } while (openingBraces > 0);
    }

    public void Run() {
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
                    GoToMathcingClosingBrace();
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
                if (ints["i"] == 3) {
                    int asdf = 23423;
                }
                int exprStart = index;
                int exprEnd = exprStart;
                while (exprEnd < line.Length && line[exprEnd] != '{') {
                    exprEnd++;
                }

                string expr = line.Substring(exprStart, exprEnd - exprStart);

                if (!EvaluateBoolExpr(expr)) {
                    GoToMathcingClosingBrace();
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
                    Debug.Log(EvaluateIntExpr(args));
                }
                break;
                case FunctionType.set_speed:
                    robot.SetSpeed(float.Parse(args));
                break;
            }
        }

        if (notKeyword && ints.TryGetValue(token, out _)) { 
            string assign = NextToken(line, index, out var exprStart);
            if (assign == "=") {
                ints[token] = EvaluateIntExpr(line.Substring(exprStart, line.Length - exprStart));
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

    int EvaluateIntExpr(string expr) {
        List<int> valueStack = new List<int>();
        List<string> operatorStack = new List<string>();
        int index = 0;
        while (index < expr.Length) {
            string token = NextToken(expr, index, out index);

            if (IsNum(token[0])) {
                valueStack.Add(int.Parse(token));
            } else if (ints.TryGetValue(token, out var varInt)) {
                valueStack.Add(varInt);
            } else {
                operatorStack.Add(token);
                // for (int i = 0; i < boolOperators.Length; i++) {
                //     if (token == boolOperators[i]) {
                //         operatorStack.Add(token);
                //     }
                // }
            }
        }

        if (valueStack.Count == 1) {
            return valueStack[0];
        }

        int valueSum = valueStack[0];
        for (int i = 0; i < operatorStack.Count; i++) {
            switch (operatorStack[i]) {
            case "+":
                valueSum += valueStack[i + 1];
                break;
            case "-":
                valueSum -= valueStack[i + 1];
                break;
            }
        }

        return valueSum;
    }

    bool EvaluateBoolExpr(string expr) {
        List<bool> valueStack = new List<bool>();
        List<Operator> operatorStack = new List<Operator>();
        int wordStart = 0;

        string firstIntExpr = "";

        int index = 0;
        while (index < expr.Length) {
            string token = NextToken(expr, index, out index);

            bool isIntExpr = false;
            bool toAdd;
            if (token == "false") {
                valueStack.Add(false);
            } else if (token == "true") {
                valueStack.Add(true);
            } else if (BuiltinFunctions.TryGetValue(token, out var functionType)) {
                switch (functionType) {
                    case FunctionType.sensor_detected:
                        valueStack.Add(robot.SensorDetected());
                    break;
                    case FunctionType.is_laser_on: {
                        int parenStart = index;
                        while (expr[parenStart] != '(') {
                            parenStart++;
                        }

                        int parenEnd = parenStart;

                        while (expr[parenEnd] != ')') {
                            parenEnd++;
                        }

                        string args = expr.Substring(parenStart+1, parenEnd-parenStart-1);
                        valueStack.Add(robot.IsLaserOn(EvaluateIntExpr(args)));
                    }
                    break;
                }
            } else {
                bool isBoolOp = false;
                for (int i = 0; i < boolOperators.Length; i++) {
                    if (token == boolOperators[i]) {
                        operatorStack.Add((Operator)i);
                        isBoolOp = true;
                        break;
                    }
                }
                if (!isBoolOp) {
                    bool isCompOp = false;
                    for (int i = 0; i < compareOperators.Length; i++) {
                        if (compareOperators[i] == token) {
                            isCompOp = true;
                            break;
                        }
                    }

                    if (!isCompOp) {
                        firstIntExpr += $" {token}";
                    } else {
                        int firstInt = EvaluateIntExpr(firstIntExpr);

                        string secondIntExpr = "";
                        int i2 = index;

                        while (i2 < expr.Length) {
                            string iToken = NextToken(expr, i2, out i2);
                            bool isIExpr = false;
                            if (iToken.Length > 0 && IsNum(iToken[0])) {
                                isIExpr = true;
                            }

                            for (int i = 0; i < intOperators.Length; i++) {
                                if (intOperators[i] == iToken) {
                                    isIExpr = true;
                                    break;
                                }
                            }
                            
                            if (!isIExpr) {
                                break;
                            }

                            secondIntExpr += $" {iToken}";
                        }

                        int secondInt = EvaluateIntExpr(secondIntExpr);

                        bool value = false;
                        switch (token) {
                            case ">":
                            value = firstInt > secondInt;
                            break;
                            case "<":
                            value = firstInt < secondInt;
                            break;
                            case "==":
                            value = firstInt == secondInt;
                            break;

                            default:
                                Debug.LogError("kys");
                            break;
                        }
                        valueStack.Add(value);
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
    Color KeywordColor;
    [SerializeField]
    Color OperatorColor;
    [SerializeField]
    Color BraceColor;
    [SerializeField]
    Color FunctionColor;
    [SerializeField]
    Color ValueColor;
    [SerializeField]
    Color CommentColor;


    [SerializeField]
    float MoveSpeed = 2;
    [SerializeField]
    float TurnSpeed = 90;
    [SerializeField]
    float instructionPeriod = 0.25f;
    [SerializeField]
    Button RunButton;
    [SerializeField]
    TMP_InputField CodeField;
    [SerializeField]
    TMP_Text HighlightedText;
    
    [SerializeField]
    RectTransform InstructionPointer;
    [SerializeField]
    Transform robotStart;
    [SerializeField]
    Sensor sensor;

    [SerializeField]
    Laser[] lasers;

    float startY;

    bool isRunningCommand = false;

    float timeAccumulator = 0;

    float Speed = 0;

    

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
        Speed = 0;
        transform.rotation = robotStart.rotation;
        timeAccumulator = 0;
        program = new Program(CodeField.text, this);
    }

    void Awake() {
        // transform.gameObject.SetActive(false);
        startY = InstructionPointer.localPosition.y;

        program = new Program(CodeField.text, this);

        RunButton.onClick.AddListener(() => {
            if (!isRunning) {
                program = new Program(CodeField.text, this);
                isRunning = true;
                RunButton.GetComponentInChildren<TMP_Text>().text = "Reset";
            } else {
                Reset();
                RunButton.GetComponentInChildren<TMP_Text>().text = "Run";
            }
        });

        CodeField.onValueChanged.AddListener(OnTextChanged);
        // ResetButton.onClick.AddListener(() => {
        //     if (!isRunning) {
        //         Debug.Log("KYS");
        //         program = new Program(CodeField.text);
        //         isRunning = true;
        //     } else {
        //         isRunning = false;
        //     }
        // });
        // Debug.Log("slkhfkjladh");
        Reset();
    }

    void Start() {
        OnTextChanged(CodeField.text);
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
                program.Run();
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

        // if (moveValue < 0)
        // {
        //     move *= -1;
        // }

        transform.position += transform.forward * Speed * Time.deltaTime;


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

    public void SetSpeed(float speed) {
        Speed = speed;
    }

    public bool SensorDetected() {
        return sensor.Innit;
    }

    public bool IsLaserOn(int index) {
        return lasers[index].On;
    }


    private string EscapeRichText(string input) {
        return input.Replace("<", "&lt;").Replace(">", "&gt;");
    }

    private string keywordPattern = @"\b(if|while|int)\b";
    private string bracePattern = @"(\(|\)|\{|\}|==|>|<|=|\+|\-)";
    private string boolValuePattern = @"\b(true|false)\b";
    private string numberPattern = @"\b\d+\b";
    // private string boolValuePattern = @"\b(true|false)\b";
    private string commentPattern = @"\/\/.*";

    private string ApplySyntaxHighlighting(string input) {
        string escapedInput = input;
        escapedInput = Regex.Replace(escapedInput, bracePattern, $"<color=#{ColorUtility.ToHtmlStringRGB(BraceColor)}>$0</color>");
        escapedInput = Regex.Replace(escapedInput, keywordPattern, $"<color=#{ColorUtility.ToHtmlStringRGB(KeywordColor)}>$0</color>");
        escapedInput = Regex.Replace(escapedInput, boolValuePattern, $"<color=#{ColorUtility.ToHtmlStringRGB(ValueColor)}>$0</color>");
        escapedInput = Regex.Replace(escapedInput, numberPattern, $"<color=#{ColorUtility.ToHtmlStringRGB(ValueColor)}>$0</color>");
        escapedInput = Regex.Replace(escapedInput, commentPattern, $"<color=#{ColorUtility.ToHtmlStringRGB(CommentColor)}>$0</color>");
        return escapedInput;
    }

    private void OnTextChanged(string input) {
        HighlightedText.text = ApplySyntaxHighlighting(input);
        // CodeField.text = ApplySyntaxHighlighting(input);
    }
}