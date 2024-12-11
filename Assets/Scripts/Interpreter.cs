using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class Interpreter : MonoBehaviour
{
    struct Token {
        public Token(TokenType type, String literal) {
            Type = type;
            Literal = literal;

        }

        public TokenType Type;
        public String Literal;
    }
    
    enum TokenType {
        Move,
        For,
        Int,
        LParen,
        RParen,
        LBrace,
        RBrace,
    }

    [SerializeField]
    Button RunBtn;

    bool running;

        
    void Awake() {
        // RunBtn.onClick.AddListener(() =>)

    }
    void Start()
    {
        Eval(new Token[]{
            new Token(TokenType.For, ""),
            new Token(TokenType.Int, "5"),
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Run(string code) {
        string[] stuff = code.Split();
        foreach (var x in stuff) {
            Debug.Log(x);
        }
    }
    
    void Eval(Token[] tokens) {
        for (int tokenIndex = 0; tokenIndex < tokens.Length; tokenIndex++) {
            var token = tokens[tokenIndex];
            switch (token.Type) {
                case TokenType.Move:
                break;
                case TokenType.For:
                    // int count = Int32.Parse(tokens[tokenIndex+1].Literal);
                    // for (int index = 0; index < count; index++) {
                    //     int lparenIndex = tokenIndex;
                    //     int rparenIndex;
                    //     for (int lparenIndex = tokenIndex; index < tokens.Length; tokenIndex++) {
                    //         if ()

                    //     }
                    //     // Eval(tokens[5..7]);
                    //     if tokens[tokenIndex]
                    //     Pars
                    //     Debug.Log("KYS");
                    // }

                    // tokenIndex += 1;
                break;
                default:
                break;
            }
            

        }

    }



}
