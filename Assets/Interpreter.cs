using System;
using UnityEngine;

public class Interpreter : MonoBehaviour
{
    struct Token {
        public TokenType type;
        public String literal;
    }
    
    enum TokenType {
        Move,
        While,
        Int,
    }

        
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void Parse(Token[] tokens) {
        for (int i = 0; i < tokens.Length; i++) {
            var token = tokens[i];
            switch (token.type) {
                case TokenType.Move:
                break;

                case 

            }
            

        }

    }



}
