from solve import *
from junk import *
import re

code = '''
B$ B$ L" B$ L" B$ L# B$ v" B$ v# v# L# B$ v" B$ v# v# L$ L# 
    ? B= v# I!
    S
    B.
      B$ v$ B/ v# I% 
      BT I" BD B% v# I% SOL>F
  S 
I'''.replace('\n', '').replace('  ', ' ').replace('  ', ' ').replace('  ', ' ').replace('  ', ' ').replace('  ', ' ').replace('  ', ' ').replace('  ', ' ').replace('  ', ' ').replace('  ', ' ').replace('  ', ' ').replace('  ', ' ').replace('  ', ' ').replace('  ', ' ').replace('  ', ' ').replace('  ', ' ').replace('  ', ' ')
print(code)
code = 'B$ B$ L! B$ v! v! L1 L2 ? B= v2 I! S B. B$ B$ v1 v1 B/ v2 I% BT I" BD B% v2 I% SOL>F I'
print(code)

def e(vv, no_prefix=False):
  acc = 0
  v = 'LR'
  if no_prefix or vv[0] != 'U':
    v= ''
  v += vv + ''
  t = {'U': 0, 'R': 1, 'D': 2, 'L': 3}
  for i in v:
    acc <<= 2
    acc |= t[i]

  if vv[-1] == 'U':
    acc << 2
  return acc
q = code + base94encode(e('UUUDDD'))
print (decode_string(icfp_post("""B. S%#(/} """ + q.strip()).text[1:]))

def solve_n(n, data, best):
  if data['meta'].get('optimizer') or data['meta'].get('programmed'): return
  url = 'https://patcdr.net/carl/icfp2024/field/%d/solution/1'%(data['timestamp'])
  long = requests.get(url).text
  if 'Ysolve' in long: return
  dirs = re.findall('[LURD]+', long)[0]
  sol = 'B. S' + encode_string('solve ' + str(n) + ' ') + ' '
  sol += code
  sol += base94encode(e(dirs, n in {'lambdaman13', 'lambdaman10', 'lambdaman9', 'lambdaman21'}))
  print(n, len(sol))
  if len(sol) >= best: return
  print(n, data)
  q = submit('lambdaman', str(n), len(sol), sol, {'optimizer': True, 'url': url})         
  print(n, q)

scoreboard = requests.get('https://patcdr.net/carl/icfp2024/scoreboard/json').json()
for prob in range(1, 22):
  probs = requests.get('https://patcdr.net/carl/icfp2024/solutions/lambdaman/lambdaman%d/json'%(prob)).json()
  for i in probs['datas']:
    name = 'lambdaman%d'%(prob)
    if i['received_score'] == -1: continue
    solve_n(name, i, scoreboard[name]['received_score'])
    scoreboard = requests.get('https://patcdr.net/carl/icfp2024/scoreboard/json').json()
