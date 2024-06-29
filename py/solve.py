import requests

letter_min = 33
letter_max = 126
base = 94
assert letter_max - letter_min + 1 == base
ordered_letters = '''abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!"#$%&'()*+,-./:;<=>?@[\]^_`|~ \n'''

letter_encoder = bytearray([0] * 128)
for i, c in enumerate(ordered_letters):
    letter_encoder[ord(c)] = i + letter_min
letter_encoder = letter_encoder.decode()

def decode_string(s):
    return ''.join(ordered_letters[ord(c) - letter_min] for c in s)

def encode_string(s):
    result = ''.join(letter_encoder[ord(c)] for c in s)
    assert '\0' not in result
    return result

req = None
def submit(problem_type, problem_name, expected_score, solution, meta):
    global req
    data = {
        'problem_type': problem_type,
        'problem_name': problem_name,
        'expected_score': expected_score,
        'solution': solution,
        'meta': meta
    }
    req = requests.post('https://patcdr.net/carl/icfp2024/solve', json=data)
    return req.json()

if __name__ == '__main__':
    answer = 'S' + encode_string('solve lambdaman3 DLRRDRLLLLLUURLURRLULUURRRRRDLLLRDRRDLRD')
    rez = submit('lambdaman',
        'lambdaman3',
        len(answer),
        answer,
        {'ByHand': True})
    print(rez)
