# The Cat is #I"!! ICFP 2024

Thanks again for another fun ICFP contest!

## C# Projects of Note:

### ConsoleRunner

Repl like swiss army knife. Did a great many things I don't know about. (SOMEONE FILL THIS OUT)  Used vis to parse out ICFP expressions for Efficency and debugging.

### Core

Library that handles ICFP parsing/evalutin/writing. And a spot to keep the submission logic.

### CoreTests

We had tests! Yeah, unit testing is good. Someone else fill this in with less 

### LambdamanSolver

Solves Lambdaman puzzles in various ways.

### Spaceship

???

### ThreeDimensional

## Python

Python was used for various utility oneoffs in the contest. Mostly Efficiency, but also a bit for other problems.

### Flask app (icfp2024.py)

We had a server setup as a passthrough for submissions to keep track of submissions. Here's a screenshot. It was mostly for data managment purposes. 

## Solving Strategies

### LambdaMan

First day, used Simulated Annealing to find paths. Later compressed the strings (py3/optimize_lambda.py) to 2 bit int sequencies and unpacked them. 

Later, used RNG stuff. ANNA PLZ FILL THIS OUT

Finally, hand coded 6, 9, and 19, with varing degress of success. 

### Rockets

I HAVE NO IDEA WHAT Y'ALL DID HERE

### 3D

I ALSO HAVE NO IDEA WHAT Y'ALL DID HERE

### Efficency

For most of these, we just formatted the code nicer and looked at what it was doing. Some were also solved by replacing large constants with smaller ones and `echo`ing the evaluation. Throw those numbers into [oeis](https://oeis.org/) to find the functions.

1. Ran the function. 
2. Take the constant at the top
3. Sum the three numbers
4. 40th fibonacci number
5. Mersenne prime > 1,000,000
6. Index of the fibonacci prime > 30 (42)
7. py/7.cnf Converted the boolean expressions to Dimacs and threw it at Kissat
8. py/7.cnf Converted the boolean expressions to Dimacs and threw it at Kissat
9. py/9.py: Threw the non Sudoku at Z3. Was dissapointed. Thought harder and then added more constraints. Then ran to a fixed point using Bash.
10. py/10.py: Threw the Sudoku at Z3. 
11. py/11.py: Threw the Sudoku at Z3. Had to search for the smallest solution.
12. Euler's Totient Function phi(1234567)+1
13. len('na'*(2**28)+'heyjude') (536870919)

### Hello

We poked around your server till we have three of them. Then solved language_test using our evaluator to finish the quadfecta. 
