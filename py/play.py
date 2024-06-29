import sys
import re
import requests

# Define the custom order for the encoding
custom_order = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!\"#$%&'()*+,-./:;<=>?@[\\]^_`|~ \n"

# Create a dictionary to map the custom order to standard ASCII values
encoding_map = {char: chr(i + 33) for i, char in enumerate(custom_order)}

# Reverse the mapping to decode the encoded string
decoding_map = {v: k for k, v in encoding_map.items()}

def decode_string(encoded_str):
    decoded_chars = []

    if encoded_str[0] == 'S':
        encoded_str = encoded_str[1:]

    for char in encoded_str:
        decoded_char = decoding_map.get(char)
        decoded_chars.append(decoded_char)
    return ''.join(decoded_chars)

def encode_string(decoded_str):
    encoded_chars = []
    for char in decoded_str:
        encoded_char = encoding_map.get(char)
        encoded_chars.append(encoded_char)
    return 'S'+''.join(encoded_chars)

def communicate_with_server(encoded_text):
    # Send the POST request
    response = requests.post(
        "https://boundvariable.space/communicate", 
        data=encoded_text,
        headers={'Authorization': 'Bearer ff993058-d102-4ee8-913f-e1c39614b957'}
    )
    
    # Check for successful response
    if response.status_code == 200:
        # Decode the response
        return response.text
    else:
        return f"Error: Received status code {response.status_code}"

def base94_to_int(base94_str):
    base = len(custom_order)
    result = 0
    for char in base94_str:
        result = result * base + custom_order.index(char)
    return result

def int_to_base94(n):
    base = len(custom_order)
    if n == 0:
        return custom_order[0]
    chars = []
    while n > 0:
        chars.append(custom_order[n % base])
        n //= base
    return ''.join(reversed(chars))


class Token:
    def __init__(self, token, context):
        self.token = token
        self.context = context
        self.indicator = token[0]
        self.body = token[1:]
        self.size = 1

    def __str__(self) -> str:
        return self.indicator + self.body

    def pprint(self, indent=0):
        return '    ' * indent + str(self)
    
    @classmethod
    def ParseProgram(cls, program):
        tokens = program.split()
        return cls.Parse(tokens)

    @classmethod
    def Parse(cls, context):
        token = context[0]
        context = context[1:]
        indicator = token[0]

        if indicator == 'T':
            return BooleanOperator(token, context)
        elif indicator == 'F':
            return BooleanOperator(token, context)
        elif indicator == 'I':
            return IntOperator(token, context)
        elif indicator == 'S':
            return StringOperator(token, context)
        elif indicator == 'U':
            return UnaryOperator(token, context)
        elif indicator == 'B':
            return BinaryOperator(token, context)
        elif indicator == '?':
            return IfStatement(token, context)
        elif indicator == 'L':
            return LambdaAbstraction(token, context)
        elif indicator == 'v':
            return Variable(token, context)
        else:
            raise ValueError(f"Unknown token indicator: {indicator}")

    def apply(self, var, param):
        return self

class BooleanOperator(Token):
    def __init__(self, token, context):
        super().__init__(token, context)

    def evaluate(self):
        return self.indicator == 'T'

class IntOperator(Token):
    def __init__(self, token, context):
        super().__init__(token, context)

    def evaluate(self):
        return base94_to_int(self.body)

class StringOperator(Token):
    def __init__(self, token, context):
        super().__init__(token, context)

    def evaluate(self):
        return decode_string(self.body)

class UnaryOperator(Token):
    def __init__(self, token, context):
        super().__init__(token, context)
        self.operand = Token.Parse(context)
        self.size = 1 + self.operand.size

    def pprint(self, indent=0):
        return '   ' * indent + str(self) + '\n' + self.operand.pprint(indent+1)

    def evaluate(self):
        if self.body == '-':
            return -self.operand.evaluate()
        elif self.body == '!':
            return not self.operand.evaluate()
        elif self.body == '#':
            return base94_to_int(self.operand.evaluate())
        elif self.body == '$':
            return int_to_base94(self.operand.evaluate())
        else:
            raise ValueError(f"Unknown unary operator: {self.operator}")
    
    def apply(self, var, param):
        self.operand = self.operand.apply(var, param)
        return self

