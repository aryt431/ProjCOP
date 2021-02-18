﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public Piece[,] Pieces { get; set; }
    private Piece selectedPiece;

    private const float SQUARE_SIZE  = 1.0F; //Square is 1 meter by 1 meter
    private const float SQUARE_OFFSET  = 0.5F; //Offset to center a piece

    private int selectionX = -1;
    private int selectionZ = -1;

    public List<GameObject> chessPiecesPrefabs;

    private List<GameObject> activeChessPieces;

    private void Start()
    {
        //Spawn all pieces
        SpawnAllChessPieces();
    }

    private void Update()
    {
        UpdateSelection();
        DrawChessBoard();
    }

    private void UpdateSelection()
    {
        //If there is no camera
        if(!Camera.main)
            return;

        RaycastHit hit;

        //If mouse is over the board update selection variables to current position
        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("Chess Plane")))
        {
            Debug.Log(hit.point); //Prints position to console for testing

            //Store X and Z values
            selectionX = (int) hit.point.x;
            selectionZ = (int) hit.point.z;
        }

        //If not over the board default to -1, -1
        else
        {
            selectionX = -1;
            selectionZ = -1;
        }
    }

    //Given an index in the ChessPiecesPrefab list spawn that pieces at position
    private void SpawnChessPieces(int index, int x, int z)
    {
        GameObject go = Instantiate(chessPiecesPrefabs[index], GetSquareCenter(x,z), Quaternion.identity) as GameObject; //Create it as a game object
        go.transform.SetParent(transform);

        //White Knights need a rotaion in both x and z
        if(index == 4)
          go.transform.Rotate(-90.0f, 0.0f, -90.0f, Space.Self); //For rotated piece since prefab is for x,y not x,z
        //Black Knights need a rotaion in both x and z
        else if(index == 10)
          go.transform.Rotate(-90.0f, 0.0f, 90.0f, Space.Self); //For rotated piece since prefab is for x,y not x,z
        //All pieces need to be rotates in the x directions
        else
          go.transform.Rotate(-90.0f, 0.0f, 0.0f, Space.Self); //For rotated piece since prefab is for x,y not x,z

        Pieces[x,z] = go.GetComponent<Piece>();
        // TODO: add setPosition method in Piece class and add line of code
        // Pieces[x,z].setPosition(x,z);
        activeChessPieces.Add(go);
    }

    private void SpawnAllChessPieces()
    {
        activeChessPieces = new List<GameObject>();
        Pieces = new Piece[8,8];

        //Spawn Kings
        SpawnChessPieces(0, 3, 0);
        SpawnChessPieces(6, 4, 7);

        //Spawns Queen
        SpawnChessPieces(1, 4, 0);
        SpawnChessPieces(7, 3, 7);

        //Spawn Rooks
        SpawnChessPieces(2, 0, 0);
        SpawnChessPieces(2, 7, 0);
        SpawnChessPieces(8, 0, 7);
        SpawnChessPieces(8, 7, 7);

        //Spawn Bishops
        SpawnChessPieces(3, 2, 0);
        SpawnChessPieces(3, 5, 0);
        SpawnChessPieces(9, 2, 7);
        SpawnChessPieces(9, 5, 7);

        //Spawn Knights
        SpawnChessPieces(4, 1, 0);
        SpawnChessPieces(4, 6, 0);
        SpawnChessPieces(10, 1, 7);
        SpawnChessPieces(10, 6, 7);

        //Spawn Pawns
        for(int i = 0; i < 8; i++)
        {
            SpawnChessPieces(5, i, 1);
            SpawnChessPieces(11, i, 6);
        }
    }

    //Takes in squares position and returns a Vector3 with that squares center
    private Vector3 GetSquareCenter(int x, int z)
    {
        Vector3 origin = Vector3.zero;
        origin.x += (SQUARE_SIZE * x) + SQUARE_OFFSET;
        origin.y = 0;
        origin.z += (SQUARE_SIZE * z) + SQUARE_OFFSET;
        return origin;
    }

    //Draws out a grid on chess board
    //Make sure you have gizmos on to see it
    private void DrawChessBoard()
    {
        Vector3 widthLine = Vector3.right * 8;
        Vector3 heightLine = Vector3.forward * 8;

        //Loops thru 8 x 8 and draws a line
        for(int i = 0; i <= 8; i++)
        {
            Vector3 start = Vector3.forward * i;
            Debug.DrawLine(start, start + widthLine);

            for(int j = 0; j <= 8; j++)
            {
                start = Vector3.right * j;
                Debug.DrawLine(start, start + heightLine);
            }
        }

        //Draw the and X over the square the mouse hovers over if its hovering over the board
        if (selectionX >= 0 && selectionZ >= 0)
        {
            Debug.DrawLine(Vector3.forward * selectionZ + Vector3.right * selectionX, Vector3.forward * (selectionZ + 1) + Vector3.right * (selectionX + 1));
            Debug.DrawLine(Vector3.forward * (selectionZ + 1) + Vector3.right * selectionX, Vector3.forward * selectionZ + Vector3.right * (selectionX + 1));
        }
    }
}
