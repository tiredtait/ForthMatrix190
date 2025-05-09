# ForthMatrix190
A library for creating and manipulating mathematical matrixes, made for the MATH-190 Linear Algebra course, hence the name. 

Under construction with new features added as I progress through the course.  Currently you can create, populate and print matrixes.  

Useful words:


InitMatrix ( Row Col -- creates a matrix of Row by Col ) 
Matrix@ ( Row Col Matrix-- Returns the value at that location of the matrix ) 
Matrix! ( Value Row Col Matrix -- pushes Value to the locaton of the matrix )
.MatrixRow ( Row Matrix -- prints out a given row )
.Matrix ( Matrix --   Prints out the matrix )
FillMatrixRow ( N1 N2 . . . Row Matrix -- Fills a row with data from the colums.  1 2 3 1 Matrix FillMatrixRow fills the first row of Matrix with 1,2,3 in that order)
MatrixColumns ( Matrix -- n returns the number of columns in the matrix ) 
MatrixColumns ( Matrix - n returns the number of rows in the matrix )
FillMatrix ( N1 N2 . . . Matrix -- Fills the entire matrix from the stack )  
.LaTeXMatrix ( Matrix --  Prints out the matrix in LaTex format )
MultiplyRow ( Coefficient Row Matrix -- Multiples a single row by Coefficient )
DivideRow ( Coefficient Row Matrix -- Divides a single row by Coefficient )
MultAndAddRows ( Coefficient Row1 Row2 TargetMatrix -- Multiply Row1 by Coefficient and adds it to Row2, storing the result in Row2 and leaving Row1 unchanged )
DivAndAddRows ( Coefficient Row1 Row2 TargetMatrix -- Divide Row1 by Coefficient and adds it to Row2, storing the result in row2 and leaving row 1 unchanged )
SubtractRows ( Row1 Row2 TargetMatrix -- Subtracts Row1 From Row2, storing the result in Row2 and leaving Row1 unchanged ) 
SwapRows ( Row1 Row2 TargetMatrix -- Swap two rows )
S*, S/ ( N Matrix -- Multiplies/divides the matrix by the scalar value
DuplicateMatris ( Matrix "<spaces>" Name -- Creates a new matrix with the same idmenstions and value as the first matrix with the address sotred in word Name )

Todo:: 

M+, M- ( Matrix1 Matrix2 -- adds/subtracts Matrix1 from Matrix2, storing the result in Matrix2)

TransposeMatrix


