
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

: .LaTeXMatrix ( Matrix --  Prints out the matrix in LaTex format )
   ." \begin{bmatrix}" CR
   DUP MatrixRows 1 + 1 DO \ loop through the rows, 1 offset
	DUP MatrixColumns 1 + 1 DO
		DUP j i ROT Matrix@ . \ print the 
		DUP MatrixColumns i > IF ." & " THEN \ Separator for all but the last one
	LOOP
	." \\" CR
   LOOP
   ." \end{bmatrix}" CR
;

: MultiplyRow ( Coefficient Row Matrix -- Multiples a single row by Coefficient )
	
	DUP MatrixColumns ROT ROT \ need to know how many entries per row
		
	1 SWAP OffsetMatrix \ get to the start of the row
	SWAP 0 DO 
		DUP @ 2 PICK * OVER ! 1 CELLS + \ fetch, multiply and bury
	LOOP
	2DROP 
;

: DivideRow ( Coefficient Row Matrix -- Divides a single row by Coefficient )
	
	DUP MatrixColumns ROT ROT \ need to know how many entries per row
		
	1 SWAP OffsetMatrix \ get to the start of the row
	SWAP 0 DO 
		DUP @ 2 PICK / OVER ! 1 CELLS + \ fetch, multiply and bury
	LOOP
	2DROP 
;

: MultAndAddRows { Coefficient Row1 Row2 TargetMatrix -- Multiply Row1 by Coefficient and adds it to Row2, storing the result in Row2 and leaving Row1 unchanged }
	TargetMatrix MatrixColumns 1+ 1 DO \ Loop through the column
		Row1 i TargetMatrix Matrix@ Coefficient * 
		Row2 i TargetMatrix Matrix@ + 
		Row2 i TargetMatrix Matrix! 
	LOOP
	
;

: DivAndAddRows { Coefficient Row1 Row2 TargetMatrix -- Divide Row1 by Coefficient and adds it to Row2, storing the result in row2 and leaving row 1 unchanged }
	TargetMatrix MatrixColumns 1+ 1 DO \ Loop through the column
		Row1 i TargetMatrix Matrix@ Coefficient /
		Row2 i TargetMatrix Matrix@ + 
		Row2 i TargetMatrix Matrix! 
	LOOP
	
;
: AddRows { Row1 Row2 TargetMatrix -- Add Row1 To Row2, storing the result in Row2 and leaving Row1 unchanged } 
	TargetMatrix MatrixColumns 1+ 1 DO \ Loop through the column
		Row2 i TargetMatrix Matrix@  
		Row1 i TargetMatrix Matrix@ +
		Row2 i TargetMatrix Matrix! 
	LOOP
	
;

: SubtractRows { Row1 Row2 TargetMatrix -- Subtracts Row1 From Row2, storing the result in Row2 and leaving Row1 unchanged } 
	TargetMatrix MatrixColumns 1+ 1 DO \ Loop through the column
		Row2 i TargetMatrix Matrix@  
		Row1 i TargetMatrix Matrix@ -
		Row2 i TargetMatrix Matrix! 
	LOOP
	
;

: SwapRows { Row1 Row2 TargetMatrix -- Swap two rows }
	TargetMatrix MatrixColumns 1+ 1 DO \ Loop through the column
		Row1 i TargetMatrix Matrix@
		Row2 i TargetMatrix Matrix@  
		Row1 i TargetMatrix Matrix! 
		Row2 i TargetMatrix Matrix! 
	LOOP
	
;

: S* ( N Matrix -- Multiplies the matrix elements by N )
	\ technically could just loop through the memories but worth doing it the right way
	DUP MatrixRows 1+ 1 DO
		DUP MatrixColumns 1+ 1 DO
			2DUP j i ROT Matrix@ * \ fetch and multiply
		        OVER j SWAP i SWAP  Matrix! \ bury 
		LOOP
	LOOP

;

: S/ ( N Matrix -- Divide the matrix elements by N )
	\ technically could just loop through the memories but worth doing it the right way
	DUP MatrixRows 1+ 1 DO
		DUP MatrixColumns 1+ 1 DO
			2DUP j i ROT Matrix@ SWAP / \ fetch and divide, order matters here
		        OVER j SWAP i SWAP  Matrix! \ bury 
		LOOP
	LOOP

;

\ Quick test matrix
3 4 InitMatrix TestMatrix
 1  2  3  4
 5  6  7  8
 9 10 11 12
TestMatrix FillMatrix


