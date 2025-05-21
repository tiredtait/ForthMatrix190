
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

: AllocateMatrix ( Row Col -- Matrix creates a matrix of Row by Col )  ( var name )
	\ Data format has the rows concatinated to create the colums with two extra cells at the beginning containing the number of rows and the number of columns

	2DUP * 2 + CELLS ALLOCATE DROP \ Row * Col entries plus two for 
                         \ keeping the size
	\ Copy the row to Var[0]
	ROT OVER !

	\ Copy the col to Var[1]
	DUP 1 CELLS + ROT SWAP !  
;

: MatrixColumns ( Matrix -- n returns the number of columns in the matrix ) 
  1 CELLS + @
;

: MatrixRows ( Matrix - n returns the number of rows in the matrix )
 @
;

: .MatrixDim ( Matrix -- prints the dimension of the matrix ) 
DUP MatrixRows . ." by " MatrixColumns .
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
   DROP
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
	2DROP
;

: S/ ( N Matrix -- Divide the matrix elements by N )
	\ technically could just loop through the memories but worth doing it the right way
	DUP MatrixRows 1+ 1 DO
		DUP MatrixColumns 1+ 1 DO
			2DUP j i ROT Matrix@ SWAP / \ fetch and divide, order matters her
		        OVER j SWAP i SWAP  Matrix! \ bury 
		LOOP
	LOOP
	2DROP
;



: CopyMatrix ( Matrix1 Matrix2 -- Copies the contents of matrix1 to matrix2 ) 
	DUP MatrixRows 1+ 1 DO
		DUP MatrixColumns 1+ 1 DO
			OVER j i ROT Matrix@ \ fetch row j column i from Matrix1
			OVER j SWAP i SWAP Matrix! \ and bury it in matrix2
		LOOP
	LOOP
	2DROP
; 


: MatrixSize ( Matrix -- SizeOfMatrix puts the number of elements [row * col] on stack)
	DUP MatrixColumns SWAP MatrixRows * 
;

: DuplicateMatrix ( Row Col -- creates a matrix of Row by Col )  ( var name )
	\ Data format has the rows concatinated to create the colums with two extra cells at the beginning containing the number of rows and the number of columns
	VARIABLE HERE CELL - \ ( Matrix NewMatrix )
	OVER MatrixSize 2 + CELLS ALLOT ( Matrix NewMatrix ) \ Allocate the space for a new matrix, saving the size
	OVER MatrixRows OVER ! DUP MatrixRows . CELL + \ Write the Row number, advance 1 to column number
	OVER MatrixColumns OVER ! DUP @ . CELL + \ same with columns
	SWAP DUP MatrixSize SWAP 2 CELLS + ( NewMatrix MatrixSize Matrix[2] )

	ROT ROT 0 DO ( Matrix NewMatrix ) \ Move the matrix pointer to the beginning of the elements and loop through
		OVER i CELLS + @ OVER i CELLS + !  \ Copy from matrix to matrix
	LOOP
	DROP DROP	
;


: TransposeMatrix ( Matrix -- Transpose the matrix, swap dimensions and copy elements currently works with square ) 
	\ put all of the elements on the stack
	1 \ placeholder for the loop
	OVER MatrixColumns 1 + 1 DO 
		OVER MatrixRows 1 + 1 DO (  r_1 r_2 . . . Matrix r_n||placeholder )
i . j . DUP . CR
\ gotta mathhammer this part so it goes to the right place
			SWAP DUP i j ROT Matrix@ 

		LOOP
	LOOP
	SWAP
	DUP MatrixRows
	OVER MatrixColumns 
	2 PICK !
	OVER 1 CELLS + ! 
	FillMatrix 
	DROP 	
; 

: M+ ( Matrix1 Matrix2 -- Add Matrix1 to Matrix2, storing the result in Matrix2 ) 
	DUP MatrixRows 1 + 1 DO
		DUP MatrixColumns 1 + 1 DO 
			OVER j i ROT Matrix@ 
			OVER j i ROT Matrix@ + \ fetch and sum
			OVER j i ROT Matrix! \ \and store
		LOOP
	LOOP
	2DROP
;


: M- ( Matrix1 Matrix2 -- Subtract Matrix2 From Matrix1, storing the result in Matrix2 ) 
	DUP MatrixRows 1 + 1 DO
		DUP MatrixColumns 1 + 1 DO  
			OVER j i ROT Matrix@ 
			OVER j i ROT Matrix@ - \ fetch and subtract, OOP important
			OVER j i ROT Matrix! \ \and store
		LOOP
	LOOP
	2DROP