class BinaryOperator(Token):
    def __init__(self, token, context):
        super().__init__(token, context)
        self.operand1 = Token.Parse(context)
        self.operand2 = Token.Parse(context[self.operand1.size:])
        self.size = 1 + self.operand1.size + self.operand2.size

    def pprint(self, indent=0):
        return '    ' * indent + str(self) + '\n' + self.operand1.pprint(indent+1) + '\n' + self.operand2.pprint(indent+1)

    def evaluate(self):
        if self.body == '$':
            if isinstance(self.operand1, LambdaAbstraction):
                return self.operand1.evaluate(self.operand2)
            else:
                raise ValueError(f"Operand1 is not a LambdaAbstraction: {self.operand1}")

        if self.body == '+':
            return self.operand1.evaluate() + self.operand2.evaluate()
        elif self.body == '-':
            return self.operand1.evaluate() - self.operand2.evaluate()
        elif self.body == '*':
            return self.operand1.evaluate() * self.operand2.evaluate()
        elif self.body == '/':
            return self.operand1.evaluate() // self.operand2.evaluate()
        elif self.body == '%':
            return self.operand1.evaluate() % self.operand2.evaluate()
        elif self.body == '<':
            return self.operand1.evaluate() < self.operand2.evaluate()
        elif self.body == '>':
            return self.operand1.evaluate() > self.operand2.evaluate()
        elif self.body == '=':
            return self.operand1.evaluate() == self.operand2.evaluate()
        elif self.body == '|':
            return self.operand1.evaluate() or self.operand2.evaluate()
        elif self.body == '&':
            return self.operand1.evaluate() and self.operand2.evaluate()
        elif self.body == '.':
            return self.operand1.evaluate() + self.operand2.evaluate()
        elif self.body == 'T':
            return self.operand2[:self.operand1.evaluate()].evaluate()
        elif self.body == 'D':
            return self.operand2[self.operand1.evaluate():].evaluate()
        else:
            raise ValueError(f"Unknown binary operator: {self.operator}")

    def apply(self, var, param):
        self.operand1 = self.operand1.apply(var, param)
        self.operand2 = self.operand2.apply(var, param)
        return self

class IfStatement(Token):
    def __init__(self, token, context):
        super().__init__(token, context)
        self.condition = Token.Parse(context)
        self.if_true = Token.Parse(context[self.condition.size:])
        self.if_false = Token.Parse(context[self.condition.size+self.if_true.size:])
        self.size = 1 + self.condition.size + self.if_true.size + self.if_false.size

    def pprint(self, indent=0):
        return '  ' * indent + str(self) + '\n' + self.condition.pprint(indent+1) + '\n' + self.if_true.pprint(indent+1) + '\n' + self.if_false.pprint(indent+1)

    def evaluate(self):
        condition = self.condition.evaluate()
        if condition:
            return self.if_true
        else:
            return self.if_false

    def apply(self, var, param):
        self.condition = self.condition.apply(var, param)
        self.if_false = self.if_false.apply(var, param)
        self.if_true = self.if_true.apply(var, param)
        return self

class LambdaAbstraction(Token):
    def __init__(self, token, context):
        super().__init__(token, context)
        self.func = Token.Parse(context)
        self.size = 1 + self.func.size

    def pprint(self, indent=0):
        return '    ' * indent + str(self) + '\n' + self.func.pprint(indent+1)

    def evaluate(self, param):
        return self.func.apply(self.body, param)

    def apply(self, var, param):
        self.func = self.func.apply(var, param)
        return self

class Variable(Token):
    def __init__(self, token, context):
        super().__init__(token, context)

    def evaluate(self):
        return self
    
    def apply(self, var, param):
        if var == self.body:
            return param
        else:
            return self



text = """

B$ 
    L" 
        B$ 
            L# 
                B$ 
                    v" 
                    B$ 
                        v# 
                        v# 
            L# 
                B$ 
                    v" 
                    B$ 
                        v# 
                        v#
    SL

"""

text = """
B. SF B$ B$ L" B$ L" B$ L# B$ v" B$ v# v# L# B$ v" B$ v# v# L$ L# ? B= v# I" v" B. v" B$ v$ B- v# I" Sl I#,
"""

def main():
        text = input("Enter program (or 'exit' to quit): ")
        
        if text.lower() == 'exit':
            return

        program = Token.ParseProgram(text)
        print(program.pprint())
        print()

        while isinstance(program, Token):
            program = program.evaluate()
            print()
            print(program.pprint())

        print('Final answer: ', program)

        # Communicate with the server and get the response
        # encoded_text = encode_string(text)
        # response = communicate_with_server(encoded_text)
        # print(response)
        # try:
        #     decoded_response = decode_string(response)
        # except TypeError:
        #     tokens = response.split()
        #     decoded_response = evaluate_program(tokens)

        # Print the decoded response
        # print("Response from server:")
        # print(decoded_response)
        
        return

if __name__ == "__main__":
    main()
