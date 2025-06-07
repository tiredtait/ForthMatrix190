# ForthMatrix190
A library for creating and manipulating mathematical matrixes, made for the MATH-190 Linear Algebra course, hence the name. 

Under construction with new features added as I progress through the course.  Currently you can create, populate and print matrixes.  

Useful words:


InitMatrix ( Row Col "Variable" -- creates a matrix of Row by Col stored in word "Variable") 
AllocateMatrix ( Row Col -- Matrix creates a matrix of Row by Col placed on the stack ) 
Matrix@ ( Row Col Matrix-- Returns the value at that location of the matrix ) 
Matrix! ( Value Row Col Matrix -- pushes Value to the locaton of the matrix )
.MatrixRow ( Row Matrix -- prints out a given row )
.Matrix ( Matrix --   Prints out the matrix )
FillMatrixRow ( N1 N2 . . . Row Matrix -- Fills a row with data from the colums.  1 2 3 1 Matrix FillMatrixRow fills the first row of Matrix with 1,2,3 in that order)
MatrixColumns ( Matrix -- n returns the number of columns in the matrix ) 
MatrixColumns ( Matrix - n returns the number of rows in the matrix )
FillMatrix ( N1 N2 . . . Matrix -- Fills the entire matrix from the stack )  
DumpMatrix ( Matrix -- N1 N2 ... Len Dumps the content of the matrix to the stack followed by the number of items dumped )
.LaTeXMatrix ( Matrix --  Prints out the matrix in LaTex format )
MultiplyRow ( Coefficient Row Matrix -- Multiples a single row by Coefficient )
DivideRow ( Coefficient Row Matrix -- Divides a single row by Coefficient )
MultAndAddRows ( Coefficient Row1 Row2 TargetMatrix -- Multiply Row1 by Coefficient and adds it to Row2, storing the result in Row2 and leaving Row1 unchanged )
DivAndAddRows ( Coefficient Row1 Row2 TargetMatrix -- Divide Row1 by Coefficient and adds it to Row2, storing the result in row2 and leaving row 1 unchanged )
SubtractRows ( Row1 Row2 TargetMatrix -- Subtracts Row1 From Row2, storing the result in Row2 and leaving Row1 unchanged ) 
SwapRows ( Row1 Row2 TargetMatrix -- Swap two rows )
S*, S/ ( N Matrix -- Multiplies/divides the matrix by the scalar value
DuplicateMatris ( Matrix "<spaces>" Name -- Creates a new matrix with the same idmenstions and value as the first matrix with the address sotred in word Name )
M+, M- ( Matrix1 Matrix2 -- adds/subtracts Matrix1 from Matrix2, storing the result in Matrix2)
TransposeMatrix ( works with square matrixes )
M* ( Matrix1 Matrix2 -- OutputMatrix Multiplies two matrixes and drops the address pointer of an output matrix ) 
InvertMatrix ( Matrix -- calculates the inverse of the matrix in-place )
Determinant ( TargetMatrix -- n computes the determinant of the matrix and puts it on the stack. Works with any matrix with n>=2 )
MatrixColumnSlice { Start End TargetMatrix -- Matrix  returns a matrix consisting of colums Start to End of the Target matrix }
MatrixRowSlice { Start End TargetMatrix -- Matrix  returns a matrix consisting of rows Start to End of the Target matrix }

Vector functions: 
ColumnVector ( flag that defines vector as a column )
RowVector ( Flag that defines a vector as a row ) 
InitVector ( Orientation ELEMENTS -- "Name" creates a new vector stored as "name" with N elements and orientation ) 
VectorSize ( Vector -- N puts the size of the vector on the stack ) 
VectorOrientation ( Vector -- N puts the size of the vector on the stack )
VectorElement@ ( element Vector -- n gets the nth element of the vector with 1 being the first )
VectorElement! ( content element Vector -- Sets the nth element of the vector to conent with 1 being the first )
FillVector ( n  . . . Vector -- Fills the vector with elements from the stack ) 
.VectorOrientation ( Vector -- Prints if a vector is column or row ) 
.Vector ( Vector -- Prints the vector to stdout ) 
.LaTeXVector ( Vector -- Prints the vector to stdout formatted for LaTeX ) 
V* ( Vector1 Vector2 -- n Multiplies two vectors and places the answer on the stack.  Only works with row v col matrixes ) 
VectorLength ( Vector -- n returns the length of the vector )

building blocks:
Square: Squares the number
Sqrt: Returns the integer square root, currently only works for numbers under 100
-1^: raises -1 to the power of the number at the top of the stack, effectively odds/evens

Todo: 
RREF 