;

: ColumnVector 1 ( flag that defines vector as a column ) ;
: RowVector 0 ( Flag that defines a vector as a row )  ;



: InitVector ( Orientation ELEMENTS -- "Name" creates a new vector stored as "name" with N elements and orientation ) 
	\ Data format has the rows concatinated to create the colums with two extra cells at the beginning, the first contains the orientation and the second contains the size of the vector

	VARIABLE \ 
	DUP 2 + CELLS ALLOT \ Vector entries plus two for keeping the size/orientation
				 \ could probably save a cell by using +- for orientation but 
				 \ these days memory is cheap
	DUP 3 + CELLS HERE SWAP - \ Memory location of vector
	( Orientation Elements Vector )
	SWAP OVER 1 CELLS + ! \ Store the element count
	! \ Store the orientation

;
: VectorSize ( Vector -- N puts the size of the vector on the stack ) 
	1 CELLS + @ 
;

: VectorOrientation ( Vector -- N puts the size of the vector on the stack )
	@
; 

: .VectorOrientation ( Vector -- Prints if a vector is column or row ) 
	VectorOrientation ColumnVector = IF
		." Column"
	ELSE
		." Row"
	THEN
	CR

;

: Vector@ ( element Vector -- n gets the nth element of the vector with 1 being the first )
	SWAP 1 + CELLS + @ \ 1 offset to go to the 3rd cell
;

: Vector! ( content element Vector -- Sets the nth element of the vector to conent with 1 being the first )
	SWAP 1 + CELLS + ! \ 1 offset to go to the 3rd cell
;
: FillVector ( n  . . . Vector -- Fills the vector with elements from the stack ) 
	DUP VectorSize
	SWAP 2 CELLS + SWAP \ offset for the housekeeping cells
	1 - 0  SWAP DO \ Get the size of the vector, address space has an offset of 2 because the first two cells have the housekeeping information
		SWAP OVER i CELLS + ! 
	-1 +LOOP \ countdown
	DROP
; 


: .Vector ( Vector -- Prints the vector to stdout ) 
	DUP VectorSize 1 + 1 DO
		DUP i SWAP Vector@ . 
		DUP VectorOrientation ColumnVector = IF \ Colum vector, separate by newlines
			CR
		THEN
	LOOP	
	DROP
;

: .LaTeXVector ( Vector -- Prints the vector to stdout formatted for LaTeX ) 
   ." \begin{bmatrix}" CR
	DUP VectorSize 1 + 1 DO
		DUP i SWAP Vector@ . 
			DUP VectorOrientation ColumnVector = IF \ Colum vector, vertical
					." \\ " CR
				ELSE \ Row vector, horizontal
					DUP VectorSize i > IF \ last element doesn't get a separator
						." & "
					ELSE CR
					THEN
			THEN
	LOOP	
 ." \end{bmatrix}" CR
	DROP
;

: V* ( Vector1 Vector2 -- Result  Multiplies vector 1 by vector 2 and puts the result on the stack ) 
\ note that there are different processes depending on if the vector is a row or column vector
\ at this stage I have only read about <> by <> so that is assumed
\ as I read more
	2DUP VectorSize SWAP VectorSize = IF
		DUP VectorSize 1+ 1 DO \ Multiply the elements
			OVER i SWAP Vector@ OVER i SWAP Vector@ *
			ROT ROT \ put this product at the back
		LOOP
		DROP VectorSize 1 DO \ Sum the products

			+ 	
		LOOP
	ELSE
		2DROP -1 ." Invalid: inequal size"
	THEN
;

: FetchRow ( Row Matrix -- VectorAddr Creates a row vector from the specified row of a matrix ) 
	\ initalize the matrix
	DUP MatrixColumns 2 + CELLS ALLOCATE DROP \ allocate the memory for the matrix 
	\ Set the matrix type
	RowVector OVER !
	\ Set the matrix dimensions
	OVER MatrixColumns OVER 1 CELLS + !
	( Row Matrix Vector )
	OVER MatrixColumns 1 + 1 DO \ Need to pull elements off of the row and drop them into the vector
		2 PICK  i  3 PICK  ( Row Matrix Vector i Row Matrix )
		Matrix@ 
		OVER i SWAP ( Row Matrix Vector Val i Vector )
		Vector!
	LOOP
	SWAP DROP SWAP DROP
;


