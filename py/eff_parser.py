from junk import *
import sys

file = open('../Tasks/efficiency/efficiency%s_raw.txt'%(sys.argv[1])).read().split(' ')
if_start = file.index('?')
trunc = file[if_start+1:]
 
def parse(loc):
    token = trunc[loc]
    if token[0] == 'B':
        (left, next_loc) = parse(loc+1)
        (right, next_loc) = parse(next_loc)
        return ((token[1], left, right), next_loc)
    elif token == 'U!':
        (val, next_loc) = parse(loc+1)
        return (('!', val), next_loc)
    elif token[0] == 'v':
        return (('V', base94decode(token[1:])), loc+1)
    elif token[0] == 'I':
        return (('I', base94decode(token[1:])), loc+1)    
    else:
        print(trunc[loc:loc+3])
        1/0

numbers = set()
acc = []
def ifp(v):
    if v:
        acc.append(v)

replace = {}

def lower(tree):
    global biggest
    if tree[0][0] == 'V':
        numbers.add('V'+str(tree[1]))
        return ['V'+str(tree[1])]
    elif tree[0][0] == 'I':
        return [str(tree[1])]
    elif tree[0][0] == '&':
        ifp (lower(tree[1]))
        ifp (lower(tree[2]))
    elif tree[0][0] == '|':
        return (lower(tree[1]) + lower(tree[2]))
    elif tree[0][0] == '!':
        if tree[1][0] == '=':
            rez =  lower(tree[1])
            rez[0] = '!='
            return rez
        return ['-'+lower(tree[1])[0]]
    elif tree[0][0] == '=':
        r = ['=='] + lower(tree[1]) + lower(tree[2])
        return r
    else:
        print(tree)
        1/0


tree = (parse(0))
lower(tree[0])

print('''from z3 import *
s = Solver()''')

for i in sorted(numbers):
    print('%s = Int("%s")'%(i,i))
    print('s.add(%s <= 9, %s >= 1)'%(i,i))

for op, l, r in acc:
    print('s.add(%s %s %s)'%(l, op, r))

print('''
print (s.check())
m = s.model()
numbers = %s'''%(numbers))

print('''
acc = 0
for i in sorted(numbers):
    acc *= 9
    acc += (m[eval(i)].as_long() - 1)

print (acc)
''')
