using System;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

enum CommandType {
    Move,
    Turn,
}

class Program {
    public List<CommandType> Commands;

    public List<float> Moves;
    public List<float> Turns;

    public int current = 0;
}

// class Program2 {

// }

public class Robot : MonoBehaviour
{
    [SerializeField]
    float MoveSpeed = 2;
    [SerializeField]
    float TurnSpeed = 90;

    bool isRunningCommand = false;

    

    CommandType currentCommand;
    float commandStartTime;
    float commandDuration;


    float moveValue = 0;
    float moveAccumulator = 0;

    float turnValue = 0;
    float turnAccumulator = 0;

    string[] program;
    int programCounter = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        program = "move 2 turn 90 move 3 turn -45 move -10".Split();
    }

    // Update is called once per frame
    void Update()
    {
        while (!isRunningCommand && programCounter < program.Length) {
            Run2(program);
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

    void Run2(string[] program) {
        while (!isRunningCommand && programCounter < program.Length) {
            switch (program[programCounter]) {
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