: FetchColumn ( Column Matrix -- VectorAddr Creates a Column vector from the specified row of a matrix ) 
	\ initalize the matrix
	DUP MatrixRows 2 + CELLS ALLOCATE DROP \ allocate the memory for the matrix 
	\ Set the matrix type
	ColumnVector OVER !
	\ Set the matrix dimensions
	OVER MatrixRows OVER 1 CELLS + !
	( Row Matrix Vector )
	OVER MatrixRows 1 + 1 DO \ Need to pull elements off of the row and drop them into the vector
		i 3 PICK 3 PICK  ( Row Matrix Vector i Row Matrix )
		Matrix@ 
		OVER i SWAP ( Row Matrix Vector Val i Vector )
		Vector!
	LOOP
	SWAP DROP SWAP DROP
;


: M* { Matrix1 Matrix2 -- OutputMatrix Multiples 2 matrixes and puts a pointer to a new matrix with the product of those matrixes on the stack } 
	\ 
	Matrix2 MatrixRows Matrix1 MatrixColumns = IF \ have to be compatible
		\ allocate and size
		Matrix1 MatrixRows Matrix2 MatrixColumns AllocateMatrix 
		\ process 
		\ loop through row
		Matrix1 MatrixRows 1 + 1 DO 
			Matrix2 MatrixColumns 1 + 1 DO 
			\ i is column, j is row
			j Matrix1 FetchRow
			i Matrix2 FetchColumn
			\ fetch the vectors
			( Matrix1 Matrix2 OutputMatrix Matrix1Row[j] Matrix2[i] )
			2DUP V* 
			SWAP FREE DROP SWAP FREE DROP \ multiply the vectors and free them 
			OVER j i ROT Matrix!
			LOOP
		LOOP
	ELSE
		-1 ." Matrix size mismatch"
	THEN
;

: Square ( n - n returns n^2 )
	DUP * 
;

: Sqrt ( n - n returns the integer square root of n ) 
\ Keep testing squares until it goes over, then deduct 1.  Not fast but simple
	DUP 0 <> IF \ return 0 if 0
		1  BEGIN 
			1 +
		2DUP Square  < UNTIL
		1 - SWAP DROP
	THEN
;

\ some quick convience functions
: MatrixA ( Matrix -- n Returns the top right element of a 2x2 matrix ) 1 1 ROT Matrix@ ;
: MatrixB ( Matrix -- n Returns the top left element of a 2x2 matrix ) 1 2 ROT Matrix@ ;
: MatrixC ( Matrix -- n Returns the top left element of a 2x2 matrix ) 2 1 ROT Matrix@ ;
: MatrixD ( Matrix -- n Returns the top left element of a 2x2 matrix ) 2 2 ROT Matrix@ ;

: IsInvertable { Matrix -- n checks if a matrix is invertable and returns true or false }
	Matrix MatrixA Matrix MatrixD * Matrix MatrixB Matrix MatrixC * - 0 <> 
;

: IMScalar { Matrix -- n Calculates the scalar multiplier for the inverse of the matrix and returns to stack }
\ per theorem 7 from Understanding Linear Algebra
	1 Matrix MatrixA Matrix MatrixD * Matrix MatrixB Matrix MatrixC * - / 
;
: IMSwap { Matrix -- Swaps out and negates the matrix for the inverse calculation }
	Matrix MatrixA Matrix MatrixD 1 1 Matrix Matrix! 2 2 Matrix Matrix! \ swap a and d
	Matrix MatrixB NEGATE  1 2 Matrix Matrix! 
	Matrix MatrixC NEGATE  2 1 Matrix Matrix! 
;

: InverseMatrix  ( Matrix -- calculates the inverse of the matrix in-place, currently only works with  )
\ per theorem 7 from Understanding Linear Algebra
	DUP IF  \ Matrix has an inverse
		DUP IMScalar OVER IMSwap SWAP S* 
	ELSE 
		." Matrix does not have an inverse"
		DROP
	THEN
;

: VectorLength ( Vector -- Length computes the length of the vector and returns it to the stack ) 
	1 OVER Vector@ Square SWAP 2 SWAP Vector@ Square + Sqrt 
;
2 3 InitMatrix SampleMatrix
11 12 13
14 15 16
SampleMatrix FillMatrix

ColumnVector 3 InitVector TestVectorC
1 2 3 TestVectorC FillVector

RowVector 3 InitVector TestVectorR
5 6 7 TestVectorR FillVector


\ Quick test matrix.
2 2 InitMatrix TestMatrix
 1  2  3 
 4 \ 5  6 
\ 8 9 10
TestMatrix FillMatrix

2 3 InitMatrix SampleMatrix
11 12 13
14 15 16
SampleMatrix FillMatrix

