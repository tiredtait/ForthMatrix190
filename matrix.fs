
: InitMatrix ( Row Col -- creates a matrix of Row by Col )  ( var name )
	\ Data format has the rows concatinated to create the colums with two extra cells at the beginning containing the number of rows and the number of columns

	VARIABLE \ 
	2DUP * 2 + CELLS ALLOT \ Row * Col entries plus two for 
                         \ keeping the size
	\ Copy the row to Var[0]
	2DUP * 3 + CELLS HERE SWAP - \ Matrix size +1 extra for moving HERE a cell
	2 PICK SWAP ! ( Row Col Var[0] -> Row Col Row Var[0] )
	\ Copy the col to Var[1]
	2DUP * 2 + CELLS HERE SWAP - 
	! DROP
	
;
: MatrixColumns ( Matrix -- n returns the number of columns in the matrix ) 
  1 CELLS + @
;

: MatrixRows ( Matrix - n returns the number of rows in the matrix )
 @
;

: MatrixOffset ( Row Col Matrix -- Offset Calculates the address offset of the matrix in cell count )
 \ The matrix structure consists of two cells containing the row and col
 \ then the matrix data as rows appended to each other, so a
 \ 3x2 matrix looks like this:
 \ 0[Row]1[Col]
 \ 2[1,1]3[1,2]4[1,3]
 \ 5[2,1]6[2,2]7[2,3]
 \ 0[1,1]1[1,2]2[1,3]
 \ 3[2,1]4[2,2]5[2,3]
 \ So to calculate the offset we need the length of the rows 
 \  (aka the colums) times the number of rows plus the number of 
 \ colums, remembering that rows and colums are 1 offset
 MatrixColumns \ The Column count in the matrix ( Row Col ColCount ) 
 ROT 1 - * ( COL Row * ColCount )
 SWAP 1 - + ( How many cells forward it is )
 2 +  CELLS ( Housekeeping offset and turn to the cell count )
;

: OffsetMatrix ( Row Col Matrix -- Matrix returns the location in memory of the offset spot versus just the offset ) 
  ROT ROT 2 PICK MatrixOffset + 
;

: Matrix@ ( Row Col Matrix-- Returns the value at that location of the matrix ) 

  DUP 2OVER ROT ( Row Col Matrix - Row Col Matrix Matrix Row Col )
  MatrixOffset + @ 
  ROT ROT DROP DROP
;

: Matrix! ( Value Row Col Matrix -- pushes Value to the locaton of the matrix )
 \ Need Value Matrix+MatrixOffset 
 \ So Value Matrix Row Col Matrix MatrixOffset  
 ROT ROT 2 PICK
 MatrixOffset + !
;

: .MatrixRow ( Row Matrix -- prints out a given row )
  \ Generates offset for the first colum of the given row
  DUP MatrixColumns ( Row Matrix ColCount )
  ROT ROT ( ColCount Row Matrix )
  1 Swap OffsetMatrix ( ColCount RowLocation )
  SWAP 0 DO 
  DUP i CELLS + @ . 
  LOOP
  DROP CR
  
;

: .Matrix ( Matrix --   Prints out the matrix )
  DUP MatrixRows 
  CR
  1 + 1 DO 
  i OVER .MatrixRow
  LOOP
  DROP
;

: FillMatrixRow ( N1 N2 . . . Row Matrix -- Fills a row with data from the colums.  1 2 3 1 Matrix FillMatrixRow fills the first row of Matrix with 1,2,3 in that order)
	\ Need to know the number of colums so we know how many to pull
	\ And want to start at the end of the colum because we will
	\ be filling it backwards to clear the stack in the correct
        \ order
	DUP MatrixColumns Rot 1 + Rot 
	1 SWAP OffsetMatrix
	SWAP 1 SWAP DO 
		\ Starting from the end of the next row so decrement first
		\ and fill the previous cell
		1 CELLS - 
		SWAP OVER !  
		\ Should probably refactor so it uses Matrix! but it works from here
	-1 +LOOP \ Countdown
	DROP \ elminate Matrix
;

: FillMatrix { TargetMatrix --  ( N1 N2 . . . Matrix -- Fills the entire matrix from the stack )  }
	\ using variables because otherise there would have to be
        \ some silly level of stack manipulation
	TargetMatrix MatrixRows 
	1 SWAP DO \ reverse process through the matrix
		i TargetMatrix FillMatrixRow
	-1 +LOOP
;
\ Quick test matrix
2 3 InitMatrix TestMatrix
1 1 1 TestMatrix Matrix!
2 1 2 TestMatrix Matrix!
3 1 3 TestMatrix Matrix!
4 2 1 TestMatrix Matrix!
5 2 2 TestMatrix Matrix!
6 2 3 TestMatrix Matrix!
